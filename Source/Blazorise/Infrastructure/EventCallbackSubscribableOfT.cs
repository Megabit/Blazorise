#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Infrastructure;

/// <summary>
/// Represents an event that you may subscribe to. This differs from normal C# events in that the handlers
/// are EventCallback<typeparamref name="T"/>, and so may have async behaviors and cause component re-rendering
/// while retaining error flow.
/// </summary>
/// <typeparam name="T">A type for the eventargs.</typeparam>
public class EventCallbackSubscribable<T>
{
    #region Members

    private readonly Dictionary<EventCallbackSubscriber<T>, EventCallback<T>> callbacks = new();

    #endregion

    #region Methods

    /// <summary>
    /// Invokes all the registered callbacks sequentially, in an undefined order.
    /// </summary>
    public async Task InvokeCallbackAsync( T eventArgs )
    {
        foreach ( var callback in callbacks.Values )
        {
            await callback.InvokeAsync( eventArgs );
        }
    }

    /// <summary>
    /// Subscribes a generic event callback for the specified owner.
    /// </summary>
    /// <param name="owner">The subscriber that owns the callback, with a generic parameter <typeparamref name="T"/>.</param>
    /// <param name="callback">The generic callback method to be subscribed, with a parameter of type <typeparamref name="T"/>.</param>
    internal void Subscribe( EventCallbackSubscriber<T> owner, EventCallback<T> callback )
        => callbacks.Add( owner, callback );

    /// <summary>
    /// Unsubscribes the generic event callback for the specified owner.
    /// </summary>
    /// <param name="owner">The subscriber whose generic callback is to be unsubscribed, with a generic parameter <typeparamref name="T"/>.</param>
    internal void Unsubscribe( EventCallbackSubscriber<T> owner )
        => callbacks.Remove( owner );


    #endregion
}