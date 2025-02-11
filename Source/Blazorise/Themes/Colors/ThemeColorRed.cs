#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Defines the Red color along with its color shades.
/// </summary>
public record ThemeColorRed : ThemeColor
{
    /// <summary>
    /// The lightest shade of the red theme color.
    /// </summary>
    public ThemeColorShade _50 { get; } = new( "50", "_50", "#ffebee" );

    /// <summary>
    /// A very light shade of the red theme color.
    /// </summary>
    public ThemeColorShade _100 { get; } = new( "100", "_100", "#ffcdd2" );

    /// <summary>
    /// A light shade of the red theme color.
    /// </summary>
    public ThemeColorShade _200 { get; } = new( "200", "_200", "#ef9a9a" );

    /// <summary>
    /// A medium-light shade of the red theme color.
    /// </summary>
    public ThemeColorShade _300 { get; } = new( "300", "_300", "#e57373" );

    /// <summary>
    /// A medium shade of the red theme color.
    /// </summary>
    public ThemeColorShade _400 { get; } = new( "400", "_400", "#ef5350" );

    /// <summary>
    /// The base red theme color shade.
    /// </summary>
    public ThemeColorShade _500 { get; } = new( "500", "_500", "#f44336" );

    /// <summary>
    /// A slightly darker shade of the red theme color.
    /// </summary>
    public ThemeColorShade _600 { get; } = new( "600", "_600", "#e53935" );

    /// <summary>
    /// A dark shade of the red theme color.
    /// </summary>
    public ThemeColorShade _700 { get; } = new( "700", "_700", "#d32f2f" );

    /// <summary>
    /// A very dark shade of the red theme color.
    /// </summary>
    public ThemeColorShade _800 { get; } = new( "800", "_800", "#c62828" );

    /// <summary>
    /// The darkest shade of the red theme color.
    /// </summary>
    public ThemeColorShade _900 { get; } = new( "900", "_900", "#b71c1c" );

    /// <summary>
    /// A bright variant of the red theme color.
    /// </summary>
    public ThemeColorShade A100 { get; } = new( "A100", "A100", "#ff8a80" );

    /// <summary>
    /// A lighter accent shade of the red theme color.
    /// </summary>
    public ThemeColorShade A200 { get; } = new( "A200", "A200", "#ff5252" );

    /// <summary>
    /// A strong accent shade of the red theme color.
    /// </summary>
    public ThemeColorShade A400 { get; } = new( "A400", "A400", "#ff1744" );

    /// <summary>
    /// The darkest accent shade of the red theme color.
    /// </summary>
    public ThemeColorShade A700 { get; } = new( "A700", "A700", "#d50000" );

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