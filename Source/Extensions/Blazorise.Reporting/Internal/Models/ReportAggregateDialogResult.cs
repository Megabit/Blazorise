namespace Blazorise.Reporting.Internal;

/// <summary>
/// Contains the aggregate insertion settings selected in the aggregate dialog.
/// </summary>
public sealed class ReportAggregateDialogResult
{
    #region Properties

    /// <summary>
    /// Index of the detail band that provides the aggregate data context.
    /// </summary>
    public int SourceSectionIndex { get; set; }

    /// <summary>
    /// Index of the footer band that receives the aggregate element, or -1 for the report footer grand total.
    /// </summary>
    public int TargetSectionIndex { get; set; } = -1;

    /// <summary>
    /// Data source name or path used to calculate the aggregate.
    /// </summary>
    public string DataSourceName { get; set; }

    /// <summary>
    /// Field path summarized by the aggregate.
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// Aggregate function selected by the user.
    /// </summary>
    public ReportAggregateFunction Function { get; set; }

    #endregion
}