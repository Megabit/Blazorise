using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Blazorise.Splitter;

/// <summary>
/// Wrapper around a <see cref="double"/> that supports things like Positive/Negative infinity and NaN for use in
/// Javascript interop
/// </summary>
[JsonConverter( typeof( JavascriptNumberJsonConverter ) )]
public sealed class JavascriptNumber : IEquatable<JavascriptNumber>
{
    /// <summary>
    /// Value representing positive infinity.
    /// </summary>
    public static readonly JavascriptNumber PositiveInfinity = double.PositiveInfinity;

    /// <summary>
    /// Value representing negative infinity.
    /// </summary>
    public static readonly JavascriptNumber NegativeInfinity = double.NegativeInfinity;

    /// <summary>
    /// Value representing a non-number.
    /// </summary>
    public static readonly JavascriptNumber NaN = double.NaN;

    /// <summary>
    /// True if the number is positive or negative infinity.
    /// </summary>
    public bool IsInfinity => double.IsInfinity( Value );

    /// <summary>
    /// True if the number is positive infinity.
    /// </summary>
    public bool IsPositiveInfinity => double.IsPositiveInfinity( Value );

    /// <summary>
    /// True if the number is negative infinity.
    /// </summary>
    public bool IsNegativeInfinity => double.IsNegativeInfinity( Value );

    /// <summary>
    /// True if this is not a number.
    /// </summary>
    public bool IsNaN => double.IsNaN( Value );

    /// <summary>
    /// Double value of the number.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Creates a new <see cref="JavascriptNumber"/> from a double value.
    /// </summary>
    /// <param name="value">The value of the number.</param>
    public JavascriptNumber( double value )
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new <see cref="JavascriptNumber"/> from an integer value.
    /// </summary>
    /// <param name="value">The value of the number.</param>
    public JavascriptNumber( int value )
    {
        Value = value;
    }

    /// <summary>
    /// Implicitly converts from an integer value to a <see cref="JavascriptNumber"/>.
    /// </summary>
    /// <param name="value">Integer value.</param>
    /// <returns>A <see cref="JavascriptNumber"/> representing the integer.</returns>
    public static implicit operator JavascriptNumber( int value )
    {
        return new JavascriptNumber( value );
    }

    /// <summary>
    /// Implicitly converts from a <see cref="JavascriptNumber"/> to an integer.
    /// </summary>
    /// <param name="number"><see cref="JavascriptNumber"/>.</param>
    /// <returns>The integer value of the number.</returns>
    public static explicit operator int( JavascriptNumber number )
    {
        return (int)number.Value;
    }

    /// <summary>
    /// Implicitly converts from an double value to a <see cref="JavascriptNumber"/>.
    /// </summary>
    /// <param name="value">Double value.</param>
    /// <returns>A <see cref="JavascriptNumber"/> representing the double.</returns>
    public static implicit operator JavascriptNumber( double value )
    {
        return new JavascriptNumber( value );
    }

    /// <summary>
    /// Implicitly converts from a <see cref="JavascriptNumber"/> to an double.
    /// </summary>
    /// <param name="number"><see cref="JavascriptNumber"/>.</param>
    /// <returns>The double value of the number.</returns>
    public static explicit operator double( JavascriptNumber number )
    {
        return number.Value;
    }

    /// <summary>
    /// Indicates whether the number is equivalent to another double value.
    /// </summary>
    /// <param name="other">Other double value.</param>
    /// <returns>True if the value of this number is equivalent to the other value.</returns>
    public bool Equals( double? other )
    {
        return Value == other;
    }

    /// <inheritdoc/>
    public bool Equals( JavascriptNumber other )
    {
        if ( ReferenceEquals( null, other ) )
            return false;
        if ( ReferenceEquals( this, other ) )
            return true;
        return IsInfinity == other.IsInfinity &&
               IsPositiveInfinity == other.IsPositiveInfinity &&
               IsNegativeInfinity == other.IsNegativeInfinity &&
               IsNaN == other.IsNaN &&
               Value.Equals( other.Value );
    }

    /// <inheritdoc/>
    public override bool Equals( object obj )
    {
        return ReferenceEquals( this, obj ) || obj is JavascriptNumber other && Equals( other );
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// Overrides the == operator to compare two <see cref="JavascriptNumber"/> for equality.
    /// </summary>
    /// <param name="left">First <see cref="JavascriptNumber"/> to compare.</param>
    /// <param name="right">Second <see cref="JavascriptNumber"/> to compare.</param>
    /// <returns>True if the two numbers have the same value.</returns>
    public static bool operator ==( JavascriptNumber left, JavascriptNumber right )
    {
        return Equals( left, right );
    }

    /// <summary>
    /// Overrides the != operator to compare two <see cref="JavascriptNumber"/> for non-equality.
    /// </summary>
    /// <param name="left">First <see cref="JavascriptNumber"/> to compare.</param>
    /// <param name="right">Second <see cref="JavascriptNumber"/> to compare.</param>
    /// <returns>True if the two numbers do not have the same value.</returns>
    public static bool operator !=( JavascriptNumber left, JavascriptNumber right )
    {
        return !Equals( left, right );
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Value.ToString( CultureInfo.InvariantCulture );
    }
}