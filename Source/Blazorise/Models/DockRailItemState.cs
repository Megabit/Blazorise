namespace Blazorise;

/// <summary>
/// Represents one auto-hidden pane tab inside a <see cref="DockRailState"/>.
/// </summary>
public class DockRailItemState
{
    /// <summary>
    /// Identifies the pane shown in the auto-hide rail.
    /// </summary>
    public string PaneName { get; set; }

    /// <summary>
    /// Identifies the original tab group that owned the pane before it was auto-hidden.
    /// </summary>
    public string SourceGroupId { get; set; }

    /// <summary>
    /// Identifies a pane that remained in the original tab group when this pane was auto-hidden.
    /// </summary>
    public string SourceTabPaneName { get; set; }

    /// <summary>
    /// Defines the original pane position before it was auto-hidden.
    /// </summary>
    public DockPanePosition SourcePosition { get; set; }

    /// <summary>
    /// Defines the original pane or tab group size before it was auto-hidden.
    /// </summary>
    public string SourceSize { get; set; }

    /// <summary>
    /// Defines the original owning split ratio before the pane was auto-hidden.
    /// </summary>
    public double? SourceSplitRatio { get; set; }

    /// <summary>
    /// Defines whether the original owning split used ratio based sizing.
    /// </summary>
    public bool? SourceSplitUseRatio { get; set; }

    /// <summary>
    /// Identifies the sibling pane that the auto-hidden pane should dock next to when restored.
    /// </summary>
    public string SourceTargetPaneName { get; set; }

    /// <summary>
    /// Identifies the sibling dock node that the auto-hidden pane should dock next to when restored.
    /// </summary>
    public string SourceTargetNodeId { get; set; }

    /// <summary>
    /// Defines the side of the sibling target where the auto-hidden pane should be restored.
    /// </summary>
    public DockZone? SourceZone { get; set; }

    /// <summary>
    /// Defines the pane order inside its original tab group.
    /// </summary>
    public int SourceIndex { get; set; }

    /// <summary>
    /// Defines the order inside the auto-hide rail.
    /// </summary>
    public int Order { get; set; }
}