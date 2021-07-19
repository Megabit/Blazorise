#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the Amber color along with its color shades.
    /// </summary>
    public record ThemeColorAmber : ThemeColor
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ThemeColorShade _50 { get; } = new( "50", "_50", "#fff8e1" );
        public ThemeColorShade _100 { get; } = new( "100", "_100", "#ffecb3" );
        public ThemeColorShade _200 { get; } = new( "200", "_200", "#ffe082" );
        public ThemeColorShade _300 { get; } = new( "300", "_300", "#ffd54f" );
        public ThemeColorShade _400 { get; } = new( "400", "_400", "#ffca28" );
        public ThemeColorShade _500 { get; } = new( "500", "_500", "#ffc107" );
        public ThemeColorShade _600 { get; } = new( "600", "_600", "#ffb300" );
        public ThemeColorShade _700 { get; } = new( "700", "_700", "#ffa000" );
        public ThemeColorShade _800 { get; } = new( "800", "_800", "#ff8f00" );
        public ThemeColorShade _900 { get; } = new( "900", "_900", "#ff6f00" );
        public ThemeColorShade A100 { get; } = new( "A100", "A100", "#ffe57f" );
        public ThemeColorShade A200 { get; } = new( "A200", "A200", "#ffd740" );
        public ThemeColorShade A400 { get; } = new( "A400", "A400", "#ffc400" );
        public ThemeColorShade A700 { get; } = new( "A700", "A700", "#ffab00" );
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// A default <see cref="ThemeColorAmber"/> constructor
        /// </summary>
        public ThemeColorAmber() : base( "amber", "Amber" )
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
