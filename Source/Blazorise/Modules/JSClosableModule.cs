#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the closable JS module.
/// </summary>
public class JSClosableModule : BaseJSModule, IJSClosableModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSClosableModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask Register( DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef, ElementReference elementRef )
        => InvokeSafeVoidAsync( "registerClosableComponent", dotNetObjectRef, elementRef );

    /// <inheritdoc/>
    public virtual ValueTask RegisterLight( ElementReference elementRef )
        => InvokeSafeVoidAsync( "registerClosableLightComponent", elementRef );

    /// <inheritdoc/>
    public virtual async ValueTask Unregister( ICloseActivator component )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "unregisterClosableComponent", component.ElementRef );
    }

    /// <inheritdoc/>
    public virtual async ValueTask UnregisterLight( ElementReference elementRef )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "unregisterClosableLightComponent", elementRef );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/closable.js?v={VersionProvider.Version}";

    #endregion
}