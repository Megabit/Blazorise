#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Adapter used by JavaScript to notify the <see cref="DocumentObserver"/>.
/// </summary>
public class DocumentObserverAdapter
{
    #region Members

    private readonly DocumentObserver documentObserver;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor for <see cref="DocumentObserverAdapter"/>.
    /// </summary>
    /// <param name="documentObserver">The document observer service.</param>
    public DocumentObserverAdapter( DocumentObserver documentObserver )
    {
        this.documentObserver = documentObserver;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Notifies the service that a subscribed document event occurred.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="eventArgs">The document event arguments.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task NotifyDocumentEvent( string subscriptionId, DocumentEventArgs eventArgs )
        => documentObserver.NotifyDocumentEvent( subscriptionId, eventArgs );

    #endregion
}
