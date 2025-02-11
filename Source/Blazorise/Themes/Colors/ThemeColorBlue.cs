#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Defines the Blue color along with its color shades.
/// </summary>
public record ThemeColorBlue : ThemeColor
{
    /// <summary>
    /// The lightest shade of the blue theme color.
    /// </summary>
    public ThemeColorShade _50 { get; } = new( "50", "_50", "#e3f2fd" );

    /// <summary>
    /// A very light shade of the blue theme color.
    /// </summary>
    public ThemeColorShade _100 { get; } = new( "100", "_100", "#bbdefb" );

    /// <summary>
    /// A light shade of the blue theme color.
    /// </summary>
    public ThemeColorShade _200 { get; } = new( "200", "_200", "#90caf9" );

    /// <summary>
    /// A medium-light shade of the blue theme color.
    /// </summary>
    public ThemeColorShade _300 { get; } = new( "300", "_300", "#64b5f6" );

    /// <summary>
    /// A medium shade of the blue theme color.
    /// </summary>
    public ThemeColorShade _400 { get; } = new( "400", "_400", "#42a5f5" );

    /// <summary>
    /// The base blue theme color shade.
    /// </summary>
    public ThemeColorShade _500 { get; } = new( "500", "_500", "#2196f3" );

    /// <summary>
    /// A slightly darker shade of the blue theme color.
    /// </summary>
    public ThemeColorShade _600 { get; } = new( "600", "_600", "#1e88e5" );

    /// <summary>
    /// A dark shade of the blue theme color.
    /// </summary>
    public ThemeColorShade _700 { get; } = new( "700", "_700", "#1976d2" );

    /// <summary>
    /// A very dark shade of the blue theme color.
    /// </summary>
    public ThemeColorShade _800 { get; } = new( "800", "_800", "#1565c0" );

    /// <summary>
    /// The darkest shade of the blue theme color.
    /// </summary>
    public ThemeColorShade _900 { get; } = new( "900", "_900", "#0d47a1" );

    /// <summary>
    /// A bright variant of the blue theme color.
    /// </summary>
    public ThemeColorShade A100 { get; } = new( "A100", "A100", "#82b1ff" );

    /// <summary>
    /// A lighter accent shade of the blue theme color.
    /// </summary>
    public ThemeColorShade A200 { get; } = new( "A200", "A200", "#448aff" );

    /// <summary>
    /// A strong accent shade of the blue theme color.
    /// </summary>
    public ThemeColorShade A400 { get; } = new( "A400", "A400", "#2979ff" );

    /// <summary>
    /// The darkest accent shade of the blue theme color.
    /// </summary>
    public ThemeColorShade A700 { get; } = new( "A700", "A700", "#2962ff" );


    /// <summary>
    /// A default <see cref="ThemeColorBlue"/> constructor
    /// </summary>
    public ThemeColorBlue() : base( "blue", "Blue" )
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