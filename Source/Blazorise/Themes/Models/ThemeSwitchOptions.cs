namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="Switch{TValue}">Switch</see> component.
/// </summary>
public record ThemeSwitchOptions : ThemeBasicOptions
{
    /// <summary>
    /// Gets or sets how much the switch box-shadow will lighten(range 0-100).
    /// </summary>
    public float? BoxShadowLightenColor { get; set; } = 25;

    /// <summary>
    /// Gets or sets how much the switch disabled color will lighten(range 0-100).
    /// </summary>
    public float? DisabledLightenColor { get; set; } = 50;
}