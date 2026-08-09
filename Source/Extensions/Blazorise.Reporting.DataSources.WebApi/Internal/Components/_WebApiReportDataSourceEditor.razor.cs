#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Reporting;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.DataSources.WebApi;

/// <summary>
/// Designer editor for Web API report data source settings.
/// </summary>
public partial class _WebApiReportDataSourceEditor
{
    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if ( !ResponseFormatOptions.Any( option => string.Equals( option.Value, ResponseFormat, StringComparison.OrdinalIgnoreCase ) ) )
            Context?.SetValue( WebApiReportDataSourceSettings.ResponseFormat, WebApiReportDataSourceFormats.Auto );
    }

    private Task OnUrlChanged( string value )
    {
        Context?.SetValue( WebApiReportDataSourceSettings.Url, value );

        return Task.CompletedTask;
    }

    private Task OnHeadersChanged( string value )
    {
        Context?.SetValue( WebApiReportDataSourceSettings.Headers, value );

        return Task.CompletedTask;
    }

    private Task OnResponseFormatChanged( string value )
    {
        Context?.SetValue( WebApiReportDataSourceSettings.ResponseFormat, value );

        return Task.CompletedTask;
    }

    private Task OnDataSelectorChanged( string value )
    {
        Context?.SetValue( WebApiReportDataSourceSettings.DataSelector, value );

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provider settings context edited by the Web API data source editor.
    /// </summary>
    [Parameter] public ReportDataSourceProviderEditorContext Context { get; set; }

    [Inject] private IEnumerable<IReportWebApiResponseReader> ResponseReaders { get; set; }

    private IReadOnlyList<WebApiReportDataSourceEditorOption> ResponseFormatOptions
    {
        get
        {
            List<WebApiReportDataSourceEditorOption> options =
            [
                new( WebApiReportDataSourceFormats.Auto, Localize( "Auto-detect" ) ),
            ];

            options.AddRange( ( ResponseReaders ?? [] )
                .Where( reader => reader is not null && !string.IsNullOrWhiteSpace( reader.Format ) )
                .OrderBy( reader => string.IsNullOrWhiteSpace( reader.DisplayName ) ? reader.Format : reader.DisplayName, StringComparer.OrdinalIgnoreCase )
                .Select( reader => new WebApiReportDataSourceEditorOption( reader.Format, string.IsNullOrWhiteSpace( reader.DisplayName ) ? reader.Format : reader.DisplayName ) ) );

            return options;
        }
    }

    private string Url => Context?.GetString( WebApiReportDataSourceSettings.Url );

    private string Headers => Context?.GetString( WebApiReportDataSourceSettings.Headers );

    private string ResponseFormat => Context?.GetString( WebApiReportDataSourceSettings.ResponseFormat ) ?? WebApiReportDataSourceFormats.Auto;

    private string DataSelector => Context?.GetString( WebApiReportDataSourceSettings.DataSelector );

    private string DataSelectorPlaceholder => string.Equals( ResponseFormat, WebApiReportDataSourceFormats.Xml, StringComparison.OrdinalIgnoreCase )
        ? "/orders/order"
        : "/items";

    #endregion
}