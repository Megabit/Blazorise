using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazorise
{
    /// <summary>
    /// Service to show a simple message or prompt dialog with an optional icon.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// An event raised after the message is received. Used to notify the message dialog.
        /// </summary>
        event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        /// Show the simple info message with an optional icon.
        /// </summary>
        /// <param name="message">Info message to show.</param>
        /// <param name="title">Message title.</param>
        /// <param name="options">Options to override message dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Info( string message, string title = null, Action<MessageOptions> options = null );

        /// <summary>
        /// Show the simple info message with an optional icon.
        /// </summary>
        /// <param name="message">Info message to show.</param>
        /// <param name="title">Message title.</param>
        /// <param name="options">Options to override message dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Info( MarkupString message, string title = null, Action<MessageOptions> options = null );

        /// <summary>
        /// Show the simple success message with an optional icon.
        /// </summary>
        /// <param name="message">Success message to show.</param>
        /// <param name="title">Message title.</param>
        /// <param name="options">Options to override message dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Success( string message, string title = null, Action<MessageOptions> options = null );

        /// <summary>
        /// Show the simple success message with an optional icon.
        /// </summary>
        /// <param name="message">Success message to show.</param>
        /// <param name="title">Message title.</param>
        /// <param name="options">Options to override message dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Success( MarkupString message, string title = null, Action<MessageOptions> options = null );

        /// <summary>
        /// Show the simple warning message with an optional icon.
        /// </summary>
        /// <param name="message">Warning message to show.</param>
        /// <param name="title">Message title.</param>
        /// <param name="options">Options to override message dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Warning( string message, string title = null, Action<MessageOptions> options = null );

        /// <summary>
        /// Show the simple warning message with an optional icon.
        /// </summary>
        /// <param name="message">Warning message to show.</param>
        /// <param name="title">Message title.</param>
        /// <param name="options">Options to override message dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Warning( MarkupString message, string title = null, Action<MessageOptions> options = null );

        /// <summary>
        /// Show the simple error message with an optional icon.
        /// </summary>
        /// <param name="message">Error message to show.</param>
        /// <param name="title">Message title.</param>
        /// <param name="options">Options to override message dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Error( string message, string title = null, Action<MessageOptions> options = null );

        /// <summary>
        /// Show the simple error message with an optional icon.
        /// </summary>
        /// <param name="message">Error message to show.</param>
        /// <param name="title">Message title.</param>
        /// <param name="options">Options to override message dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Error( MarkupString message, string title = null, Action<MessageOptions> options = null );

        /// <summary>
        /// Show the confirmation message with an optional icon and will wait until user clicks on one of the supplied actions.
        /// </summary>
        /// <param name="message">Confirmation message to show.</param>
        /// <param name="title">Message title.</param>
        /// <param name="options">Options to override message dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<bool> Confirm( string message, string title = null, Action<MessageOptions> options = null );

        /// <summary>
        /// Show the confirmation message with an optional icon and will wait until user clicks on one of the supplied actions.
        /// </summary>
        /// <param name="message">Confirmation message to show.</param>
        /// <param name="title">Message title.</param>
        /// <param name="options">Options to override message dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<bool> Confirm( MarkupString message, string title = null, Action<MessageOptions> options = null );
    }
}
