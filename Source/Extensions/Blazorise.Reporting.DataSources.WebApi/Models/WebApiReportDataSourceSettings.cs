namespace Blazorise.Reporting.DataSources.WebApi;

/// <summary>
/// Defines setting names understood by the Web API report data source provider.
/// </summary>
public static class WebApiReportDataSourceSettings
{
    #region Fields

    /// <summary>
    /// Absolute HTTP or HTTPS endpoint URL.
    /// </summary>
    public const string Url = "Url";

    /// <summary>
    /// Optional request headers, with one <c>Name: Value</c> pair per line.
    /// </summary>
    public const string Headers = "Headers";

    /// <summary>
    /// Response format name or <see cref="WebApiReportDataSourceFormats.Auto" />.
    /// </summary>
    public const string ResponseFormat = "ResponseFormat";

    /// <summary>
    /// Optional format-specific selector applied to the response data.
    /// </summary>
    public const string DataSelector = "DataSelector";

    #endregion
}