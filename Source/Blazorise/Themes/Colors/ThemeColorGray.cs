﻿#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the Gray color along with its color shades.
    /// </summary>
    public record ThemeColorGray : ThemeColor
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ThemeColorShade _50 { get; } = new( "50", "_50", "#fafafa" );
        public ThemeColorShade _100 { get; } = new( "100", "_100", "#f5f5f5" );
        public ThemeColorShade _200 { get; } = new( "200", "_200", "#eee" );
        public ThemeColorShade _300 { get; } = new( "300", "_300", "#e0e0e0" );
        public ThemeColorShade _400 { get; } = new( "400", "_400", "#bdbdbd" );
        public ThemeColorShade _500 { get; } = new( "500", "_500", "#9e9e9e" );
        public ThemeColorShade _600 { get; } = new( "600", "_600", "#757575" );
        public ThemeColorShade _700 { get; } = new( "700", "_700", "#616161" );
        public ThemeColorShade _800 { get; } = new( "800", "_800", "#424242" );
        public ThemeColorShade _900 { get; } = new( "900", "_900", "#212121" );
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// A default <see cref="ThemeColorGray"/> constructor
        /// </summary>
        public ThemeColorGray() : base( "gray", "Gray" )
        {
            Shades = new()
            {
                { _50.Key, _50 },
                { _100.Key, _100 },
                { _200.Key, _200 },
                { _300.Key, _300 },
                { _400.Key, _400 },
                { _500.Key, _500 },
                { _600.Key, _600 },
                { _700.Key, _700 },
                { _800.Key, _800 },
                { _900.Key, _900 },
            };
        }
    }
}
