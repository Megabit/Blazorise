#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Defines the Purple color along with its color shades.
/// </summary>
public record ThemeColorPurple : ThemeColor
{
    /// <summary>
    /// The lightest shade of the purple theme color.
    /// </summary>
    public ThemeColorShade _50 { get; } = new( "50", "_50", "#f3e5f5" );

    /// <summary>
    /// A very light shade of the purple theme color.
    /// </summary>
    public ThemeColorShade _100 { get; } = new( "100", "_100", "#e1bee7" );

    /// <summary>
    /// A light shade of the purple theme color.
    /// </summary>
    public ThemeColorShade _200 { get; } = new( "200", "_200", "#ce93d8" );

    /// <summary>
    /// A medium-light shade of the purple theme color.
    /// </summary>
    public ThemeColorShade _300 { get; } = new( "300", "_300", "#ba68c8" );

    /// <summary>
    /// A medium shade of the purple theme color.
    /// </summary>
    public ThemeColorShade _400 { get; } = new( "400", "_400", "#ab47bc" );

    /// <summary>
    /// The base purple theme color shade.
    /// </summary>
    public ThemeColorShade _500 { get; } = new( "500", "_500", "#9c27b0" );

    /// <summary>
    /// A slightly darker shade of the purple theme color.
    /// </summary>
    public ThemeColorShade _600 { get; } = new( "600", "_600", "#8e24aa" );

    /// <summary>
    /// A dark shade of the purple theme color.
    /// </summary>
    public ThemeColorShade _700 { get; } = new( "700", "_700", "#7b1fa2" );

    /// <summary>
    /// A very dark shade of the purple theme color.
    /// </summary>
    public ThemeColorShade _800 { get; } = new( "800", "_800", "#6a1b9a" );

    /// <summary>
    /// The darkest shade of the purple theme color.
    /// </summary>
    public ThemeColorShade _900 { get; } = new( "900", "_900", "#4a148c" );

    /// <summary>
    /// A bright variant of the purple theme color.
    /// </summary>
    public ThemeColorShade A100 { get; } = new( "A100", "A100", "#ea80fc" );

    /// <summary>
    /// A lighter accent shade of the purple theme color.
    /// </summary>
    public ThemeColorShade A200 { get; } = new( "A200", "A200", "#e040fb" );

    /// <summary>
    /// A strong accent shade of the purple theme color.
    /// </summary>
    public ThemeColorShade A400 { get; } = new( "A400", "A400", "#d500f9" );

    /// <summary>
    /// The darkest accent shade of the purple theme color.
    /// </summary>
    public ThemeColorShade A700 { get; } = new( "A700", "A700", "#a0f" );


    /// <summary>
    /// A default <see cref="ThemeColorPurple"/> constructor
    /// </summary>
    public ThemeColorPurple() : base( "purple", "Purple" )
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