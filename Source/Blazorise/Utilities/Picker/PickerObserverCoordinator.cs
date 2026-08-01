#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Utilities;

/// <summary>
/// Coordinates document observer subscriptions used by picker inputs and popup menus.
/// </summary>
internal sealed class PickerObserverCoordinator : IAsyncDisposable
{
    #region Members

    private readonly IDocumentObserver documentObserver;

    private IAsyncDisposable outsideSubscription;

    private IAsyncDisposable inputKeyDownSubscription;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new picker observer coordinator.
    /// </summary>
    /// <param name="documentObserver">Document observer used to register browser event subscriptions.</param>
    public PickerObserverCoordinator( IDocumentObserver documentObserver )
    {
        this.documentObserver = documentObserver;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Registers prevention of the browser default behavior for picker opening keys.
    /// </summary>
    /// <param name="ownerId">Identifier that owns the observer subscription.</param>
    /// <param name="inputId">Picker input identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask InitializeInputKeyDownAsync( string ownerId, string inputId )
    {
        inputKeyDownSubscription ??= await documentObserver.Subscribe( new()
        {
            OwnerId = ownerId,
            EventTypes = DocumentEventTypes.KeyDown,
            Selector = $"{CssSelectorUtilities.BuildElementIdSelector( inputId )}[data-open-keys]",
            KeysFilter = new[] { "ArrowDown", "F4" },
            PreventDefault = true,
        } );
    }

    /// <summary>
    /// Synchronizes the subscription that detects pointer or focus movement outside a popup picker.
    /// </summary>
    /// <param name="open">Whether the picker popup is open.</param>
    /// <param name="inline">Whether the picker is rendered inline.</param>
    /// <param name="ownerId">Identifier that owns the observer subscription.</param>
    /// <param name="containerId">Picker container identifier.</param>
    /// <param name="popupId">Picker popup identifier.</param>
    /// <param name="handler">Callback invoked for an outside event.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask SynchronizeOutsideSubscriptionAsync(
        bool open,
        bool inline,
        string ownerId,
        string containerId,
        string popupId,
        Func<DocumentEventArgs, Task> handler )
    {
        if ( open && !inline )
        {
            outsideSubscription ??= await documentObserver.Subscribe( new()
            {
                OwnerId = ownerId,
                EventTypes = DocumentEventTypes.PointerDown | DocumentEventTypes.FocusIn,
                ExcludeSelector = $"{CssSelectorUtilities.BuildElementIdSelector( containerId )}, {CssSelectorUtilities.BuildElementIdSelector( popupId )}",
                Priority = -100,
                Handler = handler,
            } );
        }
        else
        {
            await DisposeOutsideSubscriptionAsync();
        }
    }

    /// <summary>
    /// Removes the outside event subscription.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask DisposeOutsideSubscriptionAsync()
    {
        if ( outsideSubscription is null )
            return;

        await outsideSubscription.DisposeAsync();
        outsideSubscription = null;
    }

    /// <summary>
    /// Removes all document observer subscriptions owned by the coordinator.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeOutsideSubscriptionAsync();

        if ( inputKeyDownSubscription is not null )
        {
            await inputKeyDownSubscription.DisposeAsync();
            inputKeyDownSubscription = null;
        }
    }

    #endregion
}