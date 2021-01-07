#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ThemeColorLightGreen : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#f1f8e9" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#dcedc8" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#c5e1a5" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#aed581" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#9ccc65" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#8bc34a" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#7cb342" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#689f38" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#558b2f" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#33691e" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#ccff90" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#b2ff59" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#76ff03" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#64dd17" );

        public ThemeColorLightGreen() : base( "light-green", "LightGreen" )
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
