﻿#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise
{
    public class ThemeColorBlue : ThemeColor
    {
        public ThemeColorShade _50 { get; } = new ThemeColorShade( "50", "_50", "#e3f2fd" );
        public ThemeColorShade _100 { get; } = new ThemeColorShade( "100", "_100", "#bbdefb" );
        public ThemeColorShade _200 { get; } = new ThemeColorShade( "200", "_200", "#90caf9" );
        public ThemeColorShade _300 { get; } = new ThemeColorShade( "300", "_300", "#64b5f6" );
        public ThemeColorShade _400 { get; } = new ThemeColorShade( "400", "_400", "#42a5f5" );
        public ThemeColorShade _500 { get; } = new ThemeColorShade( "500", "_500", "#2196f3" );
        public ThemeColorShade _600 { get; } = new ThemeColorShade( "600", "_600", "#1e88e5" );
        public ThemeColorShade _700 { get; } = new ThemeColorShade( "700", "_700", "#1976d2" );
        public ThemeColorShade _800 { get; } = new ThemeColorShade( "800", "_800", "#1565c0" );
        public ThemeColorShade _900 { get; } = new ThemeColorShade( "900", "_900", "#0d47a1" );
        public ThemeColorShade A100 { get; } = new ThemeColorShade( "A100", "A100", "#82b1ff" );
        public ThemeColorShade A200 { get; } = new ThemeColorShade( "A200", "A200", "#448aff" );
        public ThemeColorShade A400 { get; } = new ThemeColorShade( "A400", "A400", "#2979ff" );
        public ThemeColorShade A700 { get; } = new ThemeColorShade( "A700", "A700", "#2962ff" );

        public ThemeColorBlue() : base( "blue", "Blue" )
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
