#region Using directives
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines the Light-Blue color along with its color shades.
    /// </summary>
    public record ThemeColorLightBlue : ThemeColor
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public ThemeColorShade _50 { get; } = new( "50", "_50", "#e1f5fe" );
        public ThemeColorShade _100 { get; } = new( "100", "_100", "#b3e5fc" );
        public ThemeColorShade _200 { get; } = new( "200", "_200", "#81d4fa" );
        public ThemeColorShade _300 { get; } = new( "300", "_300", "#4fc3f7" );
        public ThemeColorShade _400 { get; } = new( "400", "_400", "#29b6f6" );
        public ThemeColorShade _500 { get; } = new( "500", "_500", "#03a9f4" );
        public ThemeColorShade _600 { get; } = new( "600", "_600", "#039be5" );
        public ThemeColorShade _700 { get; } = new( "700", "_700", "#0288d1" );
        public ThemeColorShade _800 { get; } = new( "800", "_800", "#0277bd" );
        public ThemeColorShade _900 { get; } = new( "900", "_900", "#01579b" );
        public ThemeColorShade A100 { get; } = new( "A100", "A100", "#80d8ff" );
        public ThemeColorShade A200 { get; } = new( "A200", "A200", "#40c4ff" );
        public ThemeColorShade A400 { get; } = new( "A400", "A400", "#00b0ff" );
        public ThemeColorShade A700 { get; } = new( "A700", "A700", "#0091ea" );
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// A default <see cref="ThemeColorLightBlue"/> constructor
        /// </summary>
        public ThemeColorLightBlue() : base( "light-blue", "LightBlue" )
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
