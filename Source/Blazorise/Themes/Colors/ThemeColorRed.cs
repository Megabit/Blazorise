#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the Red color along with its color shades.
    /// </summary>
    public record ThemeColorRed : ThemeColor
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ThemeColorShade _50 { get; } = new( "50", "_50", "#ffebee" );
        public ThemeColorShade _100 { get; } = new( "100", "_100", "#ffcdd2" );
        public ThemeColorShade _200 { get; } = new( "200", "_200", "#ef9a9a" );
        public ThemeColorShade _300 { get; } = new( "300", "_300", "#e57373" );
        public ThemeColorShade _400 { get; } = new( "400", "_400", "#ef5350" );
        public ThemeColorShade _500 { get; } = new( "500", "_500", "#f44336" );
        public ThemeColorShade _600 { get; } = new( "600", "_600", "#e53935" );
        public ThemeColorShade _700 { get; } = new( "700", "_700", "#d32f2f" );
        public ThemeColorShade _800 { get; } = new( "800", "_800", "#c62828" );
        public ThemeColorShade _900 { get; } = new( "900", "_900", "#b71c1c" );
        public ThemeColorShade A100 { get; } = new( "A100", "A100", "#ff8a80" );
        public ThemeColorShade A200 { get; } = new( "A200", "A200", "#ff5252" );
        public ThemeColorShade A400 { get; } = new( "A400", "A400", "#ff1744" );
        public ThemeColorShade A700 { get; } = new( "A700", "A700", "#d50000" );
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// A default <see cref="ThemeColorRed"/> constructor
        /// </summary>
        public ThemeColorRed()
            : base( "red", "Red" )
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
