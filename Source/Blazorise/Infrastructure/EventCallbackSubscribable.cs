#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Infrastructure;

/// <summary>
/// Represents an event that you may subscribe to. This differs from normal C# events in that the handlers
/// are EventCallback, and so may have async behaviors and cause component re-rendering
/// while retaining error flow.
/// </summary>
public class EventCallbackSubscribable
{
    #region Members

    private readonly Dictionary<EventCallbackSubscriber, EventCallback> callbacks = new();

    #endregion

    #region Methods

    /// <summary>
    /// Invokes all the registered callbacks sequentially, in an undefined order.
    /// </summary>
    public async Task InvokeCallbackAsync()
    {
        foreach ( var callback in callbacks.Values )
        {
            await callback.InvokeAsync();
        }
    }

    /// <summary>
    /// Subscribes an event callback for the specified owner.
    /// </summary>
    /// <param name="owner">The subscriber that owns the callback.</param>
    /// <param name="callback">The callback method to be subscribed.</param>
    internal void Subscribe( EventCallbackSubscriber owner, EventCallback callback )
        => callbacks.Add( owner, callback );

    /// <summary>
    /// Unsubscribes the event callback for the specified owner.
    /// </summary>
    /// <param name="owner">The subscriber whose callback is to be unsubscribed.</param>
    internal void Unsubscribe( EventCallbackSubscriber owner )
        => callbacks.Remove( owner );


    #endregion
}