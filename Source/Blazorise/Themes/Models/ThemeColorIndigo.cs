#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ThemeColorIndigo : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#e8eaf6" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#c5cae9" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#9fa8da" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#7986cb" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#5c6bc0" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#3f51b5" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#3949ab" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#303f9f" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#283593" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#1a237e" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#8c9eff" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#536dfe" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#3d5afe" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#304ffe" );

        public ThemeColorIndigo() : base( "indigo", "Indigo" )
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
