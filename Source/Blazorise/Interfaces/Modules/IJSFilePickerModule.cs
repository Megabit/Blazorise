#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the <see cref="FilePicker"/> JS module.
/// </summary>
public interface IJSFilePickerModule : IBaseJSModule,
    IJSDestroyableModule
{
    /// <summary>
    /// Initializes the new <see cref="FilePicker"/> within the JS module.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( ElementReference elementRef, string elementId );
}