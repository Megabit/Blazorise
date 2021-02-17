#region Using directives
#endregion

namespace Blazorise
{
    public class ThemeTooltipOptions : BasicOptions
    {
        /// <summary>
        /// Tooltip background color. Can contain alpha value in 8-hex formated color.
        /// </summary>
        public string BackgroundColor { get; set; } = "#808080e6";

        public string Color { get; set; } = "#ffffff";

        public string FontSize { get; set; } = ".875rem";

        public string FadeTime { get; set; } = "0.3s";

        public string MaxWidth { get; set; } = "15rem";

        public string Padding { get; set; } = ".5rem 1rem";

        public string ZIndex { get; set; } = "1020";
    }
}
