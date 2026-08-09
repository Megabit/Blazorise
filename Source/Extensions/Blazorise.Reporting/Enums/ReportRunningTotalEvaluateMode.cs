namespace Blazorise.Reporting;

/// <summary>
/// Defines when a running total should include the current detail record.
/// </summary>
public enum ReportRunningTotalEvaluateMode
{
    /// <summary>
    /// Includes every record in the running total.
    /// </summary>
    EveryRecord,

    /// <summary>
    /// Includes records only when the evaluate formula resolves to true.
    /// </summary>
    Formula
}