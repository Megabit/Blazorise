#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Defines the Brown color along with its color shades.
/// </summary>
public record ThemeColorBrown : ThemeColor
{
    /// <summary>
    /// The lightest shade of the brown theme color.
    /// </summary>
    public ThemeColorShade _50 { get; } = new( "50", "_50", "#efebe9" );

    /// <summary>
    /// A very light shade of the brown theme color.
    /// </summary>
    public ThemeColorShade _100 { get; } = new( "100", "_100", "#d7ccc8" );

    /// <summary>
    /// A light shade of the brown theme color.
    /// </summary>
    public ThemeColorShade _200 { get; } = new( "200", "_200", "#bcaaa4" );

    /// <summary>
    /// A medium-light shade of the brown theme color.
    /// </summary>
    public ThemeColorShade _300 { get; } = new( "300", "_300", "#a1887f" );

    /// <summary>
    /// A medium shade of the brown theme color.
    /// </summary>
    public ThemeColorShade _400 { get; } = new( "400", "_400", "#8d6e63" );

    /// <summary>
    /// The base brown theme color shade.
    /// </summary>
    public ThemeColorShade _500 { get; } = new( "500", "_500", "#795548" );

    /// <summary>
    /// A slightly darker shade of the brown theme color.
    /// </summary>
    public ThemeColorShade _600 { get; } = new( "600", "_600", "#6d4c41" );

    /// <summary>
    /// A dark shade of the brown theme color.
    /// </summary>
    public ThemeColorShade _700 { get; } = new( "700", "_700", "#5d4037" );

    /// <summary>
    /// A very dark shade of the brown theme color.
    /// </summary>
    public ThemeColorShade _800 { get; } = new( "800", "_800", "#4e342e" );

    /// <summary>
    /// The darkest shade of the brown theme color.
    /// </summary>
    public ThemeColorShade _900 { get; } = new( "900", "_900", "#3e2723" );

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