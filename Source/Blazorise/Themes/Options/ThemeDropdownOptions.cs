namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="Dropdown"/> component.
/// </summary>
public record ThemeDropdownOptions : ThemeBasicOptions
{
    /// <summary>
    /// Defines the percentage of how much the colors will blend together.
    /// </summary>
    public float? GradientBlendPercentage { get; set; } = 15f;

    /// <summary>
    /// Defines the visibility of dropdown toggle icon.
    /// </summary>
    public bool? ToggleIconVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets the global size for dropdown components.
    /// </summary>
    public Size? Size { get; set; }
}