#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the Cyan color along with its color shades.
    /// </summary>
    public record ThemeColorCyan : ThemeColor
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ThemeColorShade _50 { get; } = new( "50", "_50", "#e0f7fa" );
        public ThemeColorShade _100 { get; } = new( "100", "_100", "#b2ebf2" );
        public ThemeColorShade _200 { get; } = new( "200", "_200", "#80deea" );
        public ThemeColorShade _300 { get; } = new( "300", "_300", "#4dd0e1" );
        public ThemeColorShade _400 { get; } = new( "400", "_400", "#26c6da" );
        public ThemeColorShade _500 { get; } = new( "500", "_500", "#00bcd4" );
        public ThemeColorShade _600 { get; } = new( "600", "_600", "#00acc1" );
        public ThemeColorShade _700 { get; } = new( "700", "_700", "#0097a7" );
        public ThemeColorShade _800 { get; } = new( "800", "_800", "#00838f" );
        public ThemeColorShade _900 { get; } = new( "900", "_900", "#006064" );
        public ThemeColorShade A100 { get; } = new( "A100", "A100", "#84ffff" );
        public ThemeColorShade A200 { get; } = new( "A200", "A200", "#18ffff" );
        public ThemeColorShade A400 { get; } = new( "A400", "A400", "#00e5ff" );
        public ThemeColorShade A700 { get; } = new( "A700", "A700", "#00b8d4" );
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// A default <see cref="ThemeColorCyan"/> constructor
        /// </summary>
        public ThemeColorCyan() : base( "cyan", "Cyan" )
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
                { A100.Key, A100 },
                { A200.Key, A200 },
                { A400.Key, A400 },
                { A700.Key, A700 },
            };
        }
    }
}
