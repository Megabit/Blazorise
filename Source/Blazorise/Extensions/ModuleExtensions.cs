#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Extensions
{
    /// <summary>
    /// Helper methods for JS modules.
    /// </summary>
    public static class ModuleExtensions
    {
        /// <summary>
        /// Safely destroys the JS module.
        /// </summary>
        /// <param name="module">Module instance.</param>
        /// <param name="elementRef">Reference to the rendered element.</param>s
        /// <param name="elementId">ID of the rendered element.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task SafeDestroy( this IJSDestroyableModule module, ElementReference elementRef, string elementId )
        {
            var task = module.Destroy( elementRef, elementId );

            try
            {
                await task;
            }
            catch when ( task.IsCanceled )
            {
            }
            catch ( Microsoft.JSInterop.JSDisconnectedException )
            {
            }
        }
    }
}
