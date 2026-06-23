#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation for the DockLayout JS module.
/// </summary>
public class JSDockLayoutModule : BaseJSModule, IJSDockLayoutModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSDockLayoutModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public ValueTask BeginResize( DotNetObjectReference<DockLayout> dotNetObjectRef, ElementReference paneRef, string paneName, string nodeId, string position, long pointerId, double clientX, double clientY, string minSize, string maxSize )
        => InvokeSafeVoidAsync( "beginResize", dotNetObjectRef, paneRef, paneName, nodeId, position, pointerId, clientX, clientY, minSize, maxSize );

    /// <inheritdoc/>
    public ValueTask BeginDrag( DotNetObjectReference<DockLayout> dotNetObjectRef, ElementReference layoutRef, string paneName, long pointerId, double clientX, double clientY, bool dragGroup )
        => InvokeSafeVoidAsync( "beginDrag", dotNetObjectRef, layoutRef, paneName, pointerId, clientX, clientY, dragGroup );

    /// <inheritdoc/>
    public ValueTask Cancel()
        => InvokeSafeVoidAsync( "cancel" );

    /// <inheritdoc/>
    public ValueTask SetAutoHideOutsideHandler( DotNetObjectReference<DockLayout> dotNetObjectRef, ElementReference layoutRef, bool enabled )
        => InvokeSafeVoidAsync( "setAutoHideOutsideHandler", dotNetObjectRef, layoutRef, enabled );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/dockLayout.js?v={VersionProvider.Version}";

    #endregion
}
