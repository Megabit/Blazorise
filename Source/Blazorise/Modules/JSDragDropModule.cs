#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the tooltip JS module.
/// </summary>
public class JSDragDropModule : BaseJSModule, IJSDragDropModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    public JSDragDropModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public virtual ValueTask Initialize( ElementReference elementRef, string elementId )
        => InvokeSafeVoidAsync( "initialize", elementRef, elementId );

    /// <inheritdoc/>
    public virtual ValueTask InitializeThrottledDragEvents<T>( ElementReference elementRef, string elementId, DotNetObjectReference<T> dotNetObjectReference ) where T : class
        => InvokeSafeVoidAsync( "initializeThrottledDragEvents", elementRef, elementId, dotNetObjectReference );

    /// <inheritdoc/>
    public virtual async ValueTask DestroyThrottledDragEvents( ElementReference elementRef, string elementId )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "destroyThrottledDragEvents", elementRef, elementId );
    }

    /// <inheritdoc/>
    public virtual async ValueTask Destroy( ElementReference elementRef, string elementId )
    {
        if ( IsUnsafe )
            return;

        await InvokeSafeVoidAsync( "destroy", elementRef, elementId );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/dragDrop.js?v={VersionProvider.Version}";

    #endregion
}