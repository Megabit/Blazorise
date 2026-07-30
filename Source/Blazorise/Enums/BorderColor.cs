namespace Blazorise;

/// <summary>
/// Predefined set of contextual border colors.
/// </summary>
public record struct BorderColor
{
    #region Operators

    /// <summary>
    /// Creates a custom border color based on the supplied CSS color value.
    /// </summary>
    /// <param name="name">CSS color value.</param>
    public static implicit operator BorderColor( string name )
    {
        return new BorderColor( name );
    }

    #endregion

    /// <summary>
    /// Gets the enum name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// A default target contructor.
    /// </summary>
    /// <param name="name">Named value of the enum.</param>
    public BorderColor( string name )
    {
        Name = name;
        IsCssValue = CssColor.IsValue( name );
    }

    /// <summary>
    /// Gets whether this instance represents an explicit CSS color value.
    /// </summary>
    public bool IsCssValue { get; }

    /// <summary>
    /// No color will be applied to an element.
    /// </summary>
    public static readonly BorderColor None = new( (string)null );

    /// <summary>
    /// Primary color.
    /// </summary>
    public static readonly BorderColor Primary = new( "primary" );

    /// <summary>
    /// Secondary color.
    /// </summary>
    public static readonly BorderColor Secondary = new( "secondary" );

    /// <summary>
    /// Success color.
    /// </summary>
    public static readonly BorderColor Success = new( "success" );

    /// <summary>
    /// Danger color.
    /// </summary>
    public static readonly BorderColor Danger = new( "danger" );

    /// <summary>
    /// Warning color.
    /// </summary>
    public static readonly BorderColor Warning = new( "warning" );

    /// <summary>
    /// Info color.
    /// </summary>
    public static readonly BorderColor Info = new( "info" );

    /// <summary>
    /// Light color.
    /// </summary>
    public static readonly BorderColor Light = new( "light" );

    /// <summary>
    /// Dark color.
    /// </summary>
    public static readonly BorderColor Dark = new( "dark" );

    /// <summary>
    /// White color.
    /// </summary>
    public static readonly BorderColor White = new( "white" );
}