#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the <see cref="FileInput"/> JS module.
/// </summary>
public interface IJSFileInputModule : IBaseJSModule,
    IJSDestroyableModule
{
    /// <summary>
    /// Initializes the new <see cref="FileInput"/> within the JS module.
    /// </summary>
    /// <param name="dotNetObjectRef"></param>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( DotNetObjectReference<FileInputAdapter> dotNetObjectRef, ElementReference elementRef, string elementId );

    /// <summary>
    /// Resets the file input values.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Reset( ElementReference elementRef, string elementId );

    /// <summary>
    /// Removes a file from the current file selection.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="fileId">ID of the file to remove.</param>
    /// <returns></returns>
    ValueTask RemoveFile( ElementReference elementRef, string elementId, int fileId );

    /// <summary>
    /// Opens the file dialog.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask OpenFileDialog( ElementReference elementRef, string elementId );
}