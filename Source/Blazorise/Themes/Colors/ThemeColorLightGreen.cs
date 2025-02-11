#region Using directives
#endregion

namespace Blazorise;

/// <summary>
/// Defines the Light-Green color along with its color shades.
/// </summary>
public record ThemeColorLightGreen : ThemeColor
{
    /// <summary>
    /// The lightest shade of the light green theme color.
    /// </summary>
    public ThemeColorShade _50 { get; } = new( "50", "_50", "#f1f8e9" );

    /// <summary>
    /// A very light shade of the light green theme color.
    /// </summary>
    public ThemeColorShade _100 { get; } = new( "100", "_100", "#dcedc8" );

    /// <summary>
    /// A light shade of the light green theme color.
    /// </summary>
    public ThemeColorShade _200 { get; } = new( "200", "_200", "#c5e1a5" );

    /// <summary>
    /// A medium-light shade of the light green theme color.
    /// </summary>
    public ThemeColorShade _300 { get; } = new( "300", "_300", "#aed581" );

    /// <summary>
    /// A medium shade of the light green theme color.
    /// </summary>
    public ThemeColorShade _400 { get; } = new( "400", "_400", "#9ccc65" );

    /// <summary>
    /// The base light green theme color shade.
    /// </summary>
    public ThemeColorShade _500 { get; } = new( "500", "_500", "#8bc34a" );

    /// <summary>
    /// A slightly darker shade of the light green theme color.
    /// </summary>
    public ThemeColorShade _600 { get; } = new( "600", "_600", "#7cb342" );

    /// <summary>
    /// A dark shade of the light green theme color.
    /// </summary>
    public ThemeColorShade _700 { get; } = new( "700", "_700", "#689f38" );

    /// <summary>
    /// A very dark shade of the light green theme color.
    /// </summary>
    public ThemeColorShade _800 { get; } = new( "800", "_800", "#558b2f" );

    /// <summary>
    /// The darkest shade of the light green theme color.
    /// </summary>
    public ThemeColorShade _900 { get; } = new( "900", "_900", "#33691e" );

    /// <summary>
    /// A bright variant of the light green theme color.
    /// </summary>
    public ThemeColorShade A100 { get; } = new( "A100", "A100", "#ccff90" );

    /// <summary>
    /// A lighter accent shade of the light green theme color.
    /// </summary>
    public ThemeColorShade A200 { get; } = new( "A200", "A200", "#b2ff59" );

    /// <summary>
    /// A strong accent shade of the light green theme color.
    /// </summary>
    public ThemeColorShade A400 { get; } = new( "A400", "A400", "#76ff03" );

    /// <summary>
    /// The darkest accent shade of the light green theme color.
    /// </summary>
    public ThemeColorShade A700 { get; } = new( "A700", "A700", "#64dd17" );


    /// <summary>
    /// A default <see cref="ThemeColorLightGreen"/> constructor
    /// </summary>
    public ThemeColorLightGreen() : base( "light-green", "LightGreen" )
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