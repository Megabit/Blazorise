#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Service to show a simple notification message.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// An event raised after the notification is received.
        /// </summary>
        public event EventHandler<NotificationEventArgs> NotificationReceived;

        /// <summary>
        /// Show the simple info notification.
        /// </summary>
        /// <param name="message">Info notification to show.</param>
        /// <param name="title">Notification title.</param>
        /// <param name="options">Options to override notification dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Info( string message, string title = null, Action<NotificationOptions> options = null );

        /// <summary>
        /// Show the simple info notification.
        /// </summary>
        /// <param name="message">Info notification to show.</param>
        /// <param name="title">Notification title.</param>
        /// <param name="options">Options to override notification dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Info( MarkupString message, string title = null, Action<NotificationOptions> options = null );

        /// <summary>
        /// Show the simple success notification.
        /// </summary>
        /// <param name="message">Success notification to show.</param>
        /// <param name="title">Notification title.</param>
        /// <param name="options">Options to override notification dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Success( string message, string title = null, Action<NotificationOptions> options = null );

        /// <summary>
        /// Show the simple success notification.
        /// </summary>
        /// <param name="message">Success notification to show.</param>
        /// <param name="title">Notification title.</param>
        /// <param name="options">Options to override notification dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Success( MarkupString message, string title = null, Action<NotificationOptions> options = null );

        /// <summary>
        /// Show the simple warning notification.
        /// </summary>
        /// <param name="message">Warning notification to show.</param>
        /// <param name="title">Notification title.</param>
        /// <param name="options">Options to override notification dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Warning( string message, string title = null, Action<NotificationOptions> options = null );

        /// <summary>
        /// Show the simple warning notification.
        /// </summary>
        /// <param name="message">Warning notification to show.</param>
        /// <param name="title">Notification title.</param>
        /// <param name="options">Options to override notification dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Warning( MarkupString message, string title = null, Action<NotificationOptions> options = null );

        /// <summary>
        /// Show the simple error notification.
        /// </summary>
        /// <param name="message">Error notification to show.</param>
        /// <param name="title">Notification title.</param>
        /// <param name="options">Options to override notification dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Error( string message, string title = null, Action<NotificationOptions> options = null );

        /// <summary>
        /// Show the simple error notification.
        /// </summary>
        /// <param name="message">Error notification to show.</param>
        /// <param name="title">Notification title.</param>
        /// <param name="options">Options to override notification dialog appearance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Error( MarkupString message, string title = null, Action<NotificationOptions> options = null );
    }
}
