#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Default all the theme colors. 
    /// </summary>
    /// <remarks>
    /// Source: https://htmlcolorcodes.com/color-chart/material-design-color-chart/
    /// </remarks>
    public static class ThemeColors
    {
        /// <summary>
        /// Gets the theme <see cref="ThemeColorRed">Red</see> color.
        /// </summary>
        public static ThemeColorRed Red => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorPink">Pink</see> color.
        /// </summary>
        public static ThemeColorPink Pink => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorPurple">Purple</see> color.
        /// </summary>
        public static ThemeColorPurple Purple => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorDeepPurple">Deep Purple</see> color.
        /// </summary>
        public static ThemeColorDeepPurple DeepPurple => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorIndigo">Indigo</see> color.
        /// </summary>
        public static ThemeColorIndigo Indigo => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorBlue">Blue</see> color.
        /// </summary>
        public static ThemeColorBlue Blue => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorLightBlue">Light Blue</see> color.
        /// </summary>
        public static ThemeColorLightBlue LightBlue => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorCyan">Cyan</see> color.
        /// </summary>
        public static ThemeColorCyan Cyan => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorTeal">Teal</see> color.
        /// </summary>
        public static ThemeColorTeal Teal => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorGreen">Green</see> color.
        /// </summary>
        public static ThemeColorGreen Green => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorLightGreen">Light Green</see> color.
        /// </summary>
        public static ThemeColorLightGreen LightGreen => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorLime">Lime</see> color.
        /// </summary>
        public static ThemeColorLime Lime => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorYellow">Yellow</see> color.
        /// </summary>
        public static ThemeColorYellow Yellow => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorAmber">Amber</see> color.
        /// </summary>
        public static ThemeColorAmber Amber => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorOrange">Orange</see> color.
        /// </summary>
        public static ThemeColorOrange Orange => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorDeepOrange">Deep Orange</see> color.
        /// </summary>
        public static ThemeColorDeepOrange DeepOrange => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorBrown">Brown</see> color.
        /// </summary>
        public static ThemeColorBrown Brown => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorGray">Gray</see> color.
        /// </summary>
        public static ThemeColorGray Gray => new();

        /// <summary>
        /// Gets the theme <see cref="ThemeColorBlueGray">Blue Gray</see> color.
        /// </summary>
        public static ThemeColorBlueGray BlueGray => new();

        static ThemeColors()
        {
            Items = new()
            {
                { Red.Key, Red },
                { Pink.Key, Pink },
                { Purple.Key, Purple },
                { DeepPurple.Key, DeepPurple },
                { Indigo.Key, Indigo },
                { Blue.Key, Blue },
                { LightBlue.Key, LightBlue },
                { Cyan.Key, Cyan },
                { Teal.Key, Teal },
                { Green.Key, Green },
                { LightGreen.Key, LightGreen },
                { Lime.Key, Lime },
                { Yellow.Key, Yellow },
                { Amber.Key, Amber },
                { Orange.Key, Orange },
                { DeepOrange.Key, DeepOrange },
                { Brown.Key, Brown },
                { Gray.Key, Gray },
                { BlueGray.Key, BlueGray },
            };
        }

        /// <summary>
        /// Gets or sets the map of all theme colors.
        /// </summary>
        public static Dictionary<string, ThemeColor> Items { get; set; }
    }
}
