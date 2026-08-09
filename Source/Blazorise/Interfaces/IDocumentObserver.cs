#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise;

/// <summary>
/// Coordinates browser document events for Blazorise components.
/// </summary>
public interface IDocumentObserver : IAsyncDisposable
{
    /// <summary>
    /// Ensures the document observer has been initialized on the client.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task EnsureInitializedAsync();

    /// <summary>
    /// Subscribes to one or more document events.
    /// </summary>
    /// <param name="subscription">The subscription options.</param>
    /// <returns>A disposable subscription handle.</returns>
    ValueTask<IAsyncDisposable> Subscribe( DocumentObserverSubscription subscription );

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
