#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Defines the Deep-Orange color along with its color shades.
/// </summary>
public record ThemeColorDeepOrange : ThemeColor
{
    /// <summary>
    /// The lightest shade of the deep orange theme color.
    /// </summary>
    public ThemeColorShade _50 { get; } = new( "50", "_50", "#fbe9e7" );

    /// <summary>
    /// A very light shade of the deep orange theme color.
    /// </summary>
    public ThemeColorShade _100 { get; } = new( "100", "_100", "#ffccbc" );

    /// <summary>
    /// A light shade of the deep orange theme color.
    /// </summary>
    public ThemeColorShade _200 { get; } = new( "200", "_200", "#ffab91" );

    /// <summary>
    /// A medium-light shade of the deep orange theme color.
    /// </summary>
    public ThemeColorShade _300 { get; } = new( "300", "_300", "#ff8a65" );

    /// <summary>
    /// A medium shade of the deep orange theme color.
    /// </summary>
    public ThemeColorShade _400 { get; } = new( "400", "_400", "#ff7043" );

    /// <summary>
    /// The base deep orange theme color shade.
    /// </summary>
    public ThemeColorShade _500 { get; } = new( "500", "_500", "#ff5722" );

    /// <summary>
    /// A slightly darker shade of the deep orange theme color.
    /// </summary>
    public ThemeColorShade _600 { get; } = new( "600", "_600", "#f4511e" );

    /// <summary>
    /// A dark shade of the deep orange theme color.
    /// </summary>
    public ThemeColorShade _700 { get; } = new( "700", "_700", "#e64a19" );

    /// <summary>
    /// A very dark shade of the deep orange theme color.
    /// </summary>
    public ThemeColorShade _800 { get; } = new( "800", "_800", "#d84315" );

    /// <summary>
    /// The darkest shade of the deep orange theme color.
    /// </summary>
    public ThemeColorShade _900 { get; } = new( "900", "_900", "#bf360c" );

    /// <summary>
    /// A bright variant of the deep orange theme color.
    /// </summary>
    public ThemeColorShade A100 { get; } = new( "A100", "A100", "#ff9e80" );

    /// <summary>
    /// A lighter accent shade of the deep orange theme color.
    /// </summary>
    public ThemeColorShade A200 { get; } = new( "A200", "A200", "#ff6e40" );

    /// <summary>
    /// A strong accent shade of the deep orange theme color.
    /// </summary>
    public ThemeColorShade A400 { get; } = new( "A400", "A400", "#ff3d00" );

    /// <summary>
    /// The darkest accent shade of the deep orange theme color.
    /// </summary>
    public ThemeColorShade A700 { get; } = new( "A700", "A700", "#dd2c00" );


    /// <summary>
    /// A default <see cref="ThemeColorDeepOrange"/> constructor
    /// </summary>
    public ThemeColorDeepOrange() : base( "deep-orange", "DeepOrange" )
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