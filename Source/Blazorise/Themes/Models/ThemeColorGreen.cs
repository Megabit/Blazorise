#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ThemeColorGreen : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#e8f5e9" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#c8e6c9" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#a5d6a7" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#81c784" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#66bb6a" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#4caf50" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#43a047" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#388e3c" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#2e7d32" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#1b5e20" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#b9f6ca" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#69f0ae" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#00e676" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#00c853" );

        public ThemeColorGreen() : base( "green", "Green" )
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
