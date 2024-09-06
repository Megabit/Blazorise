#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Infrastructure;

/// <summary>
/// Represents a subscriber that may be subscribe to an <see cref="EventCallbackSubscribable{T}"/>.
/// The subscription can move between <see cref="EventCallbackSubscribable{T}"/> instances over time,
/// and automatically unsubscribes from earlier <see cref="EventCallbackSubscribable{T}"/> instances
/// whenever it moves to a new one.
/// </summary>
public class EventCallbackSubscriber<T> : IDisposable
{
    #region Members

    private readonly EventCallback<T> handler;

    private EventCallbackSubscribable<T> existingSubscription;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="EventCallbackSubscriber{T}"/> constructor.
    /// </summary>
    /// <param name="handler"></param>
    public EventCallbackSubscriber( EventCallback<T> handler )
    {
        this.handler = handler;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a subscription on the <paramref name="subscribable"/>, or moves any existing subscription to it
    /// by first unsubscribing from the previous <see cref="EventCallbackSubscribable{T}"/>.
    ///
    /// If the supplied <paramref name="subscribable"/> is null, no new subscription will be created, but any
    /// existing one will still be unsubscribed.
    /// </summary>
    /// <param name="subscribable"></param>
    public void SubscribeOrReplace( EventCallbackSubscribable<T> subscribable )
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
