#region Using directives
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Extensions
{
    /// <summary>
    /// Helper methods for arrays and enumerables.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Determines if all ellement in the supplied arrays are equal.
        /// </summary>
        /// <param name="array1">First array to check.</param>
        /// <param name="array2">Second array to check.</param>
        /// <returns>True if all elements are equal.</returns>
        public static bool AreEqual<T>( this IEnumerable<T> array1, IEnumerable<T> array2 )
        {
            if ( array1 == null && array2 == null )
                return true;

            if ( ( array1 != null && array2 == null ) || ( array2 != null && array1 == null ) )
                return false;

            return array1.SequenceEqual( array2 );
        }

        /// <summary>
        /// Determines if the supplied collection is null or empty, i.e. not containing any element.
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        /// <param name="collection">The collection to check for emptiness.</param>
        /// <returns>True if the source sequence is null ot not contains any elements; otherwise, false.</returns>
        public static bool IsNullOrEmpty<T>( this IEnumerable<T> collection )
        {
            return collection == null || !collection.Any();
        }
    }
}
