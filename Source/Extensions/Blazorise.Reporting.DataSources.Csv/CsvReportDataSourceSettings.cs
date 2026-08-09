namespace Blazorise.Reporting.DataSources.Csv;

/// <summary>
/// Defines setting names understood by the CSV report data source provider.
/// </summary>
public static class CsvReportDataSourceSettings
{
    #region Fields

    /// <summary>
    /// CSV source stored as inline text, a local file path, or an HTTP URL.
    /// </summary>
    public const string Source = "Source";

    /// <summary>
    /// Text encoding used when reading CSV content from a file path or URL.
    /// </summary>
    public const string Encoding = "Encoding";

    /// <summary>
    /// Encoding value that instructs the provider to use the runtime default encoding.
    /// </summary>
    public const string SystemEncoding = "System";

    /// <summary>
    /// Single-character delimiter used to split CSV fields.
    /// </summary>
    public const string Delimiter = "Delimiter";

    /// <summary>
    /// Indicates whether the first CSV row contains column names.
    /// </summary>
    public const string HasHeaderRow = "HasHeaderRow";

    #endregion
}