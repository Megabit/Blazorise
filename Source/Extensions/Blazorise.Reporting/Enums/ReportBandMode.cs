namespace Blazorise.Reporting;

/// <summary>
/// Defines how report bands are shown in the designer surface.
/// </summary>
public enum ReportBandMode
{
    /// <summary>
    /// Shows a left rail with collapsible band labels.
    /// </summary>
    Rail,

    /// <summary>
    /// Shows section separators inside the report page.
    /// </summary>
    Separator,

    /// <summary>
    /// Shows a compact band presentation.
    /// </summary>
    Compact,

    /// <summary>
    /// Shows horizontal band headers across the report page.
    /// </summary>
    Classic,
}