#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Extensions;

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
        if ( x is null && y is null )
            return true;

        if ( x is null || y is null )
            return false;

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

    /// <summary>
    /// Converts the value to a culture-invariant string representation.
    /// </summary>
    /// <typeparam name="T">The type of the value to convert.</typeparam>
    /// <param name="value">The value to convert to a culture-invariant string.</param>
    /// <returns>A culture-invariant string representation of the value. If the value implements <see cref="IFormattable"/>, it uses <see cref="IFormattable.ToString(string, IFormatProvider)"/> with <see cref="CultureInfo.InvariantCulture"/>; otherwise, it uses <see cref="object.ToString()"/>.</returns>
    public static string ToCultureInvariantString<T>( this T value )
    {
        if ( value is IFormattable formattable )
        {
            return formattable.ToString( null, CultureInfo.InvariantCulture );
        }

        return value?.ToString();
    }
}