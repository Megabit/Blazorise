#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Defines the Indigo color along with its color shades.
/// </summary>
public record ThemeColorIndigo : ThemeColor
{
    /// <summary>
    /// The lightest shade of the indigo theme color.
    /// </summary>
    public ThemeColorShade _50 { get; } = new( "50", "_50", "#e8eaf6" );

    /// <summary>
    /// A very light shade of the indigo theme color.
    /// </summary>
    public ThemeColorShade _100 { get; } = new( "100", "_100", "#c5cae9" );

    /// <summary>
    /// A light shade of the indigo theme color.
    /// </summary>
    public ThemeColorShade _200 { get; } = new( "200", "_200", "#9fa8da" );

    /// <summary>
    /// A medium-light shade of the indigo theme color.
    /// </summary>
    public ThemeColorShade _300 { get; } = new( "300", "_300", "#7986cb" );

    /// <summary>
    /// A medium shade of the indigo theme color.
    /// </summary>
    public ThemeColorShade _400 { get; } = new( "400", "_400", "#5c6bc0" );

    /// <summary>
    /// The base indigo theme color shade.
    /// </summary>
    public ThemeColorShade _500 { get; } = new( "500", "_500", "#3f51b5" );

    /// <summary>
    /// A slightly darker shade of the indigo theme color.
    /// </summary>
    public ThemeColorShade _600 { get; } = new( "600", "_600", "#3949ab" );

    /// <summary>
    /// A dark shade of the indigo theme color.
    /// </summary>
    public ThemeColorShade _700 { get; } = new( "700", "_700", "#303f9f" );

    /// <summary>
    /// A very dark shade of the indigo theme color.
    /// </summary>
    public ThemeColorShade _800 { get; } = new( "800", "_800", "#283593" );

    /// <summary>
    /// The darkest shade of the indigo theme color.
    /// </summary>
    public ThemeColorShade _900 { get; } = new( "900", "_900", "#1a237e" );

    /// <summary>
    /// A bright variant of the indigo theme color.
    /// </summary>
    public ThemeColorShade A100 { get; } = new( "A100", "A100", "#8c9eff" );

    /// <summary>
    /// A lighter accent shade of the indigo theme color.
    /// </summary>
    public ThemeColorShade A200 { get; } = new( "A200", "A200", "#536dfe" );

    /// <summary>
    /// A strong accent shade of the indigo theme color.
    /// </summary>
    public ThemeColorShade A400 { get; } = new( "A400", "A400", "#3d5afe" );

    /// <summary>
    /// The darkest accent shade of the indigo theme color.
    /// </summary>
    public ThemeColorShade A700 { get; } = new( "A700", "A700", "#304ffe" );

    /// <summary>
    /// A default <see cref="ThemeColorIndigo"/> constructor
    /// </summary>
    public ThemeColorIndigo() : base( "indigo", "Indigo" )
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