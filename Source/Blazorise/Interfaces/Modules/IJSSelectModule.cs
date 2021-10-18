#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Contracts for the <see cref="Select{TValue}"/> JS module.
    /// </summary>
    public interface IJSSelectModule : IBaseJSModule
    {
        /// <summary>
        /// Selects the specified item values in a multi select input.
        /// </summary>
        /// <typeparam name="TValue">Type of value.</typeparam>
        /// <param name="elementId">ID of the rendered element.</param>
        /// <param name="values">List of item values.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask SetSelectedOptions<TValue>( string elementId, IReadOnlyList<TValue> values );

        /// <summary>
        /// Gets the selected item values in a multi select input.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="elementId">ID of the rendered element.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask<TValue[]> GetSelectedOptions<TValue>( string elementId );
    }
}
