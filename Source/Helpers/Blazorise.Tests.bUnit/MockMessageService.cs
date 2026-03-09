#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Tests.bUnit;

internal sealed class MockMessageService : IMessageService
{
    public event EventHandler<MessageEventArgs> MessageReceived;

    public Task Info( string message, string title = null, Action<MessageOptions> options = null )
        => Info( (MarkupString)message, title, options );

    public Task Info( MarkupString message, string title = null, Action<MessageOptions> options = null )
        => RaiseMessage( MessageType.Info, message, title, options );

    public Task Success( string message, string title = null, Action<MessageOptions> options = null )
        => Success( (MarkupString)message, title, options );

    public Task Success( MarkupString message, string title = null, Action<MessageOptions> options = null )
        => RaiseMessage( MessageType.Success, message, title, options );

    public Task Warning( string message, string title = null, Action<MessageOptions> options = null )
        => Warning( (MarkupString)message, title, options );

    public Task Warning( MarkupString message, string title = null, Action<MessageOptions> options = null )
        => RaiseMessage( MessageType.Warning, message, title, options );

    public Task Error( string message, string title = null, Action<MessageOptions> options = null )
        => Error( (MarkupString)message, title, options );

    public Task Error( MarkupString message, string title = null, Action<MessageOptions> options = null )
        => RaiseMessage( MessageType.Error, message, title, options );

    public Task<bool> Confirm( string message, string title = null, Action<MessageOptions> options = null )
        => Confirm( (MarkupString)message, title, options );

    public Task<bool> Confirm( MarkupString message, string title = null, Action<MessageOptions> options = null )
    {
        if ( MessageReceived is null )
            return Task.FromResult( true );

        var messageOptions = MessageOptions.Default;
        options?.Invoke( messageOptions );

        var callback = new TaskCompletionSource<object>();

        MessageReceived.Invoke( this, new( MessageType.Confirmation, message, title, messageOptions, callback ) );

        return AwaitConfirmation( callback.Task );
    }

    public Task<object> Choose( string message, string title = null, Action<MessageOptions> options = null )
        => Choose( (MarkupString)message, title, options );

    public Task<object> Choose( MarkupString message, string title = null, Action<MessageOptions> options = null )
    {
        if ( MessageReceived is null )
            return Task.FromResult<object>( null );

        var messageOptions = MessageOptions.Default;
        options?.Invoke( messageOptions );

        var callback = new TaskCompletionSource<object>();

        MessageReceived.Invoke( this, new( MessageType.Choice, message, title, messageOptions, callback ) );

        return callback.Task;
    }

    private Task RaiseMessage( MessageType messageType, MarkupString message, string title, Action<MessageOptions> options )
    {
        if ( MessageReceived is null )
            return Task.CompletedTask;

        var messageOptions = MessageOptions.Default;
        options?.Invoke( messageOptions );

        MessageReceived.Invoke( this, new( messageType, message, title, messageOptions ) );

        return Task.CompletedTask;
    }

    private static async Task<bool> AwaitConfirmation( Task<object> callbackTask )
    {
        var result = await callbackTask;

        return result is bool booleanResult && booleanResult;
    }
}