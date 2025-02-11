#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Defines the Green color along with its color shades.
/// </summary>
public record ThemeColorGreen : ThemeColor
{
    /// <summary>
    /// The lightest shade of the green theme color.
    /// </summary>
    public ThemeColorShade _50 { get; } = new( "50", "_50", "#e8f5e9" );

    /// <summary>
    /// A very light shade of the green theme color.
    /// </summary>
    public ThemeColorShade _100 { get; } = new( "100", "_100", "#c8e6c9" );

    /// <summary>
    /// A light shade of the green theme color.
    /// </summary>
    public ThemeColorShade _200 { get; } = new( "200", "_200", "#a5d6a7" );

    /// <summary>
    /// A medium-light shade of the green theme color.
    /// </summary>
    public ThemeColorShade _300 { get; } = new( "300", "_300", "#81c784" );

    /// <summary>
    /// A medium shade of the green theme color.
    /// </summary>
    public ThemeColorShade _400 { get; } = new( "400", "_400", "#66bb6a" );

    /// <summary>
    /// The base green theme color shade.
    /// </summary>
    public ThemeColorShade _500 { get; } = new( "500", "_500", "#4caf50" );

    /// <summary>
    /// A slightly darker shade of the green theme color.
    /// </summary>
    public ThemeColorShade _600 { get; } = new( "600", "_600", "#43a047" );

    /// <summary>
    /// A dark shade of the green theme color.
    /// </summary>
    public ThemeColorShade _700 { get; } = new( "700", "_700", "#388e3c" );

    /// <summary>
    /// A very dark shade of the green theme color.
    /// </summary>
    public ThemeColorShade _800 { get; } = new( "800", "_800", "#2e7d32" );

    /// <summary>
    /// The darkest shade of the green theme color.
    /// </summary>
    public ThemeColorShade _900 { get; } = new( "900", "_900", "#1b5e20" );

    /// <summary>
    /// A bright variant of the green theme color.
    /// </summary>
    public ThemeColorShade A100 { get; } = new( "A100", "A100", "#b9f6ca" );

    /// <summary>
    /// A lighter accent shade of the green theme color.
    /// </summary>
    public ThemeColorShade A200 { get; } = new( "A200", "A200", "#69f0ae" );

    /// <summary>
    /// A strong accent shade of the green theme color.
    /// </summary>
    public ThemeColorShade A400 { get; } = new( "A400", "A400", "#00e676" );

    /// <summary>
    /// The darkest accent shade of the green theme color.
    /// </summary>
    public ThemeColorShade A700 { get; } = new( "A700", "A700", "#00c853" );

    /// <summary>
    /// A default <see cref="ThemeColorGreen"/> constructor
    /// </summary>
    public ThemeColorGreen() : base( "green", "Green" )
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