#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Supplies the information about the UI notification.
    /// </summary>
    public class NotificationEventArgs : EventArgs
    {
        /// <summary>
        /// A default <see cref="NotificationEventArgs"/> constructor.
        /// </summary>
        /// <param name="notificationType">Type of the notification to show.</param>
        /// <param name="message">Notification content.</param>
        /// <param name="title">Title of the notification.</param>
        /// <param name="options">Additional options to override default notification settings.</param>
        public NotificationEventArgs( NotificationType notificationType, string message, string title, NotificationOptions options )
            : this( notificationType, (MarkupString)message, title, options )
        {
        }

        /// <summary>
        /// A default <see cref="NotificationEventArgs"/> constructor.
        /// </summary>
        /// <param name="notificationType">Type of the notification to show.</param>
        /// <param name="message">Notification content.</param>
        /// <param name="title">Title of the notification.</param>
        /// <param name="options">Additional options to override default notification settings.</param>
        public NotificationEventArgs( NotificationType notificationType, MarkupString message, string title, NotificationOptions options )
        {
            NotificationType = notificationType;
            Message = message;
            Title = title;
            Options = options;
        }

        /// <summary>
        /// Gets the notification type.
        /// </summary>
        public NotificationType NotificationType { get; }

        /// <summary>
        /// Gets the notification description.
        /// </summary>
        public MarkupString Message { get; }

        /// <summary>
        /// Gets the notification title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the options that will override default notification settings.
        /// </summary>
        public NotificationOptions Options { get; }
    }
}
