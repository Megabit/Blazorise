namespace Blazorise;

/// <summary>
/// Predefined set of contextual background colors.
/// </summary>
public record Background : Enumeration<Background>
{
    /// <inheritdoc/>
    public Background( string name ) : base( name )
    {
    }

    /// <inheritdoc/>
    private Background( Background parent, string name ) : base( parent, name )
    {
    }

    /// <summary>
    /// Creates the new custom background color based on the supplied enum value.
    /// </summary>
    /// <param name="name">Name value of the enum.</param>
    public static implicit operator Background( string name )
    {
        return new Background( name );
    }

    /// <summary>
    /// No color will be applied to an element, meaning it will appear as default to whatever current theme is set to.
    /// </summary>
    public static readonly Background Default = new( null );

    /// <summary>
    /// Primary color.
    /// </summary>
    public static readonly Background Primary = new( "primary" );

    /// <summary>
    /// Secondary color.
    /// </summary>
    public static readonly Background Secondary = new( "secondary" );

    /// <summary>
    /// Success color.
    /// </summary>
    public static readonly Background Success = new( "success" );

    /// <summary>
    /// Danger color.
    /// </summary>
    public static readonly Background Danger = new( "danger" );

    /// <summary>
    /// Warning color.
    /// </summary>
    public static readonly Background Warning = new( "warning" );

    /// <summary>
    /// Info color.
    /// </summary>
    public static readonly Background Info = new( "info" );

    /// <summary>
    /// Light color.
    /// </summary>
    public static readonly Background Light = new( "light" );

    /// <summary>
    /// Dark color.
    /// </summary>
    public static readonly Background Dark = new( "dark" );

    /// <summary>
    /// White color.
    /// </summary>
    public static readonly Background White = new( "white" );

    /// <summary>
    /// Transparent color.
    /// </summary>
    public static readonly Background Transparent = new( "transparent" );

    /// <summary>
    /// Body color. Note that body color must be defined through the <see cref="Theme"/> options and
    /// not every provider supports it!
    /// </summary>
    public static readonly Background Body = new( "body" );
}