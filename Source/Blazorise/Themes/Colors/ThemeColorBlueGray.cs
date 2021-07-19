#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the Blue-Gray color along with its color shades.
    /// </summary>
    public record ThemeColorBlueGray : ThemeColor
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ThemeColorShade _50 { get; } = new( "50", "_50", "#eceff1" );
        public ThemeColorShade _100 { get; } = new( "100", "_100", "#cfd8dc" );
        public ThemeColorShade _200 { get; } = new( "200", "_200", "#b0bec5" );
        public ThemeColorShade _300 { get; } = new( "300", "_300", "#90a4ae" );
        public ThemeColorShade _400 { get; } = new( "400", "_400", "#78909c" );
        public ThemeColorShade _500 { get; } = new( "500", "_500", "#607d8b" );
        public ThemeColorShade _600 { get; } = new( "600", "_600", "#546e7a" );
        public ThemeColorShade _700 { get; } = new( "700", "_700", "#455a64" );
        public ThemeColorShade _800 { get; } = new( "800", "_800", "#37474f" );
        public ThemeColorShade _900 { get; } = new( "900", "_900", "#263238" );
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// A default <see cref="ThemeColorBlueGray"/> constructor
        /// </summary>
        public ThemeColorBlueGray() : base( "blue-gray", "BlueGray" )
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
