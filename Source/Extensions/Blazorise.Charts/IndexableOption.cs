#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Represents an object that can be either a single value or an array of values. This is used for type safe js-interop.
    /// </summary>
    /// <typeparam name="T">The type of data this <see cref="IndexableOption{T}"/> is supposed to hold.</typeparam>
    public class IndexableOption<T> : IEquatable<IndexableOption<T>>
    {
        /// <summary>
        /// The compile-time name of the property which gets the wrapped value. This is used internally for serialization.
        /// </summary>
        internal const string PropertyName = nameof( BoxedValue );

        // for serialization, there has to be a cast to object anyway
        internal object BoxedValue => IsIndexed ? IndexedValues : SingleValue;

        private readonly T[] indexedValues;

        private readonly T singleValue;

        /// <summary>
        /// The indexed values represented by this instance.
        /// </summary>
        public T[] IndexedValues
        {
            get
            {
                if ( !IsIndexed )
                    throw new InvalidOperationException( "This instance represents a single value. The indexed values are not available." );

                return indexedValues;
            }
        }

        /// <summary>
        /// The single value represented by this instance.
        /// </summary>
        public T SingleValue
        {
            get
            {
                if ( IsIndexed )
                    throw new InvalidOperationException( "This instance represents an array of values. The single value is not available." );

                return singleValue;
            }
        }

        /// <summary>
        /// Gets the value indicating whether the option wrapped in this <see cref="IndexableOption{T}"/> is indexed.
        /// <para>True if the wrapped value represents an array of <typeparamref name="T"/>, false if it represents a single value of <typeparamref name="T"/>.</para>
        /// </summary>
        public bool IsIndexed { get; }

        /// <summary>
        /// Creates a new instance of <see cref="IndexableOption{T}"/> which represents a single value.
        /// </summary>
        /// <param name="singleValue">The single value this <see cref="IndexableOption{T}"/> should represent.</param>
        public IndexableOption( T singleValue )
        {
            this.singleValue = singleValue != null ? singleValue : throw new ArgumentNullException( nameof( singleValue ) );
            IsIndexed = false;
        }

        /// <summary>
        /// Creates a new instance of <see cref="IndexableOption{T}"/> which represents an array of values.
        /// </summary>
        /// <param name="indexedValues">The array of values this <see cref="IndexableOption{T}"/> should represent.</param>
        public IndexableOption( T[] indexedValues )
        {
            this.indexedValues = indexedValues ?? throw new ArgumentNullException( nameof( indexedValues ) );
            IsIndexed = true;
        }

        /// <summary>
        /// Implicitly wraps a single value of <typeparamref name="T"/> to a new instance of <see cref="IndexableOption{T}"/>.
        /// </summary>
        /// <param name="singleValue">The single value to wrap</param>
        public static implicit operator IndexableOption<T>( T singleValue )
        {
            CheckIsNotIndexableOption( singleValue.GetType() );

            return new( singleValue );
        }

        /// <summary>
        /// Implicitly wraps an array of values of <typeparamref name="T"/> to a new instance of <see cref="IndexableOption{T}"/>.
        /// </summary>
        /// <param name="indexedValues">The array of values to wrap</param>
        public static implicit operator IndexableOption<T>( T[] indexedValues )
        {
            CheckIsNotIndexableOption( indexedValues.GetType().GetElementType() );

            return new( indexedValues );
        }

        private static void CheckIsNotIndexableOption( Type type )
        {
            if ( !type.IsGenericType )
                return;
            if ( type.GetGenericTypeDefinition() == typeof( IndexableOption<> ) )
                throw new ArgumentException( "You cannot use an indexable option inside an indexable option." );
        }

        /// <summary>
        /// Determines whether the specified <see cref="IndexableOption{T}"/> instance is considered equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="IndexableOption{T}"/> to compare with.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public bool Equals( IndexableOption<T> other )
        {
            if ( IsIndexed != other.IsIndexed )
                return false;

            if ( IsIndexed )
            {
                if ( IndexedValues == other.IndexedValues )
                    return true;

                return Enumerable.SequenceEqual( IndexedValues, other.IndexedValues );
            }
            else
            {
                return EqualityComparer<T>.Default.Equals( SingleValue, other.SingleValue );
            }
        }

        /// <summary>
        /// Determines whether the specified object instance is considered equal to the current instance.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public override bool Equals( object obj )
        {
            // an indexable option cannot store null
            if ( obj == null )
                return false;

            if ( obj is IndexableOption<T> option )
            {
                return Equals( option );
            }
            else
            {
                if ( IsIndexed )
                {
                    return IndexedValues.Equals( obj );
                }
                else
                {
                    return SingleValue.Equals( obj );
                }
            }
        }

        /// <summary>
        /// Returns the hash of the underlying object.
        /// </summary>
        /// <returns>The hash of the underlying object.</returns>
        public override int GetHashCode()
        {
            var hashCode = -506568782;
            hashCode = hashCode * -1521134295 + EqualityComparer<T[]>.Default.GetHashCode( indexedValues );
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode( singleValue );
            hashCode = hashCode * -1521134295 + IsIndexed.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Determines whether two specified <see cref="IndexableOption{T}"/> instances contain the same value.
        /// </summary>
        /// <param name="a">The first <see cref="IndexableOption{T}"/> to compare</param>
        /// <param name="b">The second <see cref="IndexableOption{T}"/> to compare</param>
        /// <returns>true if the value of a is the same as the value of b; otherwise, false.</returns>
        public static bool operator ==( IndexableOption<T> a, IndexableOption<T> b ) => a.Equals( b );

        /// <summary>
        /// Determines whether two specified <see cref="IndexableOption{T}"/> instances contain different values.
        /// </summary>
        /// <param name="a">The first <see cref="IndexableOption{T}"/> to compare</param>
        /// <param name="b">The second <see cref="IndexableOption{T}"/> to compare</param>
        /// <returns>true if the value of a is different from the value of b; otherwise, false.</returns>
        public static bool operator !=( IndexableOption<T> a, IndexableOption<T> b ) => !( a == b );
    }
}
