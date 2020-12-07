#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ThemeColorPurple : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#f3e5f5" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#e1bee7" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#ce93d8" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#ba68c8" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#ab47bc" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#9c27b0" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#8e24aa" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#7b1fa2" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#6a1b9a" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#4a148c" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#ea80fc" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#e040fb" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#d500f9" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#a0f" );

        public ThemeColorPurple() : base( "purple", "Purple" )
        {
            Shades = new Dictionary<string, ThemeColorShade>()
            {
                {_50.Key, _50},
                {_100.Key, _100},
                {_200.Key, _200},
                {_300.Key, _300},
                {_400.Key, _400},
                {_500.Key, _500},
                {_600.Key, _600},
                {_700.Key, _700},
                {_800.Key, _800},
                {_900.Key, _900},
                {A100.Key, A100},
                {A200.Key, A200},
                {A400.Key, A400},
                {A700.Key, A700},
            };
        }
    }
}
