namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="BarDropdown"/> component.
/// </summary>
public record ThemeBarDropdownOptions
{
    /// <summary>
    /// Defines the visibility of bar dropdown toggle icon.
    /// </summary>
    public bool? ToggleIconVisible { get; set; }

    /// <summary>
    /// Defines the icon name for the expand state of the dropdown.
    /// </summary>
    public IconName ToggleExpandIconName { get; set; } = IconName.ChevronDown;

    /// <summary>
    /// Defines the icon name for the collapse state of the dropdown.
    /// </summary>
    public IconName ToggleCollapseIconName { get; set; } = IconName.ChevronUp;

    /// <summary>
    ///  Defines the size of the toggle icon for the dropdown.
    /// </summary>
    public IconSize ToggleIconSIze { get; set; } = IconSize.ExtraSmall;
}