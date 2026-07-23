namespace Blazorise;

/// <summary>
/// Defines the kind of node in a dock layout tree.
/// </summary>
public enum DockNodeKind
{
    /// <summary>
    /// Represents a single dock pane.
    /// </summary>
    Pane,

    /// <summary>
    /// Represents the central dock content.
    /// </summary>
    Content,

    /// <summary>
    /// Represents a split container with two child nodes.
    /// </summary>
    Split,

    /// <summary>
    /// Represents a tabbed pane group.
    /// </summary>
    Tabs,
}