namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="Table"/> component.
/// </summary>
public record ThemeTableOptions : ThemeBasicOptions
{
    /// <summary>
    /// Defines a level of background color.
    /// </summary>
    /// <remarks>Negative level values will lighten the color, while higher levels will darken.</remarks>
    public int? BackgroundLevel { get; set; } = -9;

    /// <summary>
    /// Defines a level of border color.
    /// </summary>
    /// <remarks>Negative level values will lighten the color, while higher levels will darken.</remarks>
    public int? BorderLevel { get; set; } = -6;
}