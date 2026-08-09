namespace Blazorise.Reporting.DataSources.Sql;

/// <summary>
/// Defines setting names understood by the SQL report data source provider.
/// </summary>
public static class SqlReportDataSourceSettings
{
    #region Fields

    /// <summary>
    /// Name of the host-registered connection factory.
    /// </summary>
    public const string ConnectionName = "ConnectionName";

    /// <summary>
    /// SQL query used to load report rows.
    /// </summary>
    public const string Query = "Query";

    /// <summary>
    /// Optional command timeout in seconds.
    /// </summary>
    public const string CommandTimeout = "CommandTimeout";

    #endregion
}