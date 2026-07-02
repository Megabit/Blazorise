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

    private Modal modalRef;

    private string selectedDataSource;

    #endregion

    #region Methods

    internal async Task ShowAsync( string dataSource )
    {
        dataSourceOptions.Clear();
        dataSourceOptions.AddRange( ReportDataSourceExplorer.ResolveBindableDataSources( Definition ) );

        selectedDataSource = ResolveInitialDataSource( dataSource );

        await modalRef.Show();
    }

    private Task CloseAsync()
    {
        return modalRef.Hide();
    }

    private async Task ConfirmAsync()
    {
        await Confirmed.InvokeAsync( string.IsNullOrWhiteSpace( selectedDataSource ) ? null : selectedDataSource );
        await modalRef.Hide();
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

    #endregion

    #region Properties

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