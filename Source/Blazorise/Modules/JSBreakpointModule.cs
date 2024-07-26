#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the breakpoint JS module.
/// </summary>
public class JSBreakpointModule : BaseJSModule, IJSBreakpointModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSBreakpointModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask RegisterBreakpoint( DotNetObjectReference<BreakpointActivatorAdapter> dotNetObjectRef, string elementId )
        => InvokeSafeVoidAsync( "registerBreakpointComponent", dotNetObjectRef, elementId );

    /// <inheritdoc/>
    public virtual async ValueTask UnregisterBreakpoint( IBreakpointActivator component )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "unregisterBreakpointComponent", component.ElementId );
    }

    /// <inheritdoc/>
    public virtual async ValueTask<string> GetBreakpoint()
    {
        if ( IsUnsafe )
            return default;

        return await InvokeSafeAsync<string>( "getBreakpoint" );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/breakpoint.js?v={VersionProvider.Version}";

    #endregion
}