#region Using directives
#endregion

using System;
using System.Collections.Generic;

namespace Blazorise
{
    public class ThemeBarOptions
    {
        public string VerticalWidth { get; set; } = "230px";

        public string VerticalSmallWidth { get; set; } = "64px";

        public string VerticalBrandHeight { get; set; } = "64px";

        public string VerticalPopoutMenuWidth { get; set; } = "180px";

        public string HorizontalHeight { get; set; } = "auto";

        public ThemeBarColorOptions DarkColors { get; set; }

        public ThemeBarColorOptions LightColors { get; set; }

        public override bool Equals( object obj )
        {
            return obj is ThemeBarOptions options &&
                     VerticalWidth == options.VerticalWidth &&
                     VerticalSmallWidth == options.VerticalSmallWidth &&
                     VerticalBrandHeight == options.VerticalBrandHeight &&
                     VerticalPopoutMenuWidth == options.VerticalPopoutMenuWidth &&
                     HorizontalHeight == options.HorizontalHeight &&
                    EqualityComparer<ThemeBarColorOptions>.Default.Equals( DarkColors, options.DarkColors ) &&
                    EqualityComparer<ThemeBarColorOptions>.Default.Equals( LightColors, options.LightColors );
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( VerticalWidth, VerticalSmallWidth, VerticalBrandHeight, VerticalPopoutMenuWidth, HorizontalHeight, DarkColors, LightColors );
        }
    }
}
