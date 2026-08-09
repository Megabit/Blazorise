#region Using directives
using System.Collections.Generic;
using Blazorise.Extensions;
using Blazorise.Reporting;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.DataSources.WebApi;

/// <summary>
/// Declares a Web API data source available to report bands and fields.
/// </summary>
public partial class ReportWebApiDataSource : BaseReportDataSourceComponent
{
    #region Members

    private string serializedHeaders;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override bool HasDefinitionChanged( ParameterView parameters )
    {
        return parameters.IsParameterChanged( Name )
            || parameters.IsParameterChanged( Url )
            || parameters.IsParameterChanged( Headers )
            || !string.Equals( serializedHeaders, WebApiReportDataSourceHeaders.Serialize( Headers ), System.StringComparison.Ordinal )
            || parameters.IsParameterChanged( ResponseFormat )
            || parameters.IsParameterChanged( DataSelector )
            || parameters.IsParameterChanged( Schema );
    }

    /// <inheritdoc />
    protected override ReportDataSourceDefinition CreateDataSourceDefinition()
    {
        Dictionary<string, object> settings = [];

        if ( Url is not null )
            settings[WebApiReportDataSourceSettings.Url] = Url;

        serializedHeaders = WebApiReportDataSourceHeaders.Serialize( Headers );

        if ( serializedHeaders is not null )
            settings[WebApiReportDataSourceSettings.Headers] = serializedHeaders;

        if ( ResponseFormat is not null )
            settings[WebApiReportDataSourceSettings.ResponseFormat] = ResponseFormat;

        if ( DataSelector is not null )
            settings[WebApiReportDataSourceSettings.DataSelector] = DataSelector;

        return new()
        {
            Name = Name,
            ProviderType = WebApiReportDataSourceProvider.ProviderType,
            Settings = settings,
            Schema = Schema,
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Name used by report bands and fields to reference this Web API data source.
    /// </summary>
    [Parameter] public string Name { get; set; } = "Default";

    /// <summary>
    /// Absolute HTTP or HTTPS endpoint URL.
    /// </summary>
    [Parameter] public string Url { get; set; }

    /// <summary>
    /// Optional request headers stored in the report definition. Do not use this collection for credentials that report authors must not see.
    /// </summary>
    [Parameter] public IReadOnlyDictionary<string, string> Headers { get; set; }

    /// <summary>
    /// Registered response format name or <see cref="WebApiReportDataSourceFormats.Auto" />.
    /// </summary>
    [Parameter] public string ResponseFormat { get; set; } = WebApiReportDataSourceFormats.Auto;

    /// <summary>
    /// Optional format-specific selector. The built-in readers use JSON Pointer for JSON and XPath for XML.
    /// </summary>
    [Parameter] public string DataSelector { get; set; }

    /// <summary>
    /// Optional field schema used instead of a schema inferred from a Web API response.
    /// </summary>
    [Parameter] public ReportDataSourceSchema Schema { get; set; }

    #endregion
}