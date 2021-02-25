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

            if ( array1 != null && array2 != null && array1.Count() == array2.Count() )
            {
                return array1
                    .Zip( array2, ( value1, value2 ) => value1.IsEqual( value2 ) )
                    .All( result => result );
            }

            return false;
        }
    }
}
