namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="Progress"/> component.
/// </summary>
public record ThemeProgressOptions : ThemeBasicOptions
{
    /// <summary>
    /// Default color of the progress bar.
    /// </summary>
    public string PageProgressDefaultColor { get; set; } = "#ffffff";

    /// <summary>
    /// Gets or sets the global size for progress components.
    /// </summary>
    public Size? Size { get; set; }
}