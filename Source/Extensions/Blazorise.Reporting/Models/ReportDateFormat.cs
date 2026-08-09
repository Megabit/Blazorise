namespace Blazorise.Reporting;

/// <summary>
/// Defines common date and time patterns for report values.
/// </summary>
public enum ReportDateFormat
{
    /// <summary>
    /// Uses the current culture short date pattern.
    /// </summary>
    ShortDate,

    /// <summary>
    /// Uses the current culture long date pattern.
    /// </summary>
    LongDate,

    /// <summary>
    /// Uses the current culture short time pattern.
    /// </summary>
    ShortTime,

    /// <summary>
    /// Uses the current culture long time pattern.
    /// </summary>
    LongTime,

    /// <summary>
    /// Uses the current culture short date and short time pattern.
    /// </summary>
    ShortDateTime,

    /// <summary>
    /// Uses the current culture long date and short time pattern.
    /// </summary>
    LongDateTime,
}