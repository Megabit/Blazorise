#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Determines whether two objects of type T are equal.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>True if the specified objects are equal; otherwise, false.</returns>
        public static bool IsEqual<T>( this T x, T y )
        {
            return EqualityComparer<T>.Default.Equals( x, y );
        }
    }
}
