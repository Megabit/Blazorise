namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="Rating"/> component.
/// </summary>
public record ThemeRatingOptions : ThemeBasicOptions
{
    /// <summary>
    /// Gets or sets the mouse hover opacity for the rating item(range 0-1).
    /// </summary>
    public float? HoverOpacity { get; set; } = 0.7f;
}