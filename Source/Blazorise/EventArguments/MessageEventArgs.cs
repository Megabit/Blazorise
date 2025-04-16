#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Supplies the information about the UI message.
/// </summary>
public class MessageEventArgs : EventArgs
{
    /// <summary>
    /// A default <see cref="MessageEventArgs"/> constructor.
    /// </summary>
    /// <param name="messageType">Type of the message to show.</param>
    /// <param name="message">Message content.</param>
    /// <param name="title">Title of the message.</param>
    /// <param name="options">Additional options to override default dialog settings.</param>
    public MessageEventArgs( MessageType messageType, string message, string title, MessageOptions options )
        : this( messageType, (MarkupString)message, title, options, null )
    {
    }

    /// <summary>
    /// A default <see cref="MessageEventArgs"/> constructor.
    /// </summary>
    /// <param name="messageType">Type of the message to show.</param>
    /// <param name="message">Message content.</param>
    /// <param name="title">Title of the message.</param>
    /// <param name="options">Additional options to override default dialog settings.</param>
    public MessageEventArgs( MessageType messageType, MarkupString message, string title, MessageOptions options )
        : this( messageType, message, title, options, null )
    {
    }

    /// <summary>
    /// A default <see cref="MessageEventArgs"/> constructor.
    /// </summary>
    /// <param name="messageType">Type of the message to show.</param>
    /// <param name="message">Message content.</param>
    /// <param name="title">Title of the message.</param>
    /// <param name="options">Additional options to override default message settings.</param>
    /// <param name="callback">Callback that will execute once the user responds with an action.</param>
    public MessageEventArgs( MessageType messageType, string message, string title, MessageOptions options, TaskCompletionSource<object> callback )
        : this( messageType, (MarkupString)message, title, options, callback )
    {
    }

    /// <summary>
    /// A default <see cref="MessageEventArgs"/> constructor.
    /// </summary>
    /// <param name="messageType">Type of the message to show.</param>
    /// <param name="message">Message content.</param>
    /// <param name="title">Title of the message.</param>
    /// <param name="options">Additional options to override default message settings.</param>
    /// <param name="callback">Callback that will execute once the user responds with an action.</param>
    public MessageEventArgs( MessageType messageType, MarkupString message, string title, MessageOptions options, TaskCompletionSource<object> callback )
    {
        MessageType = messageType;
        Message = message;
        Title = title;
        Options = options;
        Callback = callback;
    }

    /// <summary>
    /// Gets the message type.
    /// </summary>
    public MessageType MessageType { get; }

    /// <summary>
    /// Gets the message description.
    /// </summary>
    public MarkupString Message { get; }

    /// <summary>
    /// Gets the message title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the options that will override default message settings.
    /// </summary>
    public MessageOptions Options { get; }

    /// <summary>
    /// Gets the callback that will execute once the user responds with an action.
    /// </summary>
    public TaskCompletionSource<object> Callback { get; }
}