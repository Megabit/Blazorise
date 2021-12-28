#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Contracts for the video JS module.
    /// </summary>
    public interface IJSVideoModule : IBaseJSModule,
        IJSDestroyableModule
    {
        /// <summary>
        /// Initializes the new video within the JS module.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="elementId">ID of the rendered element.</param>
        /// <param name="options">Additional options for the video initialization.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask Initialize( ElementReference elementRef, string elementId, object options );
    }
}
