#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Infrastructure;

/// <summary>
/// Represents an event callback that may be subscribed to an <see cref="EventCallbackSubscribable"/>.
/// </summary>
public class EventCallbackSubscriber : IDisposable
{
    #region Members

    private readonly EventCallback handler;

    private EventCallbackSubscribable existingSubscription;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="EventCallbackSubscriber"/> constructor.
    /// </summary>
    /// <param name="handler"></param>
    public EventCallbackSubscriber( EventCallback handler )
    {
        this.handler = handler;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a subscription on the <paramref name="subscribable"/>, or moves any existing subscription to it
    /// by first unsubscribing from the previous <see cref="EventCallbackSubscribable"/>.
    ///
    /// If the supplied <paramref name="subscribable"/> is null, no new subscription will be created, but any
    /// existing one will still be unsubscribed.
    /// </summary>
    /// <param name="subscribable"></param>
    public void SubscribeOrReplace( EventCallbackSubscribable subscribable )
    {
        if ( subscribable != existingSubscription )
        {
            existingSubscription?.Unsubscribe( this );
            subscribable?.Subscribe( this, handler );
            existingSubscription = subscribable;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        existingSubscription?.Unsubscribe( this );
    }

    #endregion
}