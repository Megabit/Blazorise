namespace Blazorise.Reporting;

/// <summary>
/// Represents data loaded by a report data source provider.
/// </summary>
public sealed class ReportDataSourceResult
{
    #region Properties

    /// <summary>
    /// Data object consumed by report bands and field expressions.
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// Schema discovered while loading the data source.
    /// </summary>
    public ReportDataSourceSchema Schema { get; set; }

    #endregion
}