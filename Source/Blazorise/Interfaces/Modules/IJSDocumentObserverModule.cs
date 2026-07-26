#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Modules;

/// <summary>
/// Contract for the document observer JavaScript module.
/// </summary>
public interface IJSDocumentObserverModule : IBaseJSModule
{
    /// <summary>
    /// Initializes the document observer.
    /// </summary>
    /// <param name="dotNetObjectRef">The .NET object that receives document event callbacks.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize( DotNetObjectReference<DocumentObserverAdapter> dotNetObjectRef );

    /// <summary>
    /// Registers a document event subscription.
    /// </summary>
    /// <param name="subscription">The subscription options.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask AddSubscription( DocumentObserverJsSubscription subscription );

    /// <summary>
    /// Removes a document event subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RemoveSubscription( string subscriptionId );

    /// <summary>
    /// Captures a pointer stream for an owner.
    /// </summary>
    /// <param name="ownerId">The owner identifier.</param>
    /// <param name="pointerId">The pointer identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask CapturePointer( string ownerId, long pointerId );

    /// <summary>
    /// Releases a captured pointer stream for an owner.
    /// </summary>
    /// <param name="ownerId">The owner identifier.</param>
    /// <param name="pointerId">The pointer identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ReleasePointer( string ownerId, long pointerId );
}
