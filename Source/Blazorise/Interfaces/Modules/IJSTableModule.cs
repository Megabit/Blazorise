#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules
{
    /// <summary>
    /// Contracts for the <see cref="Table"/> JS module.
    /// </summary>
    public interface IJSTableModule : IBaseJSModule
    {
        /// <summary>
        /// Initializes the fixed header for the table.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="elementId">ID of the rendered element.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask InitializeFixedHeader( ElementReference elementRef, string elementId );

        /// <summary>
        /// Removes the fixed header from the table.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="elementId">ID of the rendered element.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask DestroyFixedHeader( ElementReference elementRef, string elementId );

        /// <summary>
        /// Scrolls the table to the specified position in pixels.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="elementId">ID of the rendered element.</param>
        /// <param name="pixels">Position of scroll in pixels.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask ScrollTableToPixels( ElementReference elementRef, string elementId, int pixels );

        /// <summary>
        /// Scrolls the table to the specified row number.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="elementId">ID of the rendered element.</param>
        /// <param name="row">Row number to scroll.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask ScrollTableToRow( ElementReference elementRef, string elementId, int row );

        /// <summary>
        /// Initializes the table columns to be resized based on the given mode.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="elementId">ID of the rendered element.</param>
        /// <param name="resizeMode">Resize mode of the table column.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask InitializeResizable( ElementReference elementRef, string elementId, TableResizeMode resizeMode );

        /// <summary>
        /// Removes the resize mode.
        /// </summary>
        /// <param name="elementRef">Reference to the rendered element.</param>
        /// <param name="elementId">ID of the rendered element.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        ValueTask DestroyResizable( ElementReference elementRef, string elementId );
    }
}
