namespace Blazorise.Reporting;

/// <summary>
/// Defines the report elements supported by the designer.
/// </summary>
public enum ReportElementType
{
    /// <summary>
    /// Static text content.
    /// </summary>
    Text,

    /// <summary>
    /// Data-bound field content.
    /// </summary>
    Field,

    /// <summary>
    /// Table layout container.
    /// </summary>
    Table,

    /// <summary>
    /// Image element.
    /// </summary>
    Image,

    /// <summary>
    /// Horizontal or vertical line element.
    /// </summary>
    Line,

    /// <summary>
    /// Rectangular shape element.
    /// </summary>
    Rectangle,

    /// <summary>
    /// Explicit page break marker.
    /// </summary>
    PageBreak
}