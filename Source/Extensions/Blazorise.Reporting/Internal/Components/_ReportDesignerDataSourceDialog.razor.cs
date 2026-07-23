#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Internal dialog used to select the data source bound to a report band.
/// </summary>
public partial class _ReportDesignerDataSourceDialog
{
    #region Members

    private const string NoDataSourceValue = "";

    private readonly List<ReportDesignerDataSourceOption> dataSourceOptions = [];

    private string selectedDataSource;

    #endregion

    #region Methods

    internal async Task Show( string dataSource )
    {
        await ShowReportModal<_ReportDesignerDataSourceDialog>( parameters =>
        {
            parameters.Add( nameof( Definition ), Definition );
            parameters.Add( nameof( InitialDataSource ), dataSource );
            parameters.Add( nameof( Confirmed ), Confirmed );
        } );
    }

    private Task Close()
    {
        return CloseReportModal();
    }

    private async Task Confirm()
    {
        await Confirmed.InvokeAsync( string.IsNullOrWhiteSpace( selectedDataSource ) ? null : selectedDataSource );
        await CloseReportModal();
    }

    private Task OnSelectedDataSourceChanged( string value )
    {
        selectedDataSource = value;

        return Task.CompletedTask;
    }

    private string ResolveInitialDataSource( string dataSource )
    {
        if ( string.IsNullOrWhiteSpace( dataSource ) )
            return NoDataSourceValue;

        ReportDesignerDataSourceOption resolvedDataSource = dataSourceOptions.FirstOrDefault( source =>
            string.Equals( source.Value, dataSource, StringComparison.OrdinalIgnoreCase )
            || string.Equals( source.DisplayName, dataSource, StringComparison.OrdinalIgnoreCase ) );

        return resolvedDataSource?.Value ?? NoDataSourceValue;
    }

    protected override void OnInitialized()
    {
        dataSourceOptions.Clear();
        dataSourceOptions.AddRange( ReportDataSourceExplorer.ResolveBindableDataSources( Definition ) );

        selectedDataSource = ResolveInitialDataSource( InitialDataSource );
    }

    #endregion

    #region Properties

    [Parameter] public string InitialDataSource { get; set; }

    /// <summary>
    /// Report definition that provides the bindable data sources.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Raised when the data source selection is confirmed.
    /// </summary>
    [Parameter] public EventCallback<string> Confirmed { get; set; }

    #endregion

}