#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ThemeColorLightBlue : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#e1f5fe" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#b3e5fc" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#81d4fa" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#4fc3f7" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#29b6f6" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#03a9f4" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#039be5" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#0288d1" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#0277bd" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#01579b" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#80d8ff" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#40c4ff" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#00b0ff" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#0091ea" );

        public ThemeColorLightBlue() : base( "light-blue", "LightBlue" )
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
