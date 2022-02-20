#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Extensions
{
    /// <summary>
    /// Generic methods for all object types.
    /// </summary>
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

        /// <summary>
        /// Safely disposes the object.
        /// </summary>
        /// <param name="disposable">Instance of the object to dispose.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task SafeDisposeAsync( this IAsyncDisposable disposable )
        {
            var disposableTask = disposable.DisposeAsync();

            try
            {
                await disposableTask;
            }
            catch when ( disposableTask.IsCanceled )
            {
            }
            catch ( Microsoft.JSInterop.JSDisconnectedException )
            {
            }
        }
    }
}
