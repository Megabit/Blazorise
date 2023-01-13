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
}