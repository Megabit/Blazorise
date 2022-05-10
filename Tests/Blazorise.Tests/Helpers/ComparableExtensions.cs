using System;

namespace Blazorise.Tests.Helpers;

/// <summary>
/// A set of extension methods for working with types that implement <see cref="IComparable"/>
/// </summary>
public static class ComparableExtensions
{
    /// <summary>
    /// <paramref name="lowerBound"/>  ≤ <paramref name="value"/> ≤ <paramref name="upperBound"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="lowerBound"></param>
    /// <param name="upperBound"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsBetween<T>( this T value, T lowerBound, T upperBound ) where T : IComparable<T>
    {
        return lowerBound.CompareTo( value ) <= 0 && upperBound.CompareTo( value ) >= 0;
    }

    /// <summary>
    /// <paramref name="lowerBound"/> &lt; <paramref name="value"/> ≤ <paramref name="upperBound"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="lowerBound"></param>
    /// <param name="upperBound"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsBetweenExclusiveLowerBound<T>( this T value, T lowerBound, T upperBound ) where T : IComparable<T>
    {
        return lowerBound.CompareTo( value ) < 0 && value.CompareTo( upperBound ) <= 0;
    }

    /// <summary>
    /// <paramref name="lowerBound"/> ≤ <paramref name="value"/> &lt; <paramref name="upperBound"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="lowerBound"></param>
    /// <param name="upperBound"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsBetweenExclusiveUpperBound<T>( this T value, T lowerBound, T upperBound ) where T : IComparable<T>
    {
        return lowerBound.CompareTo( value ) <= 0 && upperBound.CompareTo( value ) > 0;
    }

    /// <summary>
    /// <paramref name="lowerBound"/> &lt; <paramref name="value"/> &lt; <paramref name="upperBound"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="lowerBound"></param>
    /// <param name="upperBound"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsBetweenExclusiveBounds<T>( this T value, T lowerBound, T upperBound ) where T : IComparable<T>
    {
        return lowerBound.CompareTo( value ) < 0 && upperBound.CompareTo( value ) > 0;
    }

    /// <summary>
    /// <paramref name="value"/> &gt; <paramref name="upperBound"/> or <paramref name="value"/> &lt; <paramref name="lowerBound"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="lowerBound"></param>
    /// <param name="upperBound"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsNotBetween<T>( this T value, T lowerBound, T upperBound ) where T : IComparable<T>
    {
        return !IsBetween( value, lowerBound, upperBound );
    }
}
