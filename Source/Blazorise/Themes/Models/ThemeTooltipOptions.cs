#region Using directives
#endregion

using System;

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

        public override bool Equals( object obj )
        {
            return obj is ThemeTooltipOptions options &&
                     base.Equals( obj ) &&
                     BackgroundColor == options.BackgroundColor &&
                     Color == options.Color &&
                     FontSize == options.FontSize &&
                     FadeTime == options.FadeTime &&
                     MaxWidth == options.MaxWidth &&
                     Padding == options.Padding &&
                     ZIndex == options.ZIndex;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( base.GetHashCode(), BackgroundColor, Color, FontSize, FadeTime, MaxWidth, Padding, ZIndex );
        }
    }
}
