#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Defines the Blue-Gray color along with its color shades.
/// </summary>
public record ThemeColorBlueGray : ThemeColor
{
    /// <summary>
    /// The lightest shade of the blue-gray theme color.
    /// </summary>
    public ThemeColorShade _50 { get; } = new( "50", "_50", "#eceff1" );

    /// <summary>
    /// A very light shade of the blue-gray theme color.
    /// </summary>
    public ThemeColorShade _100 { get; } = new( "100", "_100", "#cfd8dc" );

    /// <summary>
    /// A light shade of the blue-gray theme color.
    /// </summary>
    public ThemeColorShade _200 { get; } = new( "200", "_200", "#b0bec5" );

    /// <summary>
    /// A medium-light shade of the blue-gray theme color.
    /// </summary>
    public ThemeColorShade _300 { get; } = new( "300", "_300", "#90a4ae" );

    /// <summary>
    /// A medium shade of the blue-gray theme color.
    /// </summary>
    public ThemeColorShade _400 { get; } = new( "400", "_400", "#78909c" );

    /// <summary>
    /// The base blue-gray theme color shade.
    /// </summary>
    public ThemeColorShade _500 { get; } = new( "500", "_500", "#607d8b" );

    /// <summary>
    /// A slightly darker shade of the blue-gray theme color.
    /// </summary>
    public ThemeColorShade _600 { get; } = new( "600", "_600", "#546e7a" );

    /// <summary>
    /// A dark shade of the blue-gray theme color.
    /// </summary>
    public ThemeColorShade _700 { get; } = new( "700", "_700", "#455a64" );

    /// <summary>
    /// A very dark shade of the blue-gray theme color.
    /// </summary>
    public ThemeColorShade _800 { get; } = new( "800", "_800", "#37474f" );

    /// <summary>
    /// The darkest shade of the blue-gray theme color.
    /// </summary>
    public ThemeColorShade _900 { get; } = new( "900", "_900", "#263238" );

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