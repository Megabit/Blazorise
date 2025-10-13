namespace Blazorise;

/// <summary>
/// Predefined set of contextual background colors.
/// </summary>
public record Background : Enumeration<Background>
{
    #region Constructors

    /// <inheritdoc/>
    public Background( string name ) : base( name )
    {
    }

    /// <inheritdoc/>
    private Background( Background parent, string name ) : base( parent, name )
    {
    }

    #endregion

    #region Operators

    /// <summary>
    /// Creates the new custom background color based on the supplied enum value.
    /// </summary>
    /// <param name="name">Name value of the enum.</param>
    public static implicit operator Background( string name )
    {
        return new Background( name );
    }

    #endregion

    #region Properties

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

    /// <summary>
    /// Subtle variant of the current background (e.g., "success subtle"). 
    /// Returning <see cref="BackgroundVariant"/> prevents chaining like ".Subtle.Success".
    /// </summary>
    public BackgroundVariant Subtle => new( new Background( this, "subtle" ) );

    #endregion
}

/// <summary>
/// A wrapper returned by variant properties to prevent further chaining in IntelliSense.
/// Implicitly converts back to <see cref="Background"/> so it can be assigned wherever a Background is expected.
/// </summary>
public readonly struct BackgroundVariant
{
    internal BackgroundVariant( Background background ) => Value = background;

    internal Background Value { get; }

    /// <summary>
    /// Implicitly converts a <see cref="BackgroundVariant"/> to its corresponding <see cref="Background"/> value.
    /// </summary>
    /// <param name="backgroundVariant">The <see cref="BackgroundVariant"/> instance to convert.</param>
    public static implicit operator Background( BackgroundVariant backgroundVariant ) => backgroundVariant.Value;

    /// <summary>
    /// Returns a string representation of the current object.
    /// </summary>
    public override string ToString() => Value?.ToString() ?? string.Empty;
}