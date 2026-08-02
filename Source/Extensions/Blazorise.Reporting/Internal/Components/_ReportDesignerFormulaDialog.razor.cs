#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.CodeEditor;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Internal dialog used to edit report formula expressions.
/// </summary>
public partial class _ReportDesignerFormulaDialog
{
    #region Members

    private const int DefaultRoundDecimalPlaces = 2;

    private const string FormulaLanguageId = "blazorise-report-formula";

    private static readonly IReadOnlyList<CodeEditorLanguageDefinition> FormulaLanguages =
    [
        new()
        {
            Id = FormulaLanguageId,
            Aliases = ["Report formula"],
            Tokenizer = new()
            {
                IgnoreCase = true,
                DefaultToken = string.Empty,
                Tokens =
                [
                    new() { Pattern = "\\s+", Token = "white" },
                    new() { Pattern = "\\{[^}\\r\\n]+\\}", Token = "variable" },
                    new() { Pattern = "\\{[^}\\r\\n]*$", Token = "invalid" },
                    new() { Pattern = "\"(?:\\\\.|[^\"\\\\])*\"", Token = "string" },
                    new() { Pattern = "'(?:\\\\.|[^'\\\\])*'", Token = "string" },
                    new() { Pattern = "\"(?:\\\\.|[^\"\\\\])*$", Token = "invalid" },
                    new() { Pattern = "'(?:\\\\.|[^'\\\\])*$", Token = "invalid" },
                    new() { Pattern = "\\b\\d+(?:\\.\\d+)?\\b", Token = "number" },
                    new() { Pattern = "\\b(?:IsNull|IsNullOrEmpty|Coalesce|Contains|StartsWith|EndsWith|Upper|Lower|Length|Round|Abs|Today|Now|Count|Sum|Average|Avg|Minimum|Min|Maximum|Max)(?=\\s*\\()", Token = "predefined" },
                    new() { Pattern = "\\b(?:if|then|else|true|false|null)\\b", Token = "keyword" },
                    new() { Pattern = "[+\\-*/%!=<>?:&|]+", Token = "operator" },
                    new() { Pattern = "[(),]", Token = "delimiter.parenthesis" },
                    new() { Pattern = "[A-Za-z_][A-Za-z0-9_.]*", Token = "identifier" },
                ],
            },
        },
    ];

    private static readonly CodeEditorOptions FormulaEditorOptions = new()
    {
        AutomaticLayout = true,
        Minimap = false,
        LineNumbers = true,
        WordWrap = true,
        TabSize = 4,
        ScrollBeyondLastLine = false,
        AdditionalOptions = new()
        {
            ["folding"] = false,
            ["glyphMargin"] = false,
            ["lineDecorationsWidth"] = 8,
            ["lineNumbersMinChars"] = 2,
            ["overviewRulerLanes"] = 0,
        },
    };

    private static readonly IReadOnlyList<string> FormulaCompletionTriggerCharacters = ["{"];

    private static readonly IReadOnlyList<ReportFormulaFunctionOption> Functions =
    [
        new( "Additional Functions", "IsNull", "IsNull({0})", "Returns true when a value is null." ),
        new( "Additional Functions", "IsNullOrEmpty", "IsNullOrEmpty({0})", "Returns true when a text value is null or empty." ),
        new( "Additional Functions", "Coalesce", "Coalesce({0}, null)", "Returns the first non-null value." ),
        new( "Text", "Contains", "Contains({0}, \"\")", "Returns true when text contains the specified value." ),
        new( "Text", "StartsWith", "StartsWith({0}, \"\")", "Returns true when text starts with the specified value." ),
        new( "Text", "EndsWith", "EndsWith({0}, \"\")", "Returns true when text ends with the specified value." ),
        new( "Text", "Upper", "Upper({0})", "Converts text to uppercase." ),
        new( "Text", "Lower", "Lower({0})", "Converts text to lowercase." ),
        new( "Text", "Length", "Length({0})", "Returns the number of characters in text." ),
        new( "Math", "Round", $"Round({{0}}, {DefaultRoundDecimalPlaces})", "Rounds a number to the specified number of decimals." ),
        new( "Math", "Abs", "Abs({0})", "Returns the absolute value of a number." ),
        new( "Date and Time", "Today", "Today()", "Returns the current date." ),
        new( "Date and Time", "Now", "Now()", "Returns the current date and time." ),
        new( "Aggregates", "Count", "Count({0})", "Counts values in a data source." ),
        new( "Aggregates", "Sum", "Sum({0})", "Sums numeric values in a data source." ),
        new( "Aggregates", "Average", "Average({0})", "Calculates the average numeric value." ),
        new( "Aggregates", "Minimum", "Minimum({0})", "Returns the minimum value." ),
        new( "Aggregates", "Maximum", "Maximum({0})", "Returns the maximum value." ),
    ];

    private static readonly IReadOnlyList<ReportFormulaOperatorOption> Operators =
    [
        new( "Arithmetic", "Add", " + ", "Adds two values." ),
        new( "Arithmetic", "Subtract", " - ", "Subtracts one value from another." ),
        new( "Arithmetic", "Multiply", " * ", "Multiplies two values." ),
        new( "Arithmetic", "Divide", " / ", "Divides one value by another." ),
        new( "Arithmetic", "Modulo", " % ", "Returns the remainder after division." ),
        new( "Boolean", "And", " && ", "Returns true when both conditions are true." ),
        new( "Boolean", "Or", " || ", "Returns true when at least one condition is true." ),
        new( "Boolean", "Not", "!", "Negates a boolean value." ),
        new( "Boolean", "True", "true", "Boolean true literal." ),
        new( "Boolean", "False", "false", "Boolean false literal." ),
        new( "Boolean", "Null", "null", "Null literal." ),
        new( "Comparisons", "Equal", " == ", "Compares whether two values are equal." ),
        new( "Comparisons", "Not Equal", " != ", "Compares whether two values are different." ),
        new( "Comparisons", "Greater Than", " > ", "Compares whether the left value is greater." ),
        new( "Comparisons", "Greater Than Or Equal", " >= ", "Compares whether the left value is greater or equal." ),
        new( "Comparisons", "Less Than", " < ", "Compares whether the left value is smaller." ),
        new( "Comparisons", "Less Than Or Equal", " <= ", "Compares whether the left value is smaller or equal." ),
        new( "Control Structures", "If Then Else", "if  then true else false", "Chooses between two values based on a condition.",
            CaretIndex: 3,
            CompletionText: "if ${1:condition} then ${2:true} else ${3:false}",
            CompletionInsertTextRules: CodeEditorCompletionItemInsertTextRule.InsertAsSnippet ),
        new( "Control Structures", "Conditional", " ? : ", "Chooses between two values based on a condition." ),
    ];

    private Blazorise.CodeEditor.CodeEditor formulaEditor;

    private string formula;

    private CodeEditorCompletionProvider formulaCompletionProvider;

    private IReadOnlyList<CodeEditorCompletionItem> formulaFieldCompletionItems = [];

    private IReadOnlyList<CodeEditorDiagnostic> formulaDiagnostics = [];

    private string selectedFieldExpression;

    private string selectedHelpDescription;

    private string selectedHelpItem;

    private string title;

    private string validationMessage;

    private bool validationSucceeded;

    #endregion

    #region Methods

    internal async Task Show( string propertyName, string value )
    {
        await ShowReportModal<_ReportDesignerFormulaDialog>( parameters =>
        {
            parameters.Add( nameof( Definition ), Definition );
            parameters.Add( nameof( Data ), Data );
            parameters.Add( nameof( SourceDataSources ), SourceDataSources );
            parameters.Add( nameof( Section ), Section );
            parameters.Add( nameof( InitialPropertyName ), propertyName );
            parameters.Add( nameof( InitialValue ), value );
            parameters.Add( nameof( Confirmed ), Confirmed );
        }, CreateReportModalOptions( ModalSize.Large ) );
    }

    private async Task Clear()
    {
        ClearValidation();
        formulaDiagnostics = [];

        if ( formulaEditor is not null )
        {
            await formulaEditor.SetValueAsync( string.Empty );
            await formulaEditor.Focus();
        }
        else
        {
            formula = null;
        }
    }

    private Task Close()
    {
        return CloseReportModal();
    }

    private async Task Check()
    {
        await SynchronizeFormula();
        ValidateFormula();
    }

    private async Task Save()
    {
        await SaveFormula();
    }

    private async Task SaveAndClose()
    {
        if ( await SaveFormula() )
            await CloseReportModal();
    }

    private async Task<bool> SaveFormula()
    {
        await SynchronizeFormula();

        if ( !ValidateFormula() )
            return false;

        await Confirmed.InvokeAsync( string.IsNullOrWhiteSpace( formula ) ? null : formula.Trim() );
        validationSucceeded = true;
        validationMessage = "Formula saved.";

        return true;
    }

    private bool ValidateFormula()
    {
        ReportFormulaValidationResult result = ValidateFormulaValue();

        validationSucceeded = result.Success;
        validationMessage = result.Message;
        UpdateFormulaDiagnostics( result );

        return result.Success;
    }

    private async Task SynchronizeFormula()
    {
        if ( formulaEditor is not null )
            formula = await formulaEditor.GetValueAsync();
    }

    private ReportFormulaValidationResult ValidateFormulaValue()
    {
        return ReportFormulaEvaluator.Validate( formula, new()
        {
            Definition = Definition,
            Data = Data,
            ValidationDataSources = SourceDataSources,
            Section = Section,
        } );
    }

    private void UpdateFormulaDiagnostics( ReportFormulaValidationResult result )
    {
        if ( result.Success )
        {
            formulaDiagnostics = [];
            return;
        }

        GetFormulaPosition( formula, result.Position, out int lineNumber, out int column );
        GetFormulaPosition( formula, result.Position + Math.Max( 1, result.Length ), out int endLineNumber, out int endColumn );

        formulaDiagnostics =
        [
            new()
            {
                Severity = CodeEditorDiagnosticSeverity.Error,
                Code = "REPORT_FORMULA",
                Message = result.Message,
                StartLineNumber = lineNumber,
                StartColumn = column,
                EndLineNumber = endLineNumber,
                EndColumn = endColumn,
            },
        ];
    }

    private void ClearValidation()
    {
        validationMessage = null;
        validationSucceeded = false;
    }

    private IReadOnlyList<ReportTreeNode> BuildFieldNodes()
    {
        IEnumerable<ReportDesignerDataSourceNode> dataSources = SourceDataSources ?? ReportDataSourceExplorer.ResolveDataSourceDictionary( Definition, "Default" );

        return ReportDesignerTreeBuilder.BuildFieldsExplorerNodes( dataSources, Definition?.FormulaFields, Definition?.RunningTotals )
            .Select( CloneFormulaNode )
            .ToList();
    }

    private ReportTreeNode CloneFormulaNode( ReportTreeNode node )
    {
        bool selectable = node?.Value is ReportFieldTreeNodeValue;

        return new()
        {
            Key = $"formula:{node?.Key}",
            Text = node?.Text,
            Detail = node?.Detail,
            Kind = node?.Kind ?? ReportTreeNodeKind.Folder,
            Selectable = selectable,
            Draggable = false,
            Value = node?.Value,
            Children = node?.Children?.Select( CloneFormulaNode ).ToList() ?? [],
        };
    }

    private async Task OnFieldNodeClicked( ReportTreeNode node )
    {
        if ( node?.Value is ReportFieldTreeNodeValue field )
        {
            selectedFieldExpression = ReportExpressionFormatter.FormatFieldExpression( Definition, field.DataSourceName, field.FieldName );
            selectedHelpItem = selectedFieldExpression;
            selectedHelpDescription = "Report field value.";
            await InsertText( selectedFieldExpression );
        }
    }

    private async Task OnFunctionNodeClicked( ReportTreeNode node )
    {
        if ( node?.Value is ReportFormulaFunctionOption function )
            await InsertFunction( function );
    }

    private async Task OnOperatorNodeClicked( ReportTreeNode node )
    {
        if ( node?.Value is ReportFormulaOperatorOption operatorOption )
            await InsertOperator( operatorOption );
    }

    private Task InsertFunction( ReportFormulaFunctionOption function )
    {
        string fieldExpression = string.IsNullOrWhiteSpace( selectedFieldExpression ) ? "{Field}" : selectedFieldExpression;

        selectedHelpItem = function.Name;
        selectedHelpDescription = function.Description;

        return InsertText( function.Template.Replace( "{0}", fieldExpression, StringComparison.Ordinal ) );
    }

    private Task InsertOperator( ReportFormulaOperatorOption operatorOption )
    {
        selectedHelpItem = operatorOption.Name;
        selectedHelpDescription = operatorOption.Description;

        return InsertText( operatorOption.Text, operatorOption.CaretIndex );
    }

    private async Task InsertText( string text, int? caretIndex = null )
    {
        if ( string.IsNullOrWhiteSpace( text ) )
            return;

        if ( formulaEditor is null )
        {
            formula = string.IsNullOrWhiteSpace( formula )
                ? text
                : $"{formula}{text}";

            formulaDiagnostics = [];
            return;
        }

        string editorValue = await formulaEditor.GetValueAsync() ?? string.Empty;
        CodeEditorSelection selection = await formulaEditor.GetSelectionAsync() ?? CreateEndSelection( editorValue );
        int startOffset = GetFormulaOffset( editorValue, selection.StartLineNumber, selection.StartColumn );
        int endOffset = GetFormulaOffset( editorValue, selection.EndLineNumber, selection.EndColumn );
        string updatedValue = editorValue[..startOffset] + text + editorValue[endOffset..];
        int relativeCaretOffset = Math.Min( Math.Max( 0, caretIndex ?? text.Length ), text.Length );
        int caretOffset = startOffset + relativeCaretOffset;

        await formulaEditor.SetValueAsync( updatedValue );

        GetFormulaPosition( updatedValue, caretOffset, out int lineNumber, out int column );
        await formulaEditor.SetSelectionAsync( new()
        {
            StartLineNumber = lineNumber,
            StartColumn = column,
            EndLineNumber = lineNumber,
            EndColumn = column,
        } );
        await formulaEditor.Focus();
    }

    private Task OnFormulaChanged( string value )
    {
        formula = value;
        ClearValidation();
        formulaDiagnostics = [];

        return Task.CompletedTask;
    }

    protected override void OnInitialized()
    {
        title = string.IsNullOrWhiteSpace( InitialPropertyName )
            ? "Edit formula"
            : $"Edit {InitialPropertyName} formula";
        formula = InitialValue;
        selectedFieldExpression = null;
        selectedHelpItem = null;
        selectedHelpDescription = null;
        formulaFieldCompletionItems = CreateFormulaFieldCompletionItems();
        formulaCompletionProvider = new()
        {
            Language = FormulaLanguageId,
            TriggerCharacters = FormulaCompletionTriggerCharacters,
            Items = CreateFormulaSyntaxCompletionItems(),
            ItemsProvider = ProvideFormulaCompletionItems,
        };
        ClearValidation();
        formulaDiagnostics = [];
    }

    private static IReadOnlyList<CodeEditorCompletionItem> CreateFormulaSyntaxCompletionItems()
    {
        List<CodeEditorCompletionItem> completionItems = Functions
            .Select( function => new CodeEditorCompletionItem
            {
                Label = function.Name,
                InsertText = function.Template.Replace( "{0}", "{Field}", StringComparison.Ordinal ),
                Kind = CodeEditorCompletionItemKind.Function,
                Detail = function.Category,
                Documentation = function.Description,
                FilterText = function.Name,
            } )
            .ToList();

        completionItems.AddRange( Operators.Select( operatorOption => new CodeEditorCompletionItem
        {
            Label = operatorOption.Name,
            InsertText = operatorOption.CompletionText ?? operatorOption.Text,
            Kind = IsFormulaKeyword( operatorOption.Text )
                ? CodeEditorCompletionItemKind.Keyword
                : CodeEditorCompletionItemKind.Operator,
            Detail = operatorOption.Category,
            Documentation = operatorOption.Description,
            FilterText = operatorOption.Name,
            InsertTextRules = operatorOption.CompletionInsertTextRules,
        } ) );

        return completionItems;
    }

    private IReadOnlyList<CodeEditorCompletionItem> CreateFormulaFieldCompletionItems()
    {
        List<CodeEditorCompletionItem> completionItems = [];

        HashSet<string> fieldExpressions = new( StringComparer.OrdinalIgnoreCase );

        foreach ( ReportTreeNode fieldNode in EnumerateFormulaNodes( FieldNodes ) )
        {
            if ( fieldNode?.Value is not ReportFieldTreeNodeValue field )
                continue;

            string expression = ReportExpressionFormatter.FormatFieldExpression( Definition, field.DataSourceName, field.FieldName );

            if ( string.IsNullOrWhiteSpace( expression ) || !fieldExpressions.Add( expression ) )
                continue;

            completionItems.Add( new()
            {
                Label = expression,
                InsertText = expression,
                Kind = CodeEditorCompletionItemKind.Field,
                Detail = fieldNode.Detail ?? "Report field",
                Documentation = $"Inserts the {expression} report field.",
                FilterText = expression,
                SortText = $"0_{expression}",
            } );
        }

        return completionItems;
    }

    private Task<IReadOnlyList<CodeEditorCompletionItem>> ProvideFormulaCompletionItems( CodeEditorCompletionContext context )
    {
        CodeEditorCompletionRange range = GetFormulaCompletionRange( context );

        if ( range is null )
            return Task.FromResult<IReadOnlyList<CodeEditorCompletionItem>>( [] );

        IReadOnlyList<CodeEditorCompletionItem> completionItems = formulaFieldCompletionItems
            .Select( item => new CodeEditorCompletionItem
            {
                Label = item.Label,
                InsertText = item.InsertText,
                Kind = item.Kind,
                Detail = item.Detail,
                Documentation = item.Documentation,
                FilterText = item.FilterText,
                SortText = item.SortText,
                CommitCharacters = item.CommitCharacters,
                InsertTextRules = item.InsertTextRules,
                Range = range,
            } )
            .ToArray();

        return Task.FromResult( completionItems );
    }

    private static CodeEditorCompletionRange GetFormulaCompletionRange( CodeEditorCompletionContext context )
    {
        if ( context is null || context.LineNumber < 1 || context.Column < 1 )
            return null;

        string lineText = context.LineText ?? string.Empty;
        int cursorIndex = Math.Min( context.Column - 1, lineText.Length );
        string textBeforeCursor = lineText[..cursorIndex];
        int openingIndex = textBeforeCursor.LastIndexOf( '{' );
        int closingIndex = textBeforeCursor.LastIndexOf( '}' );

        if ( openingIndex <= closingIndex )
            return null;

        int endColumn = context.Column;

        if ( cursorIndex < lineText.Length && lineText[cursorIndex] == '}' )
            endColumn++;

        return new()
        {
            StartLineNumber = context.LineNumber,
            StartColumn = openingIndex + 1,
            EndLineNumber = context.LineNumber,
            EndColumn = endColumn,
        };
    }

    private static IEnumerable<ReportTreeNode> EnumerateFormulaNodes( IEnumerable<ReportTreeNode> nodes )
    {
        foreach ( ReportTreeNode node in nodes ?? [] )
        {
            yield return node;

            foreach ( ReportTreeNode child in EnumerateFormulaNodes( node?.Children ) )
                yield return child;
        }
    }

    private static bool IsFormulaKeyword( string value )
    {
        string normalizedValue = value?.Trim();

        return string.Equals( normalizedValue, "true", StringComparison.OrdinalIgnoreCase )
            || string.Equals( normalizedValue, "false", StringComparison.OrdinalIgnoreCase )
            || string.Equals( normalizedValue, "null", StringComparison.OrdinalIgnoreCase )
            || normalizedValue?.StartsWith( "if ", StringComparison.OrdinalIgnoreCase ) == true;
    }

    private static CodeEditorSelection CreateEndSelection( string value )
    {
        GetFormulaPosition( value, value?.Length ?? 0, out int lineNumber, out int column );

        return new()
        {
            StartLineNumber = lineNumber,
            StartColumn = column,
            EndLineNumber = lineNumber,
            EndColumn = column,
        };
    }

    private static int GetFormulaOffset( string value, int lineNumber, int column )
    {
        if ( string.IsNullOrEmpty( value ) )
            return 0;

        int currentLine = 1;
        int currentColumn = 1;

        for ( int index = 0; index < value.Length; index++ )
        {
            if ( currentLine == Math.Max( 1, lineNumber ) && currentColumn >= Math.Max( 1, column ) )
                return index;

            if ( value[index] == '\n' )
            {
                currentLine++;
                currentColumn = 1;
            }
            else if ( value[index] != '\r' )
            {
                currentColumn++;
            }
        }

        return value.Length;
    }

    private static void GetFormulaPosition( string value, int offset, out int lineNumber, out int column )
    {
        lineNumber = 1;
        column = 1;

        if ( string.IsNullOrEmpty( value ) )
            return;

        int maximumOffset = Math.Min( Math.Max( 0, offset ), value.Length );

        for ( int index = 0; index < maximumOffset; index++ )
        {
            if ( value[index] == '\n' )
            {
                lineNumber++;
                column = 1;
            }
            else if ( value[index] != '\r' )
            {
                column++;
            }
        }
    }

    private static IReadOnlyList<ReportTreeNode> BuildFormulaNodes( string prefix, ReportTreeNodeKind kind, IEnumerable<IReportFormulaTreeOption> options )
    {
        return options
            .GroupBy( option => option.Category )
            .Select( group => new ReportTreeNode
            {
                Key = $"{prefix}:category:{group.Key}",
                Text = group.Key,
                Kind = ReportTreeNodeKind.Folder,
                Selectable = false,
                Children = group.Select( option => new ReportTreeNode
                {
                    Key = $"{prefix}:item:{group.Key}:{option.Name}",
                    Text = option.Name,
                    Kind = kind,
                    Selectable = true,
                    Value = option,
                } ).ToList(),
            } )
            .ToList();
    }

    #endregion

    #region Properties

    private string Title => title ?? "Edit formula";

    private IReadOnlyList<ReportTreeNode> FieldNodes => BuildFieldNodes();

    private IReadOnlyList<ReportTreeNode> FunctionNodes => BuildFormulaNodes( "function", ReportTreeNodeKind.Function, Functions );

    private IReadOnlyList<ReportTreeNode> OperatorNodes => BuildFormulaNodes( "operator", ReportTreeNodeKind.Operator, Operators );

    private string SelectedHelpItem => selectedHelpItem ?? "Formula";

    private string SelectedHelpDescription => selectedHelpDescription ?? "Select a field, function, or operator to insert it into the formula.";

    private bool HasValidationMessage => !string.IsNullOrWhiteSpace( validationMessage );

    private Color ValidationAlertColor => validationSucceeded ? Color.Success : Color.Danger;

    [Parameter] public string InitialPropertyName { get; set; }

    [Parameter] public string InitialValue { get; set; }

    /// <summary>
    /// Report definition used to discover fields.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Report data used while validating formula expressions.
    /// </summary>
    [Parameter] public object Data { get; set; }

    /// <summary>
    /// Optional pre-resolved source fields used by scoped designers such as subreports.
    /// </summary>
    [Parameter] public IReadOnlyList<ReportDesignerDataSourceNode> SourceDataSources { get; set; }

    /// <summary>
    /// Report band used while validating formula expressions.
    /// </summary>
    [Parameter] public ReportBandDefinition Section { get; set; }

    /// <summary>
    /// Raised when the formula expression is confirmed.
    /// </summary>
    [Parameter] public EventCallback<string> Confirmed { get; set; }

    #endregion

    #region Nested types

    private interface IReportFormulaTreeOption
    {
        string Category { get; }

        string Name { get; }

        string Description { get; }
    }

    private sealed record ReportFormulaFunctionOption( string Category, string Name, string Template, string Description ) : IReportFormulaTreeOption;

    private sealed record ReportFormulaOperatorOption(
        string Category,
        string Name,
        string Text,
        string Description,
        int? CaretIndex = null,
        string CompletionText = null,
        CodeEditorCompletionItemInsertTextRule CompletionInsertTextRules = CodeEditorCompletionItemInsertTextRule.None ) : IReportFormulaTreeOption;

    #endregion
}