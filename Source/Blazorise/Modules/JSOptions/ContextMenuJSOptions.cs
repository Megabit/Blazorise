namespace Blazorise.Modules;

/// <summary>
/// Represents JavaScript options for initializing a context menu component.
/// </summary>
public class ContextMenuJSOptions
{
    /// <summary>
    /// Gets or sets the direction in which the context menu should open.
    /// </summary>
    public string Direction { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the context menu should be end-aligned.
    /// </summary>
    public bool EndAligned { get; set; }

    /// <summary>
    /// Gets or sets the strategy used to position the context menu.
    /// </summary>
    public string Strategy { get; set; }

    /// <summary>
    /// Gets or sets whether Floating UI should only be applied when the menu element is already positioned as <c>absolute</c> or <c>fixed</c>.
    /// </summary>
    public bool OnlyWhenPositioned { get; set; }
}