#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Internal dialog used to configure aggregate insertion from a detail field.
/// </summary>
public partial class _ReportDesignerAggregateDialog
{
    #region Members

    private readonly List<ReportDesignerFieldOption> fields = [];

    private readonly List<ReportAggregateFunction> supportedFunctions = [];

    private readonly List<ReportAggregateSummaryLocation> summaryLocations = [];

    private Modal modalRef;

    private string selectedFieldKey;

    private ReportAggregateFunction selectedFunction;

    private int selectedSummaryLocationIndex;

    #endregion

    #region Methods

    internal async Task ShowAsync( IEnumerable<ReportDesignerFieldOption> fieldOptions, string selectedFieldName, IEnumerable<ReportAggregateSummaryLocation> summaryLocationOptions )
    {
        fields.Clear();
        fields.AddRange( fieldOptions ?? [] );

        summaryLocations.Clear();
        summaryLocations.AddRange( summaryLocationOptions ?? [] );

        if ( summaryLocations.Count == 0 )
        {
            summaryLocations.Add( new()
            {
                TargetSectionIndex = -1,
                Name = "Grand Total (Report Footer)",
            } );
        }

        selectedFieldKey = ResolveInitialFieldKey( selectedFieldName );
        selectedFunction = ReportAggregateFunction.Sum;
        selectedSummaryLocationIndex = summaryLocations[0].TargetSectionIndex;
        RefreshSupportedFunctions();

        await modalRef.Show();
    }

    private Task CloseAsync()
    {
        return modalRef.Hide();
    }

    private async Task ConfirmAsync()
    {
        var selectedField = FindSelectedField();

        if ( selectedField is null || supportedFunctions.Count == 0 )
            return;

        await Confirmed.InvokeAsync( new()
        {
            SourceSectionIndex = selectedField.SourceSectionIndex,
            TargetSectionIndex = selectedSummaryLocationIndex,
            DataSourceName = selectedField.DataSourceName,
            FieldName = selectedField.FieldName,
            Function = selectedFunction,
        } );

        await modalRef.Hide();
    }

    private static string CreateFieldKey( ReportDesignerFieldOption field )
    {
        return $"{field.DataSourceName}\u001f{field.FieldName}";
    }

    private ReportDesignerFieldOption FindSelectedField()
    {
        return fields.FirstOrDefault( field => string.Equals( CreateFieldKey( field ), selectedFieldKey, StringComparison.Ordinal ) );
    }

    private Task OnSelectedFieldChanged( string value )
    {
        selectedFieldKey = value;
        RefreshSupportedFunctions();

        return Task.CompletedTask;
    }

    private Task OnSelectedSummaryLocationChanged( int value )
    {
        selectedSummaryLocationIndex = value;

        return Task.CompletedTask;
    }

    private Task OnSelectedFunctionChanged( ReportAggregateFunction value )
    {
        selectedFunction = value;

        return Task.CompletedTask;
    }

    private void RefreshSupportedFunctions()
    {
        supportedFunctions.Clear();

        var selectedField = FindSelectedField();

        if ( selectedField is null || ResolveSupportedFunctions is null )
            return;

        supportedFunctions.AddRange( ResolveSupportedFunctions.Invoke( selectedField ) ?? [] );

        if ( !supportedFunctions.Contains( selectedFunction ) )
            selectedFunction = supportedFunctions.FirstOrDefault();
    }

    private string ResolveInitialFieldKey( string selectedFieldName )
    {
        var selectedField = fields.FirstOrDefault( field => string.Equals( field.FieldName, selectedFieldName, StringComparison.OrdinalIgnoreCase ) )
            ?? fields.FirstOrDefault();

        return selectedField is null ? null : CreateFieldKey( selectedField );
    }

    #endregion

    #region Properties

    private bool CanConfirm => fields.Count > 0 && supportedFunctions.Count > 0 && FindSelectedField() is not null;

    /// <summary>
    /// Resolves aggregate functions supported by the selected field option.
    /// </summary>
    [Parameter] public Func<ReportDesignerFieldOption, IReadOnlyList<ReportAggregateFunction>> ResolveSupportedFunctions { get; set; }

    /// <summary>
    /// Raised when the aggregate configuration is confirmed.
    /// </summary>
    [Parameter] public EventCallback<ReportAggregateDialogResult> Confirmed { get; set; }

    #endregion
}