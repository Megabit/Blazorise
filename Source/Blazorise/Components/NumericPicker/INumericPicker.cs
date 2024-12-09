#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Base interface for <see cref="NumericPicker{TValue}"/> to be updated from javascript.
/// </summary>
/// <remarks>
/// This is needed to set the value from javascript because calling generic component directly is not supported by Blazor.
/// </remarks>
public interface INumericPicker
{
    /// <summary>
    /// Updates the <see cref="NumericPicker{TValue}"/> with the new value. This method is intended for internal framework use only and should not be called directly by user code.
    /// </summary>
    /// <param name="value">New value.</param>
    /// <returns>Returns awaitable task.</returns>
    Task SetValue( string value );
}