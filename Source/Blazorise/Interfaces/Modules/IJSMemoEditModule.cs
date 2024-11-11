#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contracts for the <see cref="MemoEdit"/> JS module.
/// </summary>
public interface IJSMemoEditModule : IBaseJSModule,
    IJSDestroyableModule
{
    /// <summary>
    /// Initializes the new <see cref="MemoEdit"/> within the JS module.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="options">Options to initialize the memoedit.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( ElementReference elementRef, string elementId, MemoEditInitializeJSOptions options );

    /// <summary>
    /// Updates the memo options.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <param name="options">New options to initialize the memoedit.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateOptions( ElementReference elementRef, string elementId, MemoEditUpdateJSOptions options );

    /// <summary>
    /// Recalculates the textarea height.
    /// </summary>
    /// <param name="elementRef">Reference to the rendered element.</param>
    /// <param name="elementId">ID of the rendered element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RecalculateAutoHeight( ElementReference elementRef, string elementId );
}