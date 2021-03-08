#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Utilities
{
    public static class EventUtil
    {
        // The repetition in here is because of the four combinations of handlers (sync/async * with/without arg)
        public static Action AsNonRenderingEventHandler( Action callback )
            => new SyncReceiver( callback ).Invoke;
        public static Action<TValue> AsNonRenderingEventHandler<TValue>( Action<TValue> callback )
            => new SyncReceiver<TValue>( callback ).Invoke;
        public static Func<Task> AsNonRenderingEventHandler( Func<Task> callback )
            => new AsyncReceiver( callback ).Invoke;
        public static Func<TValue, Task> AsNonRenderingEventHandler<TValue>( Func<TValue, Task> callback )
            => new AsyncReceiver<TValue>( callback ).Invoke;

        record SyncReceiver( Action callback ) : ReceiverBase { public void Invoke() => callback(); }
        record SyncReceiver<T>( Action<T> callback ) : ReceiverBase { public void Invoke( T arg ) => callback( arg ); }
        record AsyncReceiver( Func<Task> callback ) : ReceiverBase { public Task Invoke() => callback(); }
        record AsyncReceiver<T>( Func<T, Task> callback ) : ReceiverBase { public Task Invoke( T arg ) => callback( arg ); }

        // By implementing IHandleEvent, we can override the event handling logic on a per-handler basis
        // The logic here just calls the callback without triggering any re-rendering
        record ReceiverBase : IHandleEvent
        {
            public Task HandleEventAsync( EventCallbackWorkItem item, object arg ) => item.InvokeAsync( arg );
        }
    }
}
