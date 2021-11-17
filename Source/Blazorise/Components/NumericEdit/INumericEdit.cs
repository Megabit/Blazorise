#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base interface for <see cref="NumericEdit{TValue}"/> to be updated from javascript.
    /// </summary>
    /// <remarks>
    /// This is needed to set the value from javascript because calling generic component directly is not supported by Blazor.
    /// </remarks>
    public interface INumericEdit
    {
        /// <summary>
        /// Updates the <see cref="NumericEdit{TValue}"/> with the new value.
        /// </summary>
        /// <param name="value">New value.</param>
        /// <returns>Returns awaitable task.</returns>
        Task SetValue( string value );
    }
}
