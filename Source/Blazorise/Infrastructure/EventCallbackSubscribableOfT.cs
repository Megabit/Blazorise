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
    /// Don't call this directly - it gets called by EventCallbackSubscription.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="callback"></param>
    public void Subscribe( EventCallbackSubscriber<T> owner, EventCallback<T> callback )
        => callbacks.Add( owner, callback );

    /// <summary>
    /// Don't call this directly - it gets called by EventCallbackSubscription.
    /// </summary>
    /// <param name="owner"></param>
    public void Unsubscribe( EventCallbackSubscriber<T> owner )
        => callbacks.Remove( owner );

    #endregion
}