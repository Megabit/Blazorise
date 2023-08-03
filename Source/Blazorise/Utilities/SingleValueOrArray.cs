using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Blazorise.Utilities;

/// <summary>
/// Represents the union of a type or an array of the same type.
/// </summary>
/// <typeparam name="TValue">Type of the value or array of values.</typeparam>
public class SingleValueOrArray<TValue> : IList<TValue>
{
    #region Constructors

    /// <summary>
    /// Instantiates a new <see cref="SingleValueOrArray{TValue}"/> from an array of values.
    /// </summary>
    /// <param name="values">Enumerable of values.</param>
    /// <exception cref="ArgumentNullException">Thrown if the enumerable is null.</exception>
    public SingleValueOrArray( IEnumerable<TValue> values )
    {
        ArgumentNullException.ThrowIfNull( values );

        Values = values.ToList();
    }

    /// <summary>
    /// Instantiates a new <see cref="SingleValueOrArray{TValue}"/> from a single value.
    /// </summary>
    /// <param name="value">Single value.</param>
    /// <exception cref="ArgumentNullException">Thrown if the value is null.</exception>
    public SingleValueOrArray( TValue value )
    {
        ArgumentNullException.ThrowIfNull( value );

        Values = new List<TValue>
        {
            value
        };
    }

    #endregion

    #region Methods

    /// <inheritdoc />
    public IEnumerator<TValue> GetEnumerator()
    {
        return Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ( (IEnumerable)Values ).GetEnumerator();
    }

    /// <inheritdoc />
    public void Add( TValue item )
    {
        Values.Add( item );
    }

    /// <inheritdoc />
    public void Clear()
    {
        Values.Clear();
    }

    /// <inheritdoc />
    public bool Contains( TValue item )
    {
        return Values.Contains( item );
    }

    /// <inheritdoc />
    public void CopyTo( TValue[] array, int arrayIndex )
    {
        Values.CopyTo( array, arrayIndex );
    }

    /// <inheritdoc />
    public bool Remove( TValue item )
    {
        return Values.Remove( item );
    }

    /// <inheritdoc />
    public int Count => Values.Count;

    /// <inheritdoc />
    public bool IsReadOnly => Values.IsReadOnly;

    /// <inheritdoc />
    public int IndexOf( TValue item )
    {
        return Values.IndexOf( item );
    }

    /// <inheritdoc />
    public void Insert( int index, TValue item )
    {
        Values.Insert( index, item );
    }

    /// <inheritdoc />
    public void RemoveAt( int index )
    {
        Values.RemoveAt( index );
    }

    /// <summary>
    /// Implicitly converts a single value to a <see cref="SingleValueOrArray{TValue}"/>.
    /// </summary>
    /// <param name="value">Single value to convert.</param>
    /// <returns>A <see cref="SingleValueOrArray{TValue}"/> representing the value.</returns>
    public static implicit operator SingleValueOrArray<TValue>( TValue value )
    {
        return new SingleValueOrArray<TValue>( value );
    }

    /// <summary>
    /// Implicitly converts a list of values to a <see cref="SingleValueOrArray{TValue}"/>.
    /// </summary>
    /// <param name="values">List of values to convert.</param>
    /// <returns>A <see cref="SingleValueOrArray{TValue}"/> representing the values.</returns>
    public static implicit operator SingleValueOrArray<TValue>( List<TValue> values )
    {
        return new SingleValueOrArray<TValue>( values );
    }

    /// <summary>
    /// Implicitly converts an array of values to a <see cref="SingleValueOrArray{TValue}"/>.
    /// </summary>
    /// <param name="values">Array of values to convert.</param>
    /// <returns>A <see cref="SingleValueOrArray{TValue}"/> representing the values.</returns>
    public static implicit operator SingleValueOrArray<TValue>( TValue[] values )
    {
        return new SingleValueOrArray<TValue>( values );
    }

    #endregion

    #region Properties

    /// <summary>
    /// List of values.
    /// </summary>
    public IList<TValue> Values { get; }

    /// <inheritdoc />
    public TValue this[int index]
    {
        get => Values[index];
        set => Values[index] = value;
    }

    #endregion
}