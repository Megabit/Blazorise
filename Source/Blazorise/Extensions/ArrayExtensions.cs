#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Extensions;

/// <summary>
/// Helper methods for arrays and enumerables.
/// </summary>
public static class ArrayExtensions
{
    /// <summary>
    /// Determines if all ellements in the supplied arrays are equal.
    /// </summary>
    /// <param name="array1">First array to check.</param>
    /// <param name="array2">Second array to check.</param>
    /// <returns>True if all elements are equal.</returns>
    public static bool AreEqual<T>( this IEnumerable<T> array1, IEnumerable<T> array2 )
    {
        if ( ReferenceEquals( array1, array2 ) )
            return true;

        if ( array1 is null || array2 is null )
            return false;

        return array1.SequenceEqual( array2 );
    }

    /// <summary>
    /// Compares two non-generic IEnumerable collections to determine if they are equal.
    /// </summary>
    /// <param name="array1">The first IEnumerable collection.</param>
    /// <param name="array2">The second IEnumerable collection.</param>
    /// <returns>True if the collections are equal, otherwise false.</returns>
    public static bool AreEqual( this IEnumerable array1, IEnumerable array2 )
    {
        if ( array1 is null && array2 is null )
            return true;

        if ( ( array1 is not null && array2 is null ) || ( array2 is not null && array1 is null ) )
            return false;

        var enumerator1 = array1.GetEnumerator();
        var enumerator2 = array2.GetEnumerator();

        while ( enumerator1.MoveNext() )
        {
            if ( !enumerator2.MoveNext() || !Equals( enumerator1.Current, enumerator2.Current ) )
                return false;
        }

        // Check if array2 still has more elements
        if ( enumerator2.MoveNext() )
            return false;

        return true;
    }

    /// <summary>
    /// Determines if all ellements in the supplied arrays are equal. Orders the elements of both collections.
    /// </summary>
    /// <param name="array1">First array to check.</param>
    /// <param name="array2">Second array to check.</param>
    /// <returns>True if all elements are equal when ordered.</returns>
    public static bool AreEqualOrdered<T>( this IEnumerable<T> array1, IEnumerable<T> array2 )
    {
        if ( array1 is null && array2 is null )
            return true;

        if ( ( array1 is not null && array2 is null ) || ( array2 is not null && array1 is null ) )
            return false;

        return Enumerable.SequenceEqual( array1.OrderBy( e => e ), array2.OrderBy( e => e ) );
    }

    /// <summary>
    /// Determines if the supplied collection is null or empty, i.e. not containing any element.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    /// <param name="collection">The collection to check for emptiness.</param>
    /// <returns>True if the source sequence is null or if it does not contains any elements; otherwise, false.</returns>
    public static bool IsNullOrEmpty<T>( this IEnumerable<T> collection )
    {
        return collection is null || !collection.Any();
    }

    /// <summary>
    /// Returns the index of an element that matches the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of an element that matches the specified <paramref name="predicate"/>, if found; otherwise it'll return -1.
    /// </returns>
    public static int Index<T>( this IEnumerable<T> collection, Func<T, bool> predicate )
    {
        return collection?.Select( ( obj, idx ) => new { obj, idx } )?.FirstOrDefault( x => predicate( x.obj ) )?.idx ?? -1;
    }

    /// <summary>
    /// Based on https://stackoverflow.com/a/8094931
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static int GetListHash<T>( this IEnumerable<T> collection )
    {
        int hash = 19;
        foreach ( var value in collection )
        {
            hash = hash * 31 + value.GetHashCode();
        }
        return hash;
    }
}