#region Using directives
#endregion

namespace Blazorise
{
    public class ThemeTooltipOptions : BasicOptions
    {
        public string BackgroundColor { get; set; } = "#808080";

        public float? BackgroundOpacity { get; set; } = 90f;

        public string Color { get; set; } = "#ffffff";

        public string FontSize { get; set; } = ".875rem";

        public string FadeTime { get; set; } = "0.3s";

        public string MaxWidth { get; set; } = "15rem";

        public string Padding { get; set; } = ".5rem 1rem";

        public string ZIndex { get; set; } = "1020";
    }
}
