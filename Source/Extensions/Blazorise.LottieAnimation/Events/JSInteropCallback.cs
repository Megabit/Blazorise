using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Blazorise.LottieAnimation;

public class JSInteropCallback<TArgs>
{
    private readonly Func<TArgs, Task> _callback;
    
    public JSInteropCallback( Func<TArgs, Task> asyncCallback )
    {
        _callback = asyncCallback;
    }

    public JSInteropCallback( Action<TArgs> syncCallback )
    {
        _callback = args =>
        {
            syncCallback.Invoke( args );
            return Task.CompletedTask;
        };
    }
    
    [JSInvokable]
    public Task InvokeAsync(TArgs args)
    {
        return _callback.Invoke( args );
    }

    public static implicit operator JSInteropCallback<TArgs>( Func<TArgs, Task> asyncCallback )
    {
        return new JSInteropCallback<TArgs>( asyncCallback );
    }
    
    public static implicit operator JSInteropCallback<TArgs>( Action<TArgs> syncCallback )
    {
        return new JSInteropCallback<TArgs>( syncCallback );
    }

    public static implicit operator DotNetObjectReference<JSInteropCallback<TArgs>>( JSInteropCallback<TArgs> callback )
    {
        return DotNetObjectReference.Create( callback );
    }
}