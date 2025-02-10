#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Defines the Deep-Purple color along with its color shades.
/// </summary>
public record ThemeColorDeepPurple : ThemeColor
{
    /// <summary>
    /// The lightest shade of the deep purple theme color.
    /// </summary>
    public ThemeColorShade _50 { get; } = new( "50", "_50", "#ede7f6" );

    /// <summary>
    /// A very light shade of the deep purple theme color.
    /// </summary>
    public ThemeColorShade _100 { get; } = new( "100", "_100", "#d1c4e9" );

    /// <summary>
    /// A light shade of the deep purple theme color.
    /// </summary>
    public ThemeColorShade _200 { get; } = new( "200", "_200", "#b39ddb" );

    /// <summary>
    /// A medium-light shade of the deep purple theme color.
    /// </summary>
    public ThemeColorShade _300 { get; } = new( "300", "_300", "#9575cd" );

    /// <summary>
    /// A medium shade of the deep purple theme color.
    /// </summary>
    public ThemeColorShade _400 { get; } = new( "400", "_400", "#7e57c2" );

    /// <summary>
    /// The base deep purple theme color shade.
    /// </summary>
    public ThemeColorShade _500 { get; } = new( "500", "_500", "#673ab7" );

    /// <summary>
    /// A slightly darker shade of the deep purple theme color.
    /// </summary>
    public ThemeColorShade _600 { get; } = new( "600", "_600", "#5e35b1" );

    /// <summary>
    /// A dark shade of the deep purple theme color.
    /// </summary>
    public ThemeColorShade _700 { get; } = new( "700", "_700", "#512da8" );

    /// <summary>
    /// A very dark shade of the deep purple theme color.
    /// </summary>
    public ThemeColorShade _800 { get; } = new( "800", "_800", "#4527a0" );

    /// <summary>
    /// The darkest shade of the deep purple theme color.
    /// </summary>
    public ThemeColorShade _900 { get; } = new( "900", "_900", "#311b92" );

    /// <summary>
    /// A bright variant of the deep purple theme color.
    /// </summary>
    public ThemeColorShade A100 { get; } = new( "A100", "A100", "#b388ff" );

    /// <summary>
    /// A lighter accent shade of the deep purple theme color.
    /// </summary>
    public ThemeColorShade A200 { get; } = new( "A200", "A200", "#7c4dff" );

    /// <summary>
    /// A strong accent shade of the deep purple theme color.
    /// </summary>
    public ThemeColorShade A400 { get; } = new( "A400", "A400", "#651fff" );

    /// <summary>
    /// The darkest accent shade of the deep purple theme color.
    /// </summary>
    public ThemeColorShade A700 { get; } = new( "A700", "A700", "#6200ea" );

    /// <summary>
    /// A default <see cref="ThemeColorDeepPurple"/> constructor
    /// </summary>
    public ThemeColorDeepPurple() : base( "deep-purple", "DeepPurple" )
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