namespace Blazorise.Reporting;

/// <summary>
/// Defines when a running total should reset its accumulated value.
/// </summary>
public enum ReportRunningTotalResetMode
{
    /// <summary>
    /// Keeps the running total value for the whole report.
    /// </summary>
    Never,

    /// <summary>
    /// Resets the running total when the selected group changes.
    /// </summary>
    Group
}