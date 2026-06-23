#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// Default implementation of <see cref="IDocumentObserver"/>.
/// </summary>
public class DocumentObserver : IDocumentObserver
{
    #region Members

    private readonly IJSDocumentObserverModule jsDocumentObserverModule;

    private readonly Dictionary<string, DocumentObserverSubscription> subscriptions = new();

    private DotNetObjectReference<DocumentObserverAdapter> dotNetObjectRef;

    private Task initializationTask;

    private int nextSubscriptionId;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor for <see cref="DocumentObserver"/>.
    /// </summary>
    /// <param name="jsDocumentObserverModule">The document observer JavaScript module.</param>
    public DocumentObserver( IJSDocumentObserverModule jsDocumentObserverModule )
    {
        this.jsDocumentObserverModule = jsDocumentObserverModule;
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public Task EnsureInitializedAsync()
    {
        return initializationTask ??= InitializeAsync();
    }

    /// <inheritdoc/>
    public async ValueTask<IAsyncDisposable> Subscribe( DocumentObserverSubscription subscription )
    {
        if ( subscription is null )
            throw new ArgumentNullException( nameof( subscription ) );

        if ( subscription.EventTypes == DocumentEventTypes.None )
            throw new ArgumentException( "At least one document event type must be specified.", nameof( subscription ) );

        string subscriptionId = CreateSubscriptionId();

        try
        {
            subscriptions[subscriptionId] = subscription;

            await EnsureInitializedAsync();
            await jsDocumentObserverModule.AddSubscription( CreateJsSubscription( subscriptionId, subscription ) );
        }
        catch
        {
            subscriptions.Remove( subscriptionId );
            throw;
        }

        return new DocumentObserverSubscriptionHandle( this, subscriptionId );
    }

    /// <inheritdoc/>
    public async ValueTask CapturePointer( string ownerId, long pointerId )
    {
        await EnsureInitializedAsync();
        await jsDocumentObserverModule.CapturePointer( ownerId, pointerId );
    }

    /// <inheritdoc/>
    public async ValueTask ReleasePointer( string ownerId, long pointerId )
    {
        await EnsureInitializedAsync();
        await jsDocumentObserverModule.ReleasePointer( ownerId, pointerId );
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        try
        {
            foreach ( string subscriptionId in subscriptions.Keys.ToArray() )
                await jsDocumentObserverModule.RemoveSubscription( subscriptionId );

            if ( jsDocumentObserverModule is IAsyncDisposable asyncDisposable )
                await asyncDisposable.DisposeAsync();
        }
        finally
        {
            subscriptions.Clear();
            dotNetObjectRef?.Dispose();
            dotNetObjectRef = null;
        }
    }

    internal async Task NotifyDocumentEvent( string subscriptionId, DocumentEventArgs eventArgs )
    {
        if ( !subscriptions.TryGetValue( subscriptionId, out DocumentObserverSubscription subscription )
             || subscription.Handler is null )
            return;

        await subscription.Handler( eventArgs );
    }

    private async Task InitializeAsync()
    {
        dotNetObjectRef ??= DotNetObjectReference.Create( new DocumentObserverAdapter( this ) );

        await jsDocumentObserverModule.Initialize( dotNetObjectRef );
    }

    private async ValueTask Unsubscribe( string subscriptionId )
    {
        if ( string.IsNullOrWhiteSpace( subscriptionId ) )
            return;

        if ( subscriptions.Remove( subscriptionId ) )
            await jsDocumentObserverModule.RemoveSubscription( subscriptionId );
    }

    private string CreateSubscriptionId()
    {
        nextSubscriptionId++;

        return $"b-document-observer-{nextSubscriptionId}";
    }

    private static DocumentObserverJsSubscription CreateJsSubscription( string subscriptionId, DocumentObserverSubscription subscription )
    {
        return new()
        {
            Id = subscriptionId,
            OwnerId = subscription.OwnerId,
            EventNames = GetEventNames( subscription.EventTypes ).ToArray(),
            Selector = subscription.Selector,
            ExcludeSelector = subscription.ExcludeSelector,
            Priority = subscription.Priority,
            Capture = subscription.Capture,
            PreventDefault = subscription.PreventDefault,
            StopPropagation = subscription.StopPropagation,
            Throttle = subscription.Throttle,
            IgnorePointerCapture = subscription.IgnorePointerCapture,
            DotNet = subscription.Handler is not null,
        };
    }

    private static IEnumerable<string> GetEventNames( DocumentEventTypes eventTypes )
    {
        if ( eventTypes.HasFlag( DocumentEventTypes.PointerDown ) )
            yield return "pointerdown";

        if ( eventTypes.HasFlag( DocumentEventTypes.PointerMove ) )
            yield return "pointermove";

        if ( eventTypes.HasFlag( DocumentEventTypes.PointerUp ) )
            yield return "pointerup";

        if ( eventTypes.HasFlag( DocumentEventTypes.PointerCancel ) )
            yield return "pointercancel";

        if ( eventTypes.HasFlag( DocumentEventTypes.MouseDown ) )
            yield return "mousedown";

        if ( eventTypes.HasFlag( DocumentEventTypes.MouseMove ) )
            yield return "mousemove";

        if ( eventTypes.HasFlag( DocumentEventTypes.MouseUp ) )
            yield return "mouseup";

        if ( eventTypes.HasFlag( DocumentEventTypes.TouchMove ) )
            yield return "touchmove";

        if ( eventTypes.HasFlag( DocumentEventTypes.TouchEnd ) )
            yield return "touchend";

        if ( eventTypes.HasFlag( DocumentEventTypes.TouchCancel ) )
            yield return "touchcancel";

        if ( eventTypes.HasFlag( DocumentEventTypes.Click ) )
            yield return "click";

        if ( eventTypes.HasFlag( DocumentEventTypes.DoubleClick ) )
            yield return "dblclick";

        if ( eventTypes.HasFlag( DocumentEventTypes.KeyDown ) )
            yield return "keydown";

        if ( eventTypes.HasFlag( DocumentEventTypes.KeyUp ) )
            yield return "keyup";

        if ( eventTypes.HasFlag( DocumentEventTypes.FocusIn ) )
            yield return "focusin";

        if ( eventTypes.HasFlag( DocumentEventTypes.FocusOut ) )
            yield return "focusout";

        if ( eventTypes.HasFlag( DocumentEventTypes.DragStart ) )
            yield return "dragstart";

        if ( eventTypes.HasFlag( DocumentEventTypes.DragOver ) )
            yield return "dragover";

        if ( eventTypes.HasFlag( DocumentEventTypes.DragEnd ) )
            yield return "dragend";

        if ( eventTypes.HasFlag( DocumentEventTypes.Drop ) )
            yield return "drop";

        if ( eventTypes.HasFlag( DocumentEventTypes.ContextMenu ) )
            yield return "contextmenu";

        if ( eventTypes.HasFlag( DocumentEventTypes.Blur ) )
            yield return "blur";
    }

    #endregion

    #region Classes

    private sealed class DocumentObserverSubscriptionHandle : IAsyncDisposable
    {
        private readonly DocumentObserver documentObserver;

        private readonly string subscriptionId;

        private bool disposed;

        public DocumentObserverSubscriptionHandle( DocumentObserver documentObserver, string subscriptionId )
        {
            this.documentObserver = documentObserver;
            this.subscriptionId = subscriptionId;
        }

        public async ValueTask DisposeAsync()
        {
            if ( disposed )
                return;

            disposed = true;

            await documentObserver.Unsubscribe( subscriptionId );
        }
    }

    #endregion
}
