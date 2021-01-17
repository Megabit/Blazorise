#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ThemeColorAmber : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#fff8e1" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#ffecb3" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#ffe082" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#ffd54f" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#ffca28" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#ffc107" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#ffb300" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#ffa000" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#ff8f00" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#ff6f00" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#ffe57f" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#ffd740" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#ffc400" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#ffab00" );

        public ThemeColorAmber() : base( "amber", "Amber" )
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
