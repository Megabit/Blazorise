#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Default implementation of the document observer JavaScript module.
/// </summary>
public class JSDocumentObserverModule : BaseJSModule, IJSDocumentObserverModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSDocumentObserverModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public ValueTask Initialize( DotNetObjectReference<DocumentObserverAdapter> dotNetObjectRef )
        => InvokeSafeVoidAsync( "initialize", dotNetObjectRef );

    /// <inheritdoc/>
    public ValueTask AddSubscription( DocumentObserverJsSubscription subscription )
        => InvokeSafeVoidAsync( "addSubscription", subscription );

    /// <inheritdoc/>
    public ValueTask RemoveSubscription( string subscriptionId )
        => InvokeSafeVoidAsync( "removeSubscription", subscriptionId );

    /// <inheritdoc/>
    public ValueTask CapturePointer( string ownerId, long pointerId )
        => InvokeSafeVoidAsync( "capturePointer", ownerId, pointerId );

    /// <inheritdoc/>
    public ValueTask ReleasePointer( string ownerId, long pointerId )
        => InvokeSafeVoidAsync( "releasePointer", ownerId, pointerId );

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise/documentObserver.js?v={VersionProvider.Version}";

    #endregion
}
