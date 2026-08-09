namespace Blazorise.Reporting.Internal;

/// <summary>
/// Represents an aggregate summary target shown by designer dialogs.
/// </summary>
public sealed class ReportAggregateSummaryLocation
{
    #region Properties

    /// <summary>
    /// Index of the section where the aggregate summary will be inserted.
    /// </summary>
    public int TargetSectionIndex { get; set; }

    /// <summary>
    /// Label shown for the summary target in designer dialogs.
    /// </summary>
    public string Name { get; set; }

    #endregion
}