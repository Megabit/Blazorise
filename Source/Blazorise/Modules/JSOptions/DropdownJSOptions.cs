namespace Blazorise.Modules;

/// <summary>
/// Represents JavaScript options for initializing a dropdown component.
/// </summary>
public class DropdownInitializeJSOptions
{
    /// <summary>
    /// Gets or sets the ID of the target element that will trigger the dropdown.
    /// </summary>
    public string TargetElementId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the menu element that represents the dropdown content.
    /// </summary>
    public string MenuElementId { get; set; }

    /// <summary>
    /// Gets or sets the direction in which the dropdown should open (e.g., "down", "up").
    /// </summary>
    public string Direction { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the dropdown menu is right-aligned.
    /// </summary>
    public bool RightAligned { get; set; }

    /// <summary>
    /// Gets or sets the CSS class names for styling the dropdown toggle element.
    /// </summary>
    public string DropdownToggleClassNames { get; set; }

    /// <summary>
    /// Gets or sets the CSS class names for styling the dropdown menu element.
    /// </summary>
    public string DropdownMenuClassNames { get; set; }

    /// <summary>
    /// Gets or sets the CSS class name applied when the dropdown is shown.
    /// </summary>
    public string DropdownShowClassName { get; set; }

    /// <summary>
    /// Gets or sets the strategy used to position the dropdown (e.g., "absolute", "fixed").
    /// </summary>
    public string Strategy { get; set; }
}