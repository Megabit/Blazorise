#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Defines the Gray color along with its color shades.
/// </summary>
public record ThemeColorGray : ThemeColor
{
    /// <summary>
    /// The lightest shade of the gray theme color.
    /// </summary>
    public ThemeColorShade _50 { get; } = new( "50", "_50", "#fafafa" );

    /// <summary>
    /// A very light shade of the gray theme color.
    /// </summary>
    public ThemeColorShade _100 { get; } = new( "100", "_100", "#f5f5f5" );

    /// <summary>
    /// A light shade of the gray theme color.
    /// </summary>
    public ThemeColorShade _200 { get; } = new( "200", "_200", "#eee" );

    /// <summary>
    /// A medium-light shade of the gray theme color.
    /// </summary>
    public ThemeColorShade _300 { get; } = new( "300", "_300", "#e0e0e0" );

    /// <summary>
    /// A medium shade of the gray theme color.
    /// </summary>
    public ThemeColorShade _400 { get; } = new( "400", "_400", "#bdbdbd" );

    /// <summary>
    /// The base gray theme color shade.
    /// </summary>
    public ThemeColorShade _500 { get; } = new( "500", "_500", "#9e9e9e" );

    /// <summary>
    /// A slightly darker shade of the gray theme color.
    /// </summary>
    public ThemeColorShade _600 { get; } = new( "600", "_600", "#757575" );

    /// <summary>
    /// A dark shade of the gray theme color.
    /// </summary>
    public ThemeColorShade _700 { get; } = new( "700", "_700", "#616161" );

    /// <summary>
    /// A very dark shade of the gray theme color.
    /// </summary>
    public ThemeColorShade _800 { get; } = new( "800", "_800", "#424242" );

    /// <summary>
    /// The darkest shade of the gray theme color.
    /// </summary>
    public ThemeColorShade _900 { get; } = new( "900", "_900", "#212121" );


    /// <summary>
    /// A default <see cref="ThemeColorGray"/> constructor
    /// </summary>
    public ThemeColorGray() : base( "gray", "Gray" )
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