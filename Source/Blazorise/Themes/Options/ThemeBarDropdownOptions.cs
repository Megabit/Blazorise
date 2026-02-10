namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="BarDropdown"/> component.
/// </summary>
public record ThemeBarDropdownOptions
{
    /// <summary>
    /// Defines the visibility of bar dropdown toggle icon.
    /// </summary>
    public bool? ToggleIconVisible { get; set; } = true;
}