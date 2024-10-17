#region Using directives
using System;
using System.Linq.Expressions;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Helper class for running math calculations on generic types.
/// </summary>
/// <typeparam name="T">Type-parameter of the values being calculated.</typeparam>
public static class MathUtils<T>
{
    private static readonly Lazy<Func<T, T, T>> addFunc = new( AddImpl );

    private static readonly Lazy<Func<T, T, T>> subtractFunc = new( SubtractImpl );

    private static T AddImpl( T a, T b )
    {
        var paramA = Expression.Parameter( typeof( T ), "a" );
        var paramB = Expression.Parameter( typeof( T ), "b" );

        BinaryExpression body = Expression.Add( paramA, paramB );

        Func<T, T, T> add = Expression.Lambda<Func<T, T, T>>( body, paramA, paramB ).Compile();

        return add( a, b );
    }

    private static T SubtractImpl( T a, T b )
    {
        var paramA = Expression.Parameter( typeof( T ), "a" );
        var paramB = Expression.Parameter( typeof( T ), "b" );

        BinaryExpression body = Expression.Subtract( paramA, paramB );

        Func<T, T, T> subtract = Expression.Lambda<Func<T, T, T>>( body, paramA, paramB ).Compile();

        return subtract( a, b );
    }

    /// <summary>
    /// Adds two numbers together and returns the result.
    /// </summary>
    /// <param name="a">First number.</param>
    /// <param name="b">Second number.</param>
    /// <returns>Sum of two numbers.</returns>
    public static T Add( T a, T b )
    {
        return addFunc.Value( a, b );
    }

    /// <summary>
    /// Subtract two numbers and returns the result.
    /// </summary>
    /// <param name="a">First number.</param>
    /// <param name="b">Second number.</param>
    /// <returns>Subtraction of two numbers.</returns>
    public static T Subtract( T a, T b )
    {
        return subtractFunc.Value( a, b );
    }
}

/// <summary>
/// Helper class for running math calculations on known types.
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// Rounds a double value to the nearest integer or to a specified number of decimal places.
    /// </summary>
    /// <param name="value">The value to round.</param>
    /// <param name="precision">The number of decimal places to round to. If not specified, the value will be rounded to the nearest integer.</param>
    /// <returns>The rounded value.</returns>
    public static double Round( double value, int precision = 0 )
    {
        var multiplier = Math.Pow( 10, precision );
        return Math.Round( value * multiplier ) / multiplier;
    }

    /// <summary>
    /// Returns value clamped to the inclusive range of min and max.
    /// </summary>
    /// <typeparam name="TValue">Type-parameter of the values being calculated.</typeparam>
    /// <param name="value">The value to be clamped.</param>
    /// <param name="min">The upper bound of the result.</param>
    /// <param name="max">The lower bound of the result.</param>
    /// <returns>Returns value clamped to the inclusive range of min and max.</returns>
    public static TValue Clamp<TValue>( TValue value, TValue min, TValue max )
    {
        TValue result = value;

        if ( value is IComparable v )
        {
            if ( v.CompareTo( min ) < 0 )
                result = min;

            if ( v.CompareTo( max ) > 0 )
                result = max;
        }

        return result;
    }

    /// <summary>
    /// Calculates the percentage of provided number in the range of min and max.
    /// </summary>
    /// <remarks>
    /// Note: internally numbers will be converted to <c>double</c> type.
    /// </remarks>
    /// <param name="value">The value to be calculated.</param>
    /// <param name="min">The upper bound of the result.</param>
    /// <param name="max">The lower bound of the result.</param>
    /// <returns>Returns the percentage of provided number in the range of min and max.</returns>
    public static object GetPercent( object value, object min, object max )
    {
        double valueD = Converters.ChangeType<double>( value );
        double minD = Converters.ChangeType<double>( min );
        double maxD = Converters.ChangeType<double>( max );

        return maxD == minD ? 0 : ( ( valueD - minD ) / ( maxD - minD ) ) * 100;
    }
}