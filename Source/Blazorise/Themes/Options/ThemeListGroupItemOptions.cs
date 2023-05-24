namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="ListGroupItem"/> component.
/// </summary>
public record ThemeListGroupItemOptions
{
    /// <summary>
    /// Defines a level of background color.
    /// </summary>
    /// <remarks>Negative level values will lighten the color, while higher levels will darken.</remarks>
    public int? BackgroundLevel { get; set; } = -9;

    /// <summary>
    /// Defines a level of text color.
    /// </summary>
    /// <remarks>Negative level values will lighten the color, while higher levels will darken.</remarks>
    public int? ColorLevel { get; set; } = 6;

    /// <summary>
    /// Defines the percentage of how much the colors will blend together.
    /// </summary>
    public float? GradientBlendPercentage { get; set; } = 15f;
}