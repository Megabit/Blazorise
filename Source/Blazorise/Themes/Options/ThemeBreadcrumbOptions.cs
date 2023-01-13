namespace Blazorise;

/// <summary>
/// Defines the theme options for the <see cref="Breadcrumb"/> component.
/// </summary>
public record ThemeBreadcrumbOptions : ThemeBasicOptions
{
    /// <summary>
    /// Gets or sets the breadcrumb text color.
    /// </summary>
    public string Color { get; set; } = ThemeColors.Blue.Shades["400"].Value;
}