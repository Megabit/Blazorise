namespace Blazorise
{
    public record ThemeSwitchOptions : ThemeBasicOptions
    {
        public float BoxShadowLightenColor { get; set; } = 25;

        public float DisabledLightenColor { get; set; } = 50;
    }
}
