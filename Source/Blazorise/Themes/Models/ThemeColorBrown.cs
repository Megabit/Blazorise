#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ThemeColorBrown : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#efebe9" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#d7ccc8" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#bcaaa4" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#a1887f" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#8d6e63" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#795548" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#6d4c41" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#5d4037" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#4e342e" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#3e2723" );

        public ThemeColorBrown() : base( "brown", "Brown" )
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
            };
        }
    }
}
