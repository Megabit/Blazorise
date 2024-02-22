#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <inheritdoc/>
class ToastService : IToastService
{
    /// <inheritdoc/>
    public event EventHandler<ToastEventArgs> ToastReceived;

    public Task RaiseMessage( ToastIntent intent, MarkupString message, string title = null, Action<ToastInstanceOptions> options = null )
    {
        var toastOptions = ToastInstanceOptions.Default;
        options?.Invoke( toastOptions );

        ToastReceived?.Invoke( this, new( intent, message, title, toastOptions ) );

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task Info( string message, string title = null, Action<ToastInstanceOptions> options = null )
        => Info( (MarkupString)message, title, options );

    /// <inheritdoc/>
    public Task Info( MarkupString message, string title = null, Action<ToastInstanceOptions> options = null )
        => RaiseMessage( ToastIntent.Info, message, title, options );

    /// <inheritdoc/>
    public Task Success( string message, string title = null, Action<ToastInstanceOptions> options = null )
        => Success( (MarkupString)message, title, options );

    /// <inheritdoc/>
    public Task Success( MarkupString message, string title = null, Action<ToastInstanceOptions> options = null )
        => RaiseMessage( ToastIntent.Success, message, title, options );

    /// <inheritdoc/>
    public Task Warning( string message, string title = null, Action<ToastInstanceOptions> options = null )
        => Warning( (MarkupString)message, title, options );

    /// <inheritdoc/>
    public Task Warning( MarkupString message, string title = null, Action<ToastInstanceOptions> options = null )
        => RaiseMessage( ToastIntent.Warning, message, title, options );

    /// <inheritdoc/>
    public Task Error( string message, string title = null, Action<ToastInstanceOptions> options = null )
        => Error( (MarkupString)message, title, options );

    /// <inheritdoc/>
    public Task Error( MarkupString message, string title = null, Action<ToastInstanceOptions> options = null )
        => RaiseMessage( ToastIntent.Error, message, title, options );
}