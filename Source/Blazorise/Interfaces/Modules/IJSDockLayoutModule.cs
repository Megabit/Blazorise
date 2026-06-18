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
    /// Starts document-level resize tracking for a dock panel splitter.
    /// </summary>
    /// <param name="dotNetObjectRef">The .NET object that receives resize callbacks.</param>
    /// <param name="panelRef">The dock panel element reference.</param>
    /// <param name="panelName">The panel name.</param>
    /// <param name="position">The panel position.</param>
    /// <param name="clientX">The starting pointer X coordinate.</param>
    /// <param name="clientY">The starting pointer Y coordinate.</param>
    /// <param name="minSize">The configured minimum size.</param>
    /// <param name="maxSize">The configured maximum size.</param>
    /// <returns>A task that completes when tracking has started.</returns>
    ValueTask BeginResize( DotNetObjectReference<DockLayout> dotNetObjectRef, ElementReference panelRef, string panelName, string position, double clientX, double clientY, string minSize, string maxSize );

    /// <summary>
    /// Starts document-level drag tracking for a dock panel.
    /// </summary>
    /// <param name="dotNetObjectRef">The .NET object that receives drag callbacks.</param>
    /// <param name="layoutRef">The dock layout element reference.</param>
    /// <param name="panelName">The panel name.</param>
    /// <param name="clientX">The starting pointer X coordinate.</param>
    /// <param name="clientY">The starting pointer Y coordinate.</param>
    /// <param name="dragGroup">Indicates whether the entire tab group should be dragged.</param>
    /// <returns>A task that completes when tracking has started.</returns>
    ValueTask BeginDrag( DotNetObjectReference<DockLayout> dotNetObjectRef, ElementReference layoutRef, string panelName, double clientX, double clientY, bool dragGroup );

    /// <summary>
    /// Cancels the current dock pointer operation.
    /// </summary>
    /// <returns>A task that completes when the operation is cancelled.</returns>
    ValueTask Cancel();
}