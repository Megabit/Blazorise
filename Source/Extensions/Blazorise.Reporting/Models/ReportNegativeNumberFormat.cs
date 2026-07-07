namespace Blazorise.Reporting;

/// <summary>
/// Defines how negative numeric report values are displayed.
/// </summary>
public enum ReportNegativeNumberFormat
{
    /// <summary>
    /// Uses the culture default negative format.
    /// </summary>
    Default,

    /// <summary>
    /// Displays negative values with a leading minus sign.
    /// </summary>
    MinusSign,

    /// <summary>
    /// Displays negative values wrapped in parentheses.
    /// </summary>
    Parentheses,
}