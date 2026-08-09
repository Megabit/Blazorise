namespace Blazorise.Reporting.Internal;

/// <summary>
/// Defines which part of a table is resized by a designer pointer interaction.
/// </summary>
public enum ReportTableResizeKind
{
    /// <summary>
    /// A table column is resized.
    /// </summary>
    Column = 1,

    /// <summary>
    /// A table row is resized.
    /// </summary>
    Row = 2,
}