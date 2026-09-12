namespace Blazorise.Modules;

/// <summary>
/// Options for reordering items in a drop zone.
/// </summary>
public class DragDropJSOptions
{
    /// <summary>
    /// Gets or sets whether item reordering is enabled.
    /// </summary>
    public bool AllowReorder { get; set; }

    /// <summary>
    /// Gets or sets whether reorder animations are enabled.
    /// </summary>
    public bool Animated { get; set; }

    /// <summary>
    /// Gets or sets the animation duration, in milliseconds.
    /// </summary>
    public int AnimationDuration { get; set; }
}