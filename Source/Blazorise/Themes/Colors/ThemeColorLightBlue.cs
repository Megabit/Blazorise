#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Defines the Light-Blue color along with its color shades.
/// </summary>
public record ThemeColorLightBlue : ThemeColor
{
    /// <summary>
    /// The lightest shade of the light blue theme color.
    /// </summary>
    public ThemeColorShade _50 { get; } = new( "50", "_50", "#e1f5fe" );

    /// <summary>
    /// A very light shade of the light blue theme color.
    /// </summary>
    public ThemeColorShade _100 { get; } = new( "100", "_100", "#b3e5fc" );

    /// <summary>
    /// A light shade of the light blue theme color.
    /// </summary>
    public ThemeColorShade _200 { get; } = new( "200", "_200", "#81d4fa" );

    /// <summary>
    /// A medium-light shade of the light blue theme color.
    /// </summary>
    public ThemeColorShade _300 { get; } = new( "300", "_300", "#4fc3f7" );

    /// <summary>
    /// A medium shade of the light blue theme color.
    /// </summary>
    public ThemeColorShade _400 { get; } = new( "400", "_400", "#29b6f6" );

    /// <summary>
    /// The base light blue theme color shade.
    /// </summary>
    public ThemeColorShade _500 { get; } = new( "500", "_500", "#03a9f4" );

    /// <summary>
    /// A slightly darker shade of the light blue theme color.
    /// </summary>
    public ThemeColorShade _600 { get; } = new( "600", "_600", "#039be5" );

    /// <summary>
    /// A dark shade of the light blue theme color.
    /// </summary>
    public ThemeColorShade _700 { get; } = new( "700", "_700", "#0288d1" );

    /// <summary>
    /// A very dark shade of the light blue theme color.
    /// </summary>
    public ThemeColorShade _800 { get; } = new( "800", "_800", "#0277bd" );

    /// <summary>
    /// The darkest shade of the light blue theme color.
    /// </summary>
    public ThemeColorShade _900 { get; } = new( "900", "_900", "#01579b" );

    /// <summary>
    /// A bright variant of the light blue theme color.
    /// </summary>
    public ThemeColorShade A100 { get; } = new( "A100", "A100", "#80d8ff" );

    /// <summary>
    /// A lighter accent shade of the light blue theme color.
    /// </summary>
    public ThemeColorShade A200 { get; } = new( "A200", "A200", "#40c4ff" );

    /// <summary>
    /// A strong accent shade of the light blue theme color.
    /// </summary>
    public ThemeColorShade A400 { get; } = new( "A400", "A400", "#00b0ff" );

    /// <summary>
    /// The darkest accent shade of the light blue theme color.
    /// </summary>
    public ThemeColorShade A700 { get; } = new( "A700", "A700", "#0091ea" );


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