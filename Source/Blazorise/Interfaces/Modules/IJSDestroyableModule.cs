#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Contracts for the module needs to be properly destroyed, or disposed.
    /// </summary>
    public interface IJSDestroyableModule
    {
        /// <summary>
        /// Removes the element from the JS module.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>s
        /// <param name="elementId">ID of the rendered element.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask Destroy( ElementReference elementRef, string elementId );
    }
}
