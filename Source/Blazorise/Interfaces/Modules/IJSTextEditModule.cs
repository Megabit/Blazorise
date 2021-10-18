#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Contracts for the <see cref="TextEdit"/> JS module.
    /// </summary>
    public interface IJSTextEditModule : IBaseJSModule,
        IJSDestroyableModule
    {
        /// <summary>
        /// Initializes the new <see cref="TextEdit"/> within the JS module.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="elementId">ID of the rendered element.</param>
        /// <param name="maskType">Type of the edit mask.</param>
        /// <param name="editMask">Edit mask value.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask Initialize( ElementReference elementRef, string elementId, string maskType, string editMask );
    }
}
