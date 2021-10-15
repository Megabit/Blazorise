#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Contracts for the various utilites JS module.
    /// </summary>
    public interface IJSUtilitiesModule : IBaseJSModule
    {
        /// <summary>
        /// Adds a classname to the specified element.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="classname">CSS classname to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask AddClass( ElementReference elementRef, string classname );

        /// <summary>
        /// Removes a classname from the specified element.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="classname">CSS classname to remove.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask RemoveClass( ElementReference elementRef, string classname );

        /// <summary>
        /// Toggles a classname on the given element.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="classname">CSS classname to toggle.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask ToggleClass( ElementReference elementRef, string classname );

        /// <summary>
        /// Adds a classname to the body element.
        /// </summary>
        /// <param name="classname">CSS classname to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask AddClassToBody( string classname );

        /// <summary>
        /// Removes a classname from the body element.
        /// </summary>
        /// <param name="classname"></param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask RemoveClassFromBody( string classname );

        /// <summary>
        /// Indicates if parent element has a specified classname.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="classname">CSS classname to check.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask<bool> ParentHasClass( ElementReference elementRef, string classname );
    }
}
