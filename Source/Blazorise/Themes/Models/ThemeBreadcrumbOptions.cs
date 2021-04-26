namespace Blazorise
{
    public record ThemeBreadcrumbOptions : ThemeBasicOptions
    {
        public string Color { get; set; } = ThemeColors.Blue.Shades["400"].Value;
    }
}
