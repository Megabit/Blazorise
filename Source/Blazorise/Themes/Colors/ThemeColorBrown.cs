#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the Brown color along with its color shades.
    /// </summary>
    public record ThemeColorBrown : ThemeColor
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ThemeColorShade _50 { get; } = new( "50", "_50", "#efebe9" );
        public ThemeColorShade _100 { get; } = new( "100", "_100", "#d7ccc8" );
        public ThemeColorShade _200 { get; } = new( "200", "_200", "#bcaaa4" );
        public ThemeColorShade _300 { get; } = new( "300", "_300", "#a1887f" );
        public ThemeColorShade _400 { get; } = new( "400", "_400", "#8d6e63" );
        public ThemeColorShade _500 { get; } = new( "500", "_500", "#795548" );
        public ThemeColorShade _600 { get; } = new( "600", "_600", "#6d4c41" );
        public ThemeColorShade _700 { get; } = new( "700", "_700", "#5d4037" );
        public ThemeColorShade _800 { get; } = new( "800", "_800", "#4e342e" );
        public ThemeColorShade _900 { get; } = new( "900", "_900", "#3e2723" );
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// A default <see cref="ThemeColorBrown"/> constructor
        /// </summary>
        public ThemeColorBrown() : base( "brown", "Brown" )
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
