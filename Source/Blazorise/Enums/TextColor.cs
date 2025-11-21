namespace Blazorise;

/// <summary>
/// Predefined set of contextual text colors.
/// </summary>
public record TextColor : Enumeration<TextColor>
{
    #region Members

    #endregion

    #region Constructors

    /// <inheritdoc/>
    public TextColor( string name ) : base( name )
    {
    }

    /// <inheritdoc/>
    private TextColor( TextColor parent, string name ) : base( parent, name )
    {
    }

    #endregion

    #region Operators

    /// <summary>
    /// Creates the new custom text color based on the supplied enum value.
    /// </summary>
    /// <param name="name">Name value of the enum.</param>
    public static implicit operator TextColor( string name )
    {
        return new TextColor( name );
    }

    #endregion

    #region Properties

    /// <summary>
    /// No color will be applied to an element, meaning it will appear as default to whatever current theme is set to.
    /// </summary>
    public static readonly TextColor Default = new( (string)null );

    /// <summary>
    /// Primary color.
    /// </summary>
    public static readonly TextColor Primary = new( "primary" );

    /// <summary>
    /// Secondary color.
    /// </summary>
    public static readonly TextColor Secondary = new( "secondary" );

    /// <summary>
    /// Success color.
    /// </summary>
    public static readonly TextColor Success = new( "success" );

    /// <summary>
    /// Danger color.
    /// </summary>
    public static readonly TextColor Danger = new( "danger" );

    /// <summary>
    /// Warning color.
    /// </summary>
    public static readonly TextColor Warning = new( "warning" );

    /// <summary>
    /// Info color.
    /// </summary>
    public static readonly TextColor Info = new( "info" );

    /// <summary>
    /// Light color.
    /// </summary>
    public static readonly TextColor Light = new( "light" );

    /// <summary>
    /// Dark color.
    /// </summary>
    public static readonly TextColor Dark = new( "dark" );

    /// <summary>
    /// Body color.
    /// </summary>
    public static readonly TextColor Body = new( "body" );

    /// <summary>
    /// Muted color.
    /// </summary>
    public static readonly TextColor Muted = new( "muted" );

    /// <summary>
    /// White color.
    /// </summary>
    public static readonly TextColor White = new( "white" );

    /// <summary>
    /// Black text with 50% opacity on white background.
    /// </summary>
    public static readonly TextColor Black50 = new( "black-50" );

    /// <summary>
    /// White text with 50% opacity on black background.
    /// </summary>
    public static readonly TextColor White50 = new( "white-50" );

    /// <summary>
    /// Emphasized text color, used to emphasize certain parts of the text.
    /// </summary>
    public TextColor Emphasis => new( new TextColor( this, "emphasis" ) );

    #endregion
}

/// <summary>
/// A wrapper returned by variant properties to prevent further chaining in IntelliSense.
/// Implicitly converts back to <see cref="TextColor"/> so it can be assigned wherever a TextColor is expected.
/// </summary>
public readonly struct TextColorVariant
{
    internal TextColorVariant( TextColor textColor ) => Value = textColor;

    internal TextColor Value { get; }

    /// <summary>
    /// Implicitly converts the <see cref="TextColorVariant"/> back to <see cref="TextColor"/>.
    /// </summary>
    /// <param name="textColorVariant">The <see cref="TextColorVariant"/> instance to convert.</param>
    public static implicit operator TextColor( TextColorVariant textColorVariant ) => textColorVariant.Value;

    /// <summary>
    /// Returns a string representation of the current object.
    /// </summary>
    public override string ToString() => Value?.ToString() ?? string.Empty;
}