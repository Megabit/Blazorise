namespace Blazorise.Reporting.Internal;

/// <summary>
/// Defines icon and styling categories for report explorer tree nodes.
/// </summary>
public enum ReportTreeNodeKind
{
    /// <summary>
    /// Root report node.
    /// </summary>
    Report,

    /// <summary>
    /// Data source node.
    /// </summary>
    DataSource,

    /// <summary>
    /// Data field node.
    /// </summary>
    Field,

    /// <summary>
    /// Report band node.
    /// </summary>
    Band,

    /// <summary>
    /// Text element node.
    /// </summary>
    Text,

    /// <summary>
    /// Table element node.
    /// </summary>
    Table,

    /// <summary>
    /// Image element node.
    /// </summary>
    Image,

    /// <summary>
    /// Line element node.
    /// </summary>
    Line,

    /// <summary>
    /// Rectangle element node.
    /// </summary>
    Rectangle,

    /// <summary>
    /// Page break element node.
    /// </summary>
    PageBreak,

    /// <summary>
    /// Grouping folder node.
    /// </summary>
    Folder,
}