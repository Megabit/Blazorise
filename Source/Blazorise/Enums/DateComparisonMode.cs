namespace Blazorise;

/// <summary>
/// Defines which parts of a date value participate in comparisons.
/// </summary>
public enum DateComparisonMode
{
    /// <summary>
    /// Compares both the date and time components.
    /// </summary>
    DateTime = 0,

    /// <summary>
    /// Compares calendar dates, ignoring the time component without converting time zones.
    /// </summary>
    DateOnly = 1,
}