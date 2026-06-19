namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="DockLayout"/> component.
/// </summary>
public record ThemeDockLayoutOptions
{
    /// <summary>
    /// Pane background color.
    /// </summary>
    public string PaneBackground { get; set; }

    /// <summary>
    /// Pane text color.
    /// </summary>
    public string PaneColor { get; set; }

    /// <summary>
    /// Pane border color.
    /// </summary>
    public string PaneBorderColor { get; set; }

    /// <summary>
    /// Pane header background color.
    /// </summary>
    public string PaneHeaderBackground { get; set; }

    /// <summary>
    /// Pane footer background color.
    /// </summary>
    public string PaneFooterBackground { get; set; }

    /// <summary>
    /// Background color used by hoverable actions inside DockLayout.
    /// </summary>
    public string HoverBackground { get; set; }

    /// <summary>
    /// Tabs strip background color.
    /// </summary>
    public string TabsBackground { get; set; }

    /// <summary>
    /// Active tab background color.
    /// </summary>
    public string TabActiveBackground { get; set; }

    /// <summary>
    /// Active tab text color.
    /// </summary>
    public string TabActiveColor { get; set; }

    /// <summary>
    /// Tab close button color.
    /// </summary>
    public string TabCloseColor { get; set; }

    /// <summary>
    /// Drag preview background color.
    /// </summary>
    public string DragPreviewBackground { get; set; }

    /// <summary>
    /// Drag preview border color.
    /// </summary>
    public string DragPreviewBorderColor { get; set; }

    /// <summary>
    /// Drop preview background color.
    /// </summary>
    public string DropPreviewBackground { get; set; }

    /// <summary>
    /// Drop preview border color.
    /// </summary>
    public string DropPreviewBorderColor { get; set; }

    /// <summary>
    /// Compass zone background color.
    /// </summary>
    public string CompassZoneBackground { get; set; }

    /// <summary>
    /// Compass zone text color.
    /// </summary>
    public string CompassZoneColor { get; set; }

    /// <summary>
    /// Compass zone border color.
    /// </summary>
    public string CompassZoneBorderColor { get; set; }

    /// <summary>
    /// Active compass zone background color.
    /// </summary>
    public string CompassZoneActiveBackground { get; set; }

    /// <summary>
    /// Active compass zone text color.
    /// </summary>
    public string CompassZoneActiveColor { get; set; }

    /// <summary>
    /// Active compass zone border color.
    /// </summary>
    public string CompassZoneActiveBorderColor { get; set; }
}