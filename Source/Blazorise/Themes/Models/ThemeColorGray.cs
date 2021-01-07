#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ThemeColorGray : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#fafafa" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#f5f5f5" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#eee" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#e0e0e0" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#bdbdbd" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#9e9e9e" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#757575" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#616161" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#424242" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#212121" );

        public ThemeColorGray() : base( "gray", "Gray" )
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
