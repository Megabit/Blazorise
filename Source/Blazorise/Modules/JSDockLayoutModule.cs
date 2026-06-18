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
    public ValueTask BeginResize( DotNetObjectReference<DockLayout> dotNetObjectRef, ElementReference panelRef, string panelName, string position, double clientX, double clientY, string minSize, string maxSize )
        => InvokeSafeVoidAsync( "beginResize", dotNetObjectRef, panelRef, panelName, position, clientX, clientY, minSize, maxSize );

    /// <inheritdoc/>
    public ValueTask BeginDrag( DotNetObjectReference<DockLayout> dotNetObjectRef, ElementReference layoutRef, string panelName, double clientX, double clientY, bool dragGroup )
        => InvokeSafeVoidAsync( "beginDrag", dotNetObjectRef, layoutRef, panelName, clientX, clientY, dragGroup );

    /// <inheritdoc/>
    public ValueTask Cancel()
        => InvokeSafeVoidAsync( "cancel" );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/dockLayout.js?v={VersionProvider.Version}";

    #endregion
}