#region Using directives
using System.Globalization;
using System.Numerics;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Represents a numeric CSS value that can be applied to compatible fluent utility parameters.
/// </summary>
public sealed class FluentCssValue :
    IFluentSizingStyle,
    IFluentGap,
    IFluentTextSize,
    IUtilityTargeted
{
    #region Members

    private double? minValue;

    private double? maxValue;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a numeric CSS value with the supplied unit and value.
    /// </summary>
    /// <param name="unit">CSS unit.</param>
    /// <param name="value">Numeric value.</param>
    public FluentCssValue( string unit, double value )
    {
        Unit = unit;
        Value = value;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public string Class( IClassProvider classProvider )
        => null;

    /// <inheritdoc/>
    public string Style( IStyleProvider styleProvider )
        => Style( "width", true );

    /// <summary>
    /// Builds the CSS declaration for the supplied property.
    /// </summary>
    /// <param name="propertyName">CSS property name.</param>
    /// <param name="includeMinMax">Whether minimum and maximum sizing declarations should be included.</param>
    /// <returns>Built CSS declarations.</returns>
    internal string Style( string propertyName, bool includeMinMax = false )
    {
        void BuildStyles( StyleBuilder builder )
        {
            builder.Append( $"{propertyName}: {ToCssValue( Value )}" );

            if ( includeMinMax )
            {
                if ( minValue.HasValue )
                    builder.Append( $"min-{propertyName}: {ToCssValue( minValue.Value )}" );

                if ( maxValue.HasValue )
                    builder.Append( $"max-{propertyName}: {ToCssValue( maxValue.Value )}" );
            }
        }

        return new StyleBuilder( BuildStyles ).Styles;
    }

    /// <inheritdoc/>
    public IFluentSizingStyle Min( double size )
    {
        minValue = size;

        return this;
    }

    /// <inheritdoc/>
    public IFluentSizingStyle Max( double size )
    {
        maxValue = size;

        return this;
    }

    private string ToCssValue( double value )
        => $"{value.ToString( "G29", CultureInfo.InvariantCulture )}{Unit}";

    private FluentCssValue WithUtilityTarget( UtilityTarget target )
    {
        UtilityTarget = target;

        return this;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the CSS unit.
    /// </summary>
    public string Unit { get; }

    /// <summary>
    /// Gets the numeric value.
    /// </summary>
    public double Value { get; }

    /// <inheritdoc/>
    public double? FixedSize => Value;

    /// <inheritdoc/>
    public UtilityTarget? UtilityTarget { get; set; }

    /// <summary>
    /// Targets the utility output to the component element.
    /// </summary>
    public FluentCssValue OnSelf => WithUtilityTarget( Blazorise.UtilityTarget.Self );

    /// <summary>
    /// Targets the utility output to a wrapper element.
    /// </summary>
    public FluentCssValue OnWrapper => WithUtilityTarget( Blazorise.UtilityTarget.Wrapper );

    IFluentSizing IFluentUtilityTarget<IFluentSizing>.OnSelf => OnSelf;

    IFluentSizing IFluentUtilityTarget<IFluentSizing>.OnWrapper => OnWrapper;

    IFluentGap IFluentUtilityTarget<IFluentGap>.OnSelf => OnSelf;

    IFluentGap IFluentUtilityTarget<IFluentGap>.OnWrapper => OnWrapper;

    IFluentTextSize IFluentUtilityTarget<IFluentTextSize>.OnSelf => OnSelf;

    IFluentTextSize IFluentUtilityTarget<IFluentTextSize>.OnWrapper => OnWrapper;

    #endregion
}

/// <summary>
/// Provides numeric shorthand extensions for values accepted by compatible fluent utility parameters.
/// </summary>
public static class FluentCssValueExtensions
{
    /// <summary>
    /// Defines a size in pixels (1px = 1/96th of 1in).
    /// </summary>
    /// <typeparam name="TNumber">Numeric value type.</typeparam>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="FluentCssValue"/> reference.</returns>
    public static FluentCssValue Px<TNumber>( this TNumber size )
        where TNumber : INumber<TNumber>
        => WithUnit( size, "px" );

    /// <summary>
    /// Defines a size relative to the font-size of the root element.
    /// </summary>
    /// <typeparam name="TNumber">Numeric value type.</typeparam>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="FluentCssValue"/> reference.</returns>
    public static FluentCssValue Rem<TNumber>( this TNumber size )
        where TNumber : INumber<TNumber>
        => WithUnit( size, "rem" );

    /// <summary>
    /// Defines a size relative to the font-size of the element.
    /// </summary>
    /// <typeparam name="TNumber">Numeric value type.</typeparam>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="FluentCssValue"/> reference.</returns>
    public static FluentCssValue Em<TNumber>( this TNumber size )
        where TNumber : INumber<TNumber>
        => WithUnit( size, "em" );

    /// <summary>
    /// Defines a size relative to the advance measure of the glyph "0" in the element's font.
    /// </summary>
    /// <typeparam name="TNumber">Numeric value type.</typeparam>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="FluentCssValue"/> reference.</returns>
    public static FluentCssValue Ch<TNumber>( this TNumber size )
        where TNumber : INumber<TNumber>
        => WithUnit( size, "ch" );

    /// <summary>
    /// Defines a size relative to the viewport's width.
    /// </summary>
    /// <typeparam name="TNumber">Numeric value type.</typeparam>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="FluentCssValue"/> reference.</returns>
    public static FluentCssValue Vw<TNumber>( this TNumber size )
        where TNumber : INumber<TNumber>
        => WithUnit( size, "vw" );

    /// <summary>
    /// Defines a size relative to the viewport's height.
    /// </summary>
    /// <typeparam name="TNumber">Numeric value type.</typeparam>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="FluentCssValue"/> reference.</returns>
    public static FluentCssValue Vh<TNumber>( this TNumber size )
        where TNumber : INumber<TNumber>
        => WithUnit( size, "vh" );

    /// <summary>
    /// Defines a percentage size relative to the containing block.
    /// </summary>
    /// <typeparam name="TNumber">Numeric value type.</typeparam>
    /// <param name="size">Size value.</param>
    /// <returns>Returns the <see cref="FluentCssValue"/> reference.</returns>
    public static FluentCssValue Percent<TNumber>( this TNumber size )
        where TNumber : INumber<TNumber>
        => WithUnit( size, "%" );

    /// <summary>
    /// Creates a context-neutral fluent CSS value with the supplied unit.
    /// </summary>
    /// <typeparam name="TNumber">Numeric value type.</typeparam>
    /// <param name="size">Size value.</param>
    /// <param name="unit">CSS unit.</param>
    /// <returns>Returns the <see cref="FluentCssValue"/> reference.</returns>
    private static FluentCssValue WithUnit<TNumber>( TNumber size, string unit )
        where TNumber : INumber<TNumber>
        => new( unit, double.CreateChecked( size ) );
}