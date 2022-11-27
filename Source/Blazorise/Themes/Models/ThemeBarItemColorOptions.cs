namespace Blazorise;

/// <summary>
/// Defines the color options for the <see cref="BarDropdownItem"/> component.
/// </summary>
public record ThemeBarItemColorOptions
{
    /// <summary>
    /// Gets or sets the item background color when it is selected.
    /// </summary>
    public string ActiveBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the item text color when it is selected.
    /// </summary>
    public string ActiveColor { get; set; }

    /// <summary>
    /// Gets or sets the item background color when it is hovered.
    /// </summary>
    public string HoverBackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the item text color when it is hovered.
    /// </summary>
    public string HoverColor { get; set; }
}