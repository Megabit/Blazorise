namespace Blazorise.Reporting;

/// <summary>
/// Defines the user-facing value formatting category used by report elements.
/// </summary>
public enum ReportFormatCategory
{
    /// <summary>
    /// Formats the value as plain text.
    /// </summary>
    Text,

    /// <summary>
    /// Formats numeric values with optional decimal places and grouping.
    /// </summary>
    Number,

    /// <summary>
    /// Formats numeric values as currency.
    /// </summary>
    Currency,

    /// <summary>
    /// Formats numeric values as percentages.
    /// </summary>
    Percent,

    /// <summary>
    /// Formats date values.
    /// </summary>
    Date,

    /// <summary>
    /// Formats time values.
    /// </summary>
    Time,

    /// <summary>
    /// Formats date and time values.
    /// </summary>
    DateTime,

    /// <summary>
    /// Formats boolean values.
    /// </summary>
    Boolean,

    /// <summary>
    /// Uses a custom .NET format string.
    /// </summary>
    Custom,
}