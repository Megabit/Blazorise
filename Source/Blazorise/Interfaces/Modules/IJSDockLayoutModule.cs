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
    /// Starts document-level drag tracking for a dock pane.
    /// </summary>
    /// <param name="dotNetObjectRef">The .NET object that receives drag callbacks.</param>
    /// <param name="layoutRef">The dock layout element reference.</param>
    /// <param name="paneName">The pane name.</param>
    /// <param name="pointerId">The starting pointer identifier.</param>
    /// <param name="clientX">The starting pointer X coordinate.</param>
    /// <param name="clientY">The starting pointer Y coordinate.</param>
    /// <param name="dragGroup">Indicates whether the entire tab group should be dragged.</param>
    /// <returns>A task that completes when tracking has started.</returns>
    ValueTask BeginDrag( DotNetObjectReference<DockLayout> dotNetObjectRef, ElementReference layoutRef, string paneName, long pointerId, double clientX, double clientY, bool dragGroup );

    /// <summary>
    /// Cancels the current dock pointer operation.
    /// </summary>
    /// <returns>A task that completes when the operation is cancelled.</returns>
    ValueTask Cancel();

    /// <summary>
    /// Enables or disables document-level outside pointer tracking for auto-hide flyouts.
    /// </summary>
    /// <param name="dotNetObjectRef">The .NET object that receives outside pointer callbacks.</param>
    /// <param name="layoutRef">The dock layout element reference.</param>
    /// <param name="enabled">True to enable outside pointer tracking; otherwise false.</param>
    /// <returns>A task that completes when tracking has been updated.</returns>
    ValueTask SetAutoHideOutsideHandler( DotNetObjectReference<DockLayout> dotNetObjectRef, ElementReference layoutRef, bool enabled );
}