using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blazorise.Splitter;

/// <summary>
/// Represents either a single number or an array of numbers.
/// </summary>
[JsonConverter( typeof( JavascriptNumberOrArrayJsonConverter ) )]
public class JavascriptNumberOrArray : SingleValueOrArray<JavascriptNumber>
{
    /// <summary>
    /// Creates a <see cref="JavascriptNumberOrArray"/> from any array of <see cref="JavascriptNumber"/>.
    /// </summary>
    /// <param name="values">Array of <see cref="JavascriptNumber"/></param>
    public JavascriptNumberOrArray( params JavascriptNumber[] values ) : base( values )
    {
    }

    /// <summary>
    /// Creates a <see cref="JavascriptNumberOrArray"/> from any enumerable of <see cref="JavascriptNumber"/>.
    /// </summary>
    /// <param name="values">Enumerable of <see cref="JavascriptNumber"/></param>
    public JavascriptNumberOrArray( IEnumerable<JavascriptNumber> values ) : base( values )
    {
    }

    /// <summary>
    /// Creates a <see cref="JavascriptNumberOrArray"/> from a single <see cref="JavascriptNumber"/> value.
    /// </summary>
    /// <param name="value">A single <see cref="JavascriptNumber"/>.</param>
    public JavascriptNumberOrArray( JavascriptNumber value ) : base( value )
    {
    }

    /// <summary>
    /// Implicitly converts an array of <see cref="JavascriptNumber"/> to a <see cref="JavascriptNumberOrArray"/>.
    /// </summary>
    /// <param name="values">Array of <see cref="JavascriptNumber"/>.</param>
    /// <returns>A <see cref="JavascriptNumberOrArray"/>.</returns>
    public static implicit operator JavascriptNumberOrArray( JavascriptNumber[] values )
    {
        return new JavascriptNumberOrArray( values );
    }

    /// <summary>
    /// Implicitly converts a list of <see cref="JavascriptNumber"/> to a <see cref="JavascriptNumberOrArray"/>.
    /// </summary>
    /// <param name="values">List of <see cref="JavascriptNumber"/>.</param>
    /// <returns>A <see cref="JavascriptNumberOrArray"/>.</returns>
    public static implicit operator JavascriptNumberOrArray( List<JavascriptNumber> values )
    {
        return new JavascriptNumberOrArray( values );
    }

    /// <summary>
    /// Implicitly converts a single <see cref="JavascriptNumber"/> to a <see cref="JavascriptNumberOrArray"/>.
    /// </summary>
    /// <param name="value">Single <see cref="JavascriptNumber"/> value.</param>
    /// <returns>A <see cref="JavascriptNumberOrArray"/>.</returns>
    public static implicit operator JavascriptNumberOrArray( JavascriptNumber value )
    {
        return new JavascriptNumberOrArray( value );
    }

    /// <summary>
    /// Implicitly converts a single integer value to a <see cref="JavascriptNumberOrArray"/>.
    /// </summary>
    /// <param name="value">Single integer value.</param>
    /// <returns>A <see cref="JavascriptNumberOrArray"/>.</returns>
    public static implicit operator JavascriptNumberOrArray( int value )
    {
        return new JavascriptNumberOrArray( value );
    }

    /// <summary>
    /// Implicitly converts a single double value to a <see cref="JavascriptNumberOrArray"/>.
    /// </summary>
    /// <param name="value">Single double value.</param>
    /// <returns>A <see cref="JavascriptNumberOrArray"/>.</returns>
    public static implicit operator JavascriptNumberOrArray( double value )
    {
        return new JavascriptNumberOrArray( value );
    }
}