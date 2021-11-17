#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <inheritdoc/>
    class NotificationService : INotificationService
    {
        /// <inheritdoc/>
        public event EventHandler<NotificationEventArgs> NotificationReceived;

        public Task RaiseMessage( NotificationType notificationType, MarkupString message, string title = null, Action<NotificationOptions> options = null )
        {
            var notificationOptions = NotificationOptions.Default;
            options?.Invoke( notificationOptions );

            NotificationReceived?.Invoke( this, new( notificationType, message, title, notificationOptions ) );

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task Info( string message, string title = null, Action<NotificationOptions> options = null )
            => Info( (MarkupString)message, title, options );

        /// <inheritdoc/>
        public Task Info( MarkupString message, string title = null, Action<NotificationOptions> options = null )
            => RaiseMessage( NotificationType.Info, message, title, options );

        /// <inheritdoc/>
        public Task Success( string message, string title = null, Action<NotificationOptions> options = null )
            => Success( (MarkupString)message, title, options );

        /// <inheritdoc/>
        public Task Success( MarkupString message, string title = null, Action<NotificationOptions> options = null )
            => RaiseMessage( NotificationType.Success, message, title, options );

        /// <inheritdoc/>
        public Task Warning( string message, string title = null, Action<NotificationOptions> options = null )
            => Warning( (MarkupString)message, title, options );

        /// <inheritdoc/>
        public Task Warning( MarkupString message, string title = null, Action<NotificationOptions> options = null )
            => RaiseMessage( NotificationType.Warning, message, title, options );

        /// <inheritdoc/>
        public Task Error( string message, string title = null, Action<NotificationOptions> options = null )
            => Error( (MarkupString)message, title, options );

        /// <inheritdoc/>
        public Task Error( MarkupString message, string title = null, Action<NotificationOptions> options = null )
            => RaiseMessage( NotificationType.Error, message, title, options );
    }
}
