using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.Modules;

/// <summary>
/// Contract for DockLayout JavaScript interactions.
/// </summary>
public interface IJSDockLayoutModule : IBaseJSModule
{
    /// <summary>
    /// Starts document-level resize tracking for a dock pane splitter.
    /// </summary>
    /// <param name="dotNetObjectRef">The .NET object that receives resize callbacks.</param>
    /// <param name="paneRef">The dock pane element reference.</param>
    /// <param name="paneName">The pane name.</param>
    /// <param name="position">The pane position.</param>
    /// <param name="clientX">The starting pointer X coordinate.</param>
    /// <param name="clientY">The starting pointer Y coordinate.</param>
    /// <param name="minSize">The configured minimum size.</param>
    /// <param name="maxSize">The configured maximum size.</param>
    /// <returns>A task that completes when tracking has started.</returns>
    ValueTask BeginResize( DotNetObjectReference<DockLayout> dotNetObjectRef, ElementReference paneRef, string paneName, string position, double clientX, double clientY, string minSize, string maxSize );

    /// <summary>
    /// Starts document-level drag tracking for a dock pane.
    /// </summary>
    /// <param name="dotNetObjectRef">The .NET object that receives drag callbacks.</param>
    /// <param name="layoutRef">The dock layout element reference.</param>
    /// <param name="paneName">The pane name.</param>
    /// <param name="clientX">The starting pointer X coordinate.</param>
    /// <param name="clientY">The starting pointer Y coordinate.</param>
    /// <param name="dragGroup">Indicates whether the entire tab group should be dragged.</param>
    /// <returns>A task that completes when tracking has started.</returns>
    ValueTask BeginDrag( DotNetObjectReference<DockLayout> dotNetObjectRef, ElementReference layoutRef, string paneName, double clientX, double clientY, bool dragGroup );

    /// <summary>
    /// Cancels the current dock pointer operation.
    /// </summary>
    /// <returns>A task that completes when the operation is cancelled.</returns>
    ValueTask Cancel();
}