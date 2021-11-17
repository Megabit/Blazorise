#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Contracts for the modal JS module.
    /// </summary>
    public interface IJSModalModule : IBaseJSModule
    {
        /// <summary>
        /// Opens the modal.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="scrollToTop">Indicates if the window will scroll to the top.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask OpenModal( ElementReference elementRef, bool scrollToTop );

        /// <summary>
        /// Closes the modal.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask CloseModal( ElementReference elementRef );
    }
}
