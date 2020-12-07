﻿#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ThemeColorDeepOrange : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#fbe9e7" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#ffccbc" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#ffab91" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#ff8a65" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#ff7043" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#ff5722" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#f4511e" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#e64a19" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#d84315" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#bf360c" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#ff9e80" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#ff6e40" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#ff3d00" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#dd2c00" );

        public ThemeColorDeepOrange() : base( "deep-orange", "DeepOrange" )
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
