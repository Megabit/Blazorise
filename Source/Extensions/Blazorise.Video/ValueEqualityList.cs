#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Video
{
    /// <summary>
    /// Custom list that checks only values for equality.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueEqualityList<T> : List<T>
    {
        /// <inheritdoc/>
        public override bool Equals( object other )
        {
            if ( !( other is IEnumerable<T> enumerable ) )
                return false;

            return enumerable.SequenceEqual( this );
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = 0;

            foreach ( var item in this )
            {
                hashCode ^= item.GetHashCode();
            }

            return hashCode;
        }
    }
}
