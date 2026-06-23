namespace Blazorise;

/// <summary>
/// Stores the source placement used to restore a dock pane after it is hidden from the visible layout.
/// </summary>
public class DockPaneRestoreState
{
    /// <summary>
    /// Identifies the original tab group that owned the pane before it was hidden.
    /// </summary>
    public string SourceGroupId { get; set; }

    /// <summary>
    /// Identifies a pane that remained in the original tab group when this pane was hidden.
    /// </summary>
    public string SourceTabPaneName { get; set; }

    /// <summary>
    /// Defines the original pane position before it was hidden.
    /// </summary>
    public DockPanePosition SourcePosition { get; set; }

    /// <summary>
    /// Defines the original pane or tab group size before it was hidden.
    /// </summary>
    public string SourceSize { get; set; }

    /// <summary>
    /// Defines the original owning split ratio before the pane was hidden.
    /// </summary>
    public double? SourceSplitRatio { get; set; }

    /// <summary>
    /// Defines whether the original owning split used ratio based sizing.
    /// </summary>
    public bool? SourceSplitUseRatio { get; set; }

    /// <summary>
    /// Identifies the sibling pane that the original source group should dock next to when restored.
    /// </summary>
    public string SourceGroupTargetPaneName { get; set; }

    /// <summary>
    /// Identifies the sibling dock node that the original source group should dock next to when restored.
    /// </summary>
    public string SourceGroupTargetNodeId { get; set; }

    /// <summary>
    /// Defines the side of the source group sibling target where the pane should be restored.
    /// </summary>
    public DockZone? SourceGroupZone { get; set; }

    /// <summary>
    /// Defines the original source group split ratio before the pane was hidden.
    /// </summary>
    public double? SourceGroupSplitRatio { get; set; }

    /// <summary>
    /// Defines whether the original source group split used ratio based sizing.
    /// </summary>
    public bool? SourceGroupSplitUseRatio { get; set; }

    /// <summary>
    /// Identifies the sibling pane that the hidden pane should dock next to when restored.
    /// </summary>
    public string SourceTargetPaneName { get; set; }

    /// <summary>
    /// Identifies the sibling dock node that the hidden pane should dock next to when restored.
    /// </summary>
    public string SourceTargetNodeId { get; set; }

    /// <summary>
    /// Defines the side of the sibling target where the hidden pane should be restored.
    /// </summary>
    public DockZone? SourceZone { get; set; }

    /// <summary>
    /// Defines the pane order inside its original tab group.
    /// </summary>
    public int SourceIndex { get; set; }
}