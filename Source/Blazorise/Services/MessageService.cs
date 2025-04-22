#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <inheritdoc/>
class MessageService : IMessageService
{
    /// <inheritdoc/>
    public event EventHandler<MessageEventArgs> MessageReceived;

    public Task RaiseMessage( MessageType messageType, MarkupString message, string title = null, Action<MessageOptions> options = null )
    {
        var messageOptions = MessageOptions.Default;
        options?.Invoke( messageOptions );

        MessageReceived?.Invoke( this, new( messageType, message, title, messageOptions ) );

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task Info( string message, string title = null, Action<MessageOptions> options = null )
        => Info( (MarkupString)message, title, options );

    /// <inheritdoc/>
    public Task Info( MarkupString message, string title = null, Action<MessageOptions> options = null )
        => RaiseMessage( MessageType.Info, message, title, options );

    /// <inheritdoc/>
    public Task Success( string message, string title = null, Action<MessageOptions> options = null )
        => Success( (MarkupString)message, title, options );

    /// <inheritdoc/>
    public Task Success( MarkupString message, string title = null, Action<MessageOptions> options = null )
        => RaiseMessage( MessageType.Success, message, title, options );

    /// <inheritdoc/>
    public Task Warning( string message, string title = null, Action<MessageOptions> options = null )
        => Warning( (MarkupString)message, title, options );

    /// <inheritdoc/>
    public Task Warning( MarkupString message, string title = null, Action<MessageOptions> options = null )
        => RaiseMessage( MessageType.Warning, message, title, options );

    /// <inheritdoc/>
    public Task Error( string message, string title = null, Action<MessageOptions> options = null )
        => Error( (MarkupString)message, title, options );

    /// <inheritdoc/>
    public Task Error( MarkupString message, string title = null, Action<MessageOptions> options = null )
        => RaiseMessage( MessageType.Error, message, title, options );

    /// <inheritdoc/>
    public Task<bool> Confirm( string message, string title = null, Action<MessageOptions> options = null )
        => Confirm( (MarkupString)message, title, options );

    /// <inheritdoc/>
    public async Task<bool> Confirm( MarkupString message, string title = null, Action<MessageOptions> options = null )
    {
        var messageOptions = MessageOptions.Default;
        options?.Invoke( messageOptions );

        var callback = new TaskCompletionSource<object>();

        MessageReceived?.Invoke( this, new( MessageType.Confirmation, message, title, messageOptions, callback ) );

        var result = await callback.Task;

        return result is bool booleanResult && booleanResult;
    }

    /// <inheritdoc/>
    public Task<object> Choose( string message, string title = null, Action<MessageOptions> options = null )
        => Choose( (MarkupString)message, title, options );

    /// <inheritdoc/>
    public Task<object> Choose( MarkupString message, string title = null, Action<MessageOptions> options = null )
    {
        var messageOptions = MessageOptions.Default;
        options?.Invoke( messageOptions );

        var callback = new TaskCompletionSource<object>();

        MessageReceived?.Invoke( this, new( MessageType.Choice, message, title, messageOptions, callback ) );

        return callback.Task;
    }
}