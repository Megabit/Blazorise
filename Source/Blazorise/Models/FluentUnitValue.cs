#region Using directives
using System.Globalization;
#endregion

namespace Blazorise;

/// <summary>
/// Represents numeric CSS value with a unit, for example <c>320px</c> or <c>24rem</c>.
/// </summary>
public class FluentUnitValue
{
    /// <summary>
    /// Creates a new empty <see cref="FluentUnitValue"/>.
    /// </summary>
    public FluentUnitValue()
    {
    }

    /// <summary>
    /// Creates a new <see cref="FluentUnitValue"/> with the supplied unit and value.
    /// </summary>
    /// <param name="unit">CSS unit.</param>
    /// <param name="value">Numeric value.</param>
    public FluentUnitValue( string unit, double? value )
    {
        Unit = unit;
        Value = value;
    }

    /// <summary>
    /// Gets or sets CSS unit (for example: <c>px</c>, <c>rem</c>, <c>em</c>, <c>ch</c>, <c>vw</c>, <c>%</c>).
    /// </summary>
    public string Unit { get; set; }

    /// <summary>
    /// Gets or sets numeric value.
    /// </summary>
    public double? Value { get; set; }

    /// <summary>
    /// Gets whether both <see cref="Unit"/> and <see cref="Value"/> are assigned.
    /// </summary>
    public bool HasValue
        => !string.IsNullOrWhiteSpace( Unit ) && Value.HasValue;

    /// <summary>
    /// Creates a copy of current value.
    /// </summary>
    /// <returns>Copied value.</returns>
    public FluentUnitValue Clone()
        => new( Unit, Value );

    /// <summary>
    /// Converts this value to raw CSS text, for example <c>320px</c>.
    /// </summary>
    /// <returns>CSS value text or empty string when value is not assigned.</returns>
    public string ToCssValue()
    {
        if ( !HasValue )
            return string.Empty;

        return $"{Value.Value.ToString( "G29", CultureInfo.InvariantCulture )}{Unit.Trim()}";
    }

    /// <summary>
    /// Converts this value to fluent sizing object.
    /// </summary>
    /// <param name="sizingType">Sizing type.</param>
    /// <returns>Fluent sizing object or null when current value is not assigned.</returns>
    public IFluentSizing ToFluentSizing( SizingType sizingType )
    {
        if ( !HasValue )
            return null;

        var unit = Unit.Trim();
        var unitLower = unit.ToLowerInvariant();
        var value = Value.Value;

        if ( sizingType == SizingType.Width )
        {
            return unitLower switch
            {
                "px" => Width.Px( value ),
                "rem" => Width.Rem( value ),
                "em" => Width.Em( value ),
                "ch" => Width.Ch( value ),
                "vw" => Width.Vw( value ),
                _ => new FluentSizing( SizingType.Width ).WithSize( unit, value ),
            };
        }

        return unitLower switch
        {
            "px" => Height.Px( value ),
            "rem" => Height.Rem( value ),
            "em" => Height.Em( value ),
            "ch" => Height.Ch( value ),
            "vh" => Height.Vh( value ),
            _ => new FluentSizing( SizingType.Height ).WithSize( unit, value ),
        };
    }
}