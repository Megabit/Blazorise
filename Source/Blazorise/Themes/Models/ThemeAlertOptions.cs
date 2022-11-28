namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="Alert"/> component.
/// </summary>
public record ThemeAlertOptions : ThemeBasicOptions
{
    /// <summary>
    /// Defines a level of background color.
    /// </summary>
    /// <remarks>Negative level values will lighten the color, while higher levels will darken.</remarks>
    public int? BackgroundLevel { get; set; } = -10;

    /// <summary>
    /// Defines a level of border color.
    /// </summary>
    /// <remarks>Negative level values will lighten the color, while higher levels will darken.</remarks>
    public int? BorderLevel { get; set; } = -7;

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