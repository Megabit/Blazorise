#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Internal dialog used to configure running total fields.
/// </summary>
public partial class _ReportDesignerRunningTotalDialog
{
    #region Members

    private const string FieldKeySeparator = "\u001f";

    private readonly List<ReportDesignerFieldOption> fields = [];

    private readonly List<ReportAggregateFunction> supportedFunctions = [];

    private readonly List<ReportRunningTotalGroupOption> groupOptions = [];

    private ReportRunningTotalDefinition runningTotal = new();

    private string selectedFieldKey;

    #endregion

    #region Methods

    internal async Task Show( ReportRunningTotalDefinition definition )
    {
        await ShowReportModal<_ReportDesignerRunningTotalDialog>( parameters =>
        {
            parameters.Add( nameof( Definition ), Definition );
            parameters.Add( nameof( Data ), Data );
            parameters.Add( nameof( SourceDataSources ), SourceDataSources );
            parameters.Add( nameof( InitialDefinition ), definition );
            parameters.Add( nameof( Confirmed ), Confirmed );
        } );
    }

    private Task Close()
    {
        return CloseReportModal();
    }

    private async Task Confirm()
    {
        ApplySelectedField();

        if ( !CanConfirm )
            return;

        await Confirmed.InvokeAsync( CloneRunningTotal( runningTotal ) );
        await CloseReportModal();
    }

    private async Task OpenEvaluateFormulaDialog()
    {
        await ShowReportModal<_ReportDesignerFormulaDialog>( parameters =>
        {
            parameters.Add( nameof( _ReportDesignerFormulaDialog.Definition ), Definition );
            parameters.Add( nameof( _ReportDesignerFormulaDialog.Data ), Data );
            parameters.Add( nameof( _ReportDesignerFormulaDialog.SourceDataSources ), SourceDataSources );
            parameters.Add( nameof( _ReportDesignerFormulaDialog.Section ), SelectedSection );
            parameters.Add( nameof( _ReportDesignerFormulaDialog.InitialPropertyName ), "Evaluate formula" );
            parameters.Add( nameof( _ReportDesignerFormulaDialog.InitialValue ), runningTotal.EvaluateFormula );
            parameters.Add( nameof( _ReportDesignerFormulaDialog.Confirmed ), EventCallback.Factory.Create<string>( this, EvaluateFormulaConfirmed ) );
        }, CreateReportModalOptions( ModalSize.Large ) );
    }

    private Task EvaluateFormulaConfirmed( string formula )
    {
        runningTotal.EvaluateFormula = formula;

        return Task.CompletedTask;
    }

    private Task OnEvaluateFormulaChanged( string value )
    {
        runningTotal.EvaluateFormula = value;

        return Task.CompletedTask;
    }

    private Task OnEvaluateModeChanged( ReportRunningTotalEvaluateMode value )
    {
        runningTotal.EvaluateMode = value;

        return Task.CompletedTask;
    }

    private Task OnAggregateFunctionChanged( ReportAggregateFunction value )
    {
        runningTotal.AggregateFunction = value;

        return Task.CompletedTask;
    }

    private Task OnNameChanged( string value )
    {
        runningTotal.Name = value;

        return Task.CompletedTask;
    }

    private Task OnResetGroupChanged( string value )
    {
        runningTotal.ResetGroupId = value;

        return Task.CompletedTask;
    }

    private Task OnResetModeChanged( ReportRunningTotalResetMode value )
    {
        runningTotal.ResetMode = value;
        EnsureResetGroup();

        return Task.CompletedTask;
    }

    private Task OnSelectedFieldChanged( string value )
    {
        selectedFieldKey = value;
        ApplySelectedField();
        RefreshSupportedFunctions();

        return Task.CompletedTask;
    }

    private void ApplySelectedField()
    {
        ReportDesignerFieldOption field = FindSelectedField();

        if ( field is null )
            return;

        runningTotal.DataSource = field.DataSourceName;
        runningTotal.Field = field.FieldName;
    }

    private static ReportRunningTotalDefinition CloneRunningTotal( ReportRunningTotalDefinition value )
    {
        return new()
        {
            Id = string.IsNullOrWhiteSpace( value.Id ) ? Guid.NewGuid().ToString( "N" ) : value.Id,
            Name = value.Name,
            DataSource = value.DataSource,
            Field = value.Field,
            AggregateFunction = value.AggregateFunction,
            EvaluateMode = value.EvaluateMode,
            EvaluateFormula = value.EvaluateFormula,
            ResetMode = value.ResetMode,
            ResetGroupId = value.ResetGroupId,
        };
    }

    private static string CreateFieldKey( ReportDesignerFieldOption field )
    {
        return $"{field.DataSourceName}{FieldKeySeparator}{field.FieldName}";
    }

    private ReportDesignerFieldOption FindSelectedField()
    {
        return fields.FirstOrDefault( field => string.Equals( CreateFieldKey( field ), selectedFieldKey, StringComparison.Ordinal ) );
    }

    private void EnsureResetGroup()
    {
        if ( runningTotal.ResetMode != ReportRunningTotalResetMode.Group )
            return;

        if ( string.IsNullOrWhiteSpace( runningTotal.ResetGroupId )
            || groupOptions.All( group => !string.Equals( group.Id, runningTotal.ResetGroupId, StringComparison.Ordinal ) ) )
        {
            runningTotal.ResetGroupId = groupOptions.FirstOrDefault()?.Id;
        }
    }

    private void RefreshFields()
    {
        fields.Clear();

        int sectionIndex = 0;

        foreach ( ReportDesignerDataSourceNode dataSource in SourceDataSources ?? ReportDataSourceExplorer.ResolveDataSourceDictionary( Definition, "Default" ) )
        {
            fields.AddRange( FlattenFieldOptions( Definition, sectionIndex, dataSource.BindingName, dataSource.Fields ) );
            sectionIndex++;
        }
    }

    private void RefreshGroupOptions()
    {
        groupOptions.Clear();

        if ( Definition?.Bands is null )
            return;

        groupOptions.AddRange( Definition.Bands
            .Where( section => section.Type == ReportBandType.GroupHeader )
            .Select( section => new ReportRunningTotalGroupOption
            {
                Id = section.Id,
                Name = ReportDefinitionHelper.GetSectionDisplayName( section ),
            } ) );
    }

    private void RefreshSupportedFunctions()
    {
        supportedFunctions.Clear();

        ReportDesignerFieldOption field = FindSelectedField();

        if ( field is null )
            return;

        supportedFunctions.AddRange( ReportAggregateResolver.GetSupportedFunctions( Definition, Data, field.DataSourceName, field.FieldName, field.DataType ) );

        if ( !supportedFunctions.Contains( runningTotal.AggregateFunction ) )
            runningTotal.AggregateFunction = supportedFunctions.Contains( ReportAggregateFunction.Sum ) ? ReportAggregateFunction.Sum : supportedFunctions.FirstOrDefault();
    }

    private string ResolveInitialFieldKey()
    {
        ReportDesignerFieldOption selectedField = fields.FirstOrDefault( field =>
            string.Equals( field.DataSourceName, runningTotal.DataSource, StringComparison.OrdinalIgnoreCase )
            && string.Equals( field.FieldName, runningTotal.Field, StringComparison.OrdinalIgnoreCase ) )
            ?? fields.FirstOrDefault();

        return selectedField is null ? null : CreateFieldKey( selectedField );
    }

    private static IEnumerable<ReportDesignerFieldOption> FlattenFieldOptions( ReportDefinition definition, int sourceSectionIndex, string dataSourceName, IEnumerable<ReportDesignerFieldNode> nodes, string collectionPath = null )
    {
        foreach ( ReportDesignerFieldNode node in nodes ?? [] )
        {
            if ( node.IsCollection )
            {
                string collectionDataSourceName = ReportExpressionFormatter.FormatFieldPath( dataSourceName, node.Path );

                foreach ( ReportDesignerFieldOption child in FlattenFieldOptions( definition, sourceSectionIndex, collectionDataSourceName, node.Children, node.Path ) )
                {
                    yield return child;
                }

                continue;
            }

            if ( node.Children.Count == 0 )
            {
                string fieldName = ResolveFieldName( node, collectionPath );

                yield return new()
                {
                    SourceSectionIndex = sourceSectionIndex,
                    DataSourceName = dataSourceName,
                    FieldName = fieldName,
                    DisplayName = FormatFieldOptionDisplayName( definition, dataSourceName, fieldName, node.DataType ),
                    DataType = node.DataType,
                };

                continue;
            }

            foreach ( ReportDesignerFieldOption child in FlattenFieldOptions( definition, sourceSectionIndex, dataSourceName, node.Children, collectionPath ) )
            {
                yield return child;
            }
        }
    }

    private static string FormatFieldOptionDisplayName( ReportDefinition definition, string dataSourceName, string fieldName, Type dataType )
    {
        string fieldPath = ReportExpressionFormatter.FormatFieldPath( definition, dataSourceName, fieldName );
        string dataTypeName = ReportDefinitionHelper.GetDataTypeDisplayName( dataType );

        return string.IsNullOrWhiteSpace( dataTypeName )
            ? fieldPath
            : $"{fieldPath} ({dataTypeName})";
    }

    private static string ResolveFieldName( ReportDesignerFieldNode node, string collectionPath )
    {
        if ( string.IsNullOrWhiteSpace( collectionPath ) || string.IsNullOrWhiteSpace( node?.Path ) )
            return node?.Path;

        string prefix = $"{collectionPath.Trim()}.";

        return node.Path.StartsWith( prefix, StringComparison.OrdinalIgnoreCase )
            ? node.Path[prefix.Length..]
            : node.Path;
    }

    protected override void OnInitialized()
    {
        runningTotal = CloneRunningTotal( InitialDefinition ?? new() );
        RefreshFields();
        RefreshGroupOptions();

        selectedFieldKey = ResolveInitialFieldKey();
        ApplySelectedField();
        RefreshSupportedFunctions();
        EnsureResetGroup();
    }

    #endregion

    #region Properties

    private bool CanConfirm => !string.IsNullOrWhiteSpace( runningTotal.Name )
        && FindSelectedField() is not null
        && supportedFunctions.Count > 0
        && ( runningTotal.ResetMode != ReportRunningTotalResetMode.Group || !string.IsNullOrWhiteSpace( runningTotal.ResetGroupId ) );

    private ReportBandDefinition SelectedSection => Definition?.Bands?.FirstOrDefault( section =>
        string.Equals( section.DataSource, runningTotal.DataSource, StringComparison.OrdinalIgnoreCase ) );

    [Parameter] public ReportRunningTotalDefinition InitialDefinition { get; set; }

    /// <summary>
    /// Report definition used to resolve fields and groups.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Report data used to infer field types and validate formulas.
    /// </summary>
    [Parameter] public object Data { get; set; }

    /// <summary>
    /// Optional pre-resolved source fields used by scoped designers such as subreports.
    /// </summary>
    [Parameter] public IReadOnlyList<ReportDesignerDataSourceNode> SourceDataSources { get; set; }

    /// <summary>
    /// Raised when the running total configuration is confirmed.
    /// </summary>
    [Parameter] public EventCallback<ReportRunningTotalDefinition> Confirmed { get; set; }

    #endregion

    #region Nested types

    private sealed class ReportRunningTotalGroupOption
    {
        internal string Id { get; set; }

        internal string Name { get; set; }
    }

    #endregion
}