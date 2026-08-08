namespace Blazorise.Reporting.DataSources.WebApi;

/// <summary>
/// Built-in Web API response format names.
/// </summary>
public static class WebApiReportDataSourceFormats
{
    #region Fields

    /// <summary>
    /// Selects a response reader from the HTTP content type and response content.
    /// </summary>
    public const string Auto = "auto";

    /// <summary>
    /// JavaScript Object Notation response format.
    /// </summary>
    public const string Json = "json";

    /// <summary>
    /// Extensible Markup Language response format.
    /// </summary>
    public const string Xml = "xml";

    #endregion
}