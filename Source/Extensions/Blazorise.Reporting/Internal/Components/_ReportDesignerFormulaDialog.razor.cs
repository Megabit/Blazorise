#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    private const int FormulaEditorRows = 8;

    private const int FormulaPaneHeightInPixels = 180;

    private static readonly IReadOnlyList<ReportFormulaFunctionOption> Functions =
    [
        new( "Additional Functions", "IsNull", "IsNull({0})", "Returns true when a value is null." ),
        new( "Additional Functions", "IsNullOrEmpty", "IsNullOrEmpty({0})", "Returns true when a text value is null or empty." ),
        new( "Additional Functions", "Coalesce", "Coalesce({0}, {1})", "Returns the first non-null value." ),
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
        new( "Boolean", "And", " && ", "Returns true when both conditions are true." ),
        new( "Boolean", "Or", " || ", "Returns true when at least one condition is true." ),
        new( "Boolean", "True", "true", "Boolean true literal." ),
        new( "Boolean", "False", "false", "Boolean false literal." ),
        new( "Boolean", "Null", "null", "Null literal." ),
        new( "Comparisons", "Equal", " == ", "Compares whether two values are equal." ),
        new( "Comparisons", "Not Equal", " != ", "Compares whether two values are different." ),
        new( "Comparisons", "Greater Than", " > ", "Compares whether the left value is greater." ),
        new( "Comparisons", "Greater Than Or Equal", " >= ", "Compares whether the left value is greater or equal." ),
        new( "Comparisons", "Less Than", " < ", "Compares whether the left value is smaller." ),
        new( "Comparisons", "Less Than Or Equal", " <= ", "Compares whether the left value is smaller or equal." ),
        new( "Control Structures", "If Then Else", "if {0} then true else false", "Chooses between two values based on a condition." ),
        new( "Control Structures", "Conditional", " ? : ", "Chooses between two values based on a condition." ),
    ];

    private static readonly IFluentSizing FormulaPaneHeight = Blazorise.Height.Px( FormulaPaneHeightInPixels );

    private Modal modalRef;

    private string formula;

    private string selectedFieldExpression;

    private string selectedHelpDescription;

    private string selectedHelpItem;

    private string title;

    private string validationMessage;

    private bool validationSucceeded;

    #endregion

    #region Methods

    internal async Task ShowAsync( string propertyName, string value )
    {
        title = string.IsNullOrWhiteSpace( propertyName )
            ? "Edit formula"
            : $"Edit {propertyName} formula";
        formula = value;
        selectedFieldExpression = null;
        selectedHelpItem = null;
        selectedHelpDescription = null;
        ClearValidation();

        await modalRef.Show();
    }

    private Task ClearAsync()
    {
        formula = null;
        ClearValidation();

        return Task.CompletedTask;
    }

    private Task CloseAsync()
    {
        return modalRef.Hide();
    }

    private Task CheckAsync()
    {
        ValidateFormula();

        return Task.CompletedTask;
    }

    private async Task SaveAsync()
    {
        await SaveFormulaAsync();
    }

    private async Task SaveAndCloseAsync()
    {
        if ( await SaveFormulaAsync() )
            await modalRef.Hide();
    }

    private async Task<bool> SaveFormulaAsync()
    {
        if ( !ValidateFormula() )
            return false;

        await Confirmed.InvokeAsync( string.IsNullOrWhiteSpace( formula ) ? null : formula.Trim() );
        validationSucceeded = true;
        validationMessage = "Formula saved.";

        return true;
    }

    private bool ValidateFormula()
    {
        ReportFormulaValidationResult result = ReportFormulaEvaluator.Validate( formula, new()
        {
            Definition = Definition,
            Data = Data,
            Section = Section,
        } );

        validationSucceeded = result.Success;
        validationMessage = result.Message;

        return result.Success;
    }

    private void ClearValidation()
    {
        validationMessage = null;
        validationSucceeded = false;
    }

    private IReadOnlyList<ReportTreeNode> BuildFieldNodes()
    {
        IEnumerable<ReportDesignerDataSourceNode> dataSources = ReportDataSourceExplorer.ResolveDataSourceDictionary( Definition, "Default" );

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

    private Task OnFieldNodeClicked( ReportTreeNode node )
    {
        if ( node?.Value is ReportFieldTreeNodeValue field )
        {
            selectedFieldExpression = ReportExpressionFormatter.FormatFieldExpression( Definition, field.DataSourceName, field.FieldName );
            selectedHelpItem = selectedFieldExpression;
            selectedHelpDescription = "Report field value.";
            InsertText( selectedFieldExpression );
        }

        return Task.CompletedTask;
    }

    private Task OnFunctionNodeClicked( ReportTreeNode node )
    {
        if ( node?.Value is ReportFormulaFunctionOption function )
            InsertFunction( function );

        return Task.CompletedTask;
    }

    private Task OnOperatorNodeClicked( ReportTreeNode node )
    {
        if ( node?.Value is ReportFormulaOperatorOption operatorOption )
            InsertOperator( operatorOption );

        return Task.CompletedTask;
    }

    private void InsertFunction( ReportFormulaFunctionOption function )
    {
        string fieldExpression = string.IsNullOrWhiteSpace( selectedFieldExpression ) ? "{Field}" : selectedFieldExpression;

        selectedHelpItem = function.Name;
        selectedHelpDescription = function.Description;

        InsertText( function.Template.Replace( "{0}", fieldExpression, StringComparison.Ordinal ) );
    }

    private void InsertOperator( ReportFormulaOperatorOption operatorOption )
    {
        selectedHelpItem = operatorOption.Name;
        selectedHelpDescription = operatorOption.Description;

        InsertText( operatorOption.Text );
    }

    private void InsertText( string text )
    {
        if ( string.IsNullOrWhiteSpace( text ) )
            return;

        formula = string.IsNullOrWhiteSpace( formula )
            ? text
            : $"{formula}{text}";
    }

    private Task OnFormulaChanged( string value )
    {
        formula = value;
        ClearValidation();

        return Task.CompletedTask;
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

    /// <summary>
    /// Report definition used to discover fields.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Report data used while validating formula expressions.
    /// </summary>
    [Parameter] public object Data { get; set; }

    /// <summary>
    /// Report band used while validating formula expressions.
    /// </summary>
    [Parameter] public ReportSectionDefinition Section { get; set; }

    /// <summary>
    /// Raised when the formula expression is confirmed.
    /// </summary>
    [Parameter] public EventCallback<string> Confirmed { get; set; }

    #endregion

    #region Records

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

    private interface IReportFormulaTreeOption
    {
        string Category { get; }

        string Name { get; }

        string Description { get; }
    }

    private sealed record ReportFormulaFunctionOption( string Category, string Name, string Template, string Description ) : IReportFormulaTreeOption;

    private sealed record ReportFormulaOperatorOption( string Category, string Name, string Text, string Description ) : IReportFormulaTreeOption;

    #endregion
}