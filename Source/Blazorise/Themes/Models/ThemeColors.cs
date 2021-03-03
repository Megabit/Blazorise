#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Default theme colors. 
    /// </summary>
    /// <remarks>
    /// Source: https://htmlcolorcodes.com/color-chart/material-design-color-chart/
    /// </remarks>
    public static class ThemeColors
    {
        public static ThemeColorRed Red { get; } = new ThemeColorRed();
        public static ThemeColorPink Pink { get; } = new ThemeColorPink();
        public static ThemeColorPurple Purple { get; } = new ThemeColorPurple();
        public static ThemeColorDeepPurple DeepPurple { get; } = new ThemeColorDeepPurple();
        public static ThemeColorIndigo Indigo { get; } = new ThemeColorIndigo();
        public static ThemeColorBlue Blue { get; } = new ThemeColorBlue();
        public static ThemeColorLightBlue LightBlue { get; } = new ThemeColorLightBlue();
        public static ThemeColorCyan Cyan { get; } = new ThemeColorCyan();
        public static ThemeColorTeal Teal { get; } = new ThemeColorTeal();
        public static ThemeColorGreen Green { get; } = new ThemeColorGreen();
        public static ThemeColorLightGreen LightGreen { get; } = new ThemeColorLightGreen();
        public static ThemeColorLime Lime { get; } = new ThemeColorLime();
        public static ThemeColorYellow Yellow { get; } = new ThemeColorYellow();
        public static ThemeColorAmber Amber { get; } = new ThemeColorAmber();
        public static ThemeColorOrange Orange { get; } = new ThemeColorOrange();
        public static ThemeColorDeepOrange DeepOrange { get; } = new ThemeColorDeepOrange();
        public static ThemeColorBrown Brown { get; } = new ThemeColorBrown();
        public static ThemeColorGray Gray { get; } = new ThemeColorGray();
        public static ThemeColorBlueGray BlueGray { get; } = new ThemeColorBlueGray();

        static ThemeColors()
        {
            Items = new Dictionary<string, ThemeColor>
            {
                {Red.Key, Red},
                {Pink.Key, Pink},
                {Purple.Key, Purple},
                {DeepPurple.Key, DeepPurple},
                {Indigo.Key, Indigo},
                {Blue.Key, Blue},
                {LightBlue.Key, LightBlue},
                {Cyan.Key, Cyan},
                {Teal.Key, Teal},
                {Green.Key, Green},
                {LightGreen.Key, LightGreen},
                {Lime.Key, Lime},
                {Yellow.Key, Yellow},
                {Amber.Key, Amber},
                {Orange.Key, Orange},
                {DeepOrange.Key, DeepOrange},
                {Brown.Key, Brown},
                {Gray.Key, Gray},
                {BlueGray.Key, BlueGray},
            };
        }

        public static Dictionary<string, ThemeColor> Items { get; set; }
    }
}
