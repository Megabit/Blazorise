#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Snackbar;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components
{
    /// <summary>
    /// Component that handles the <see cref="INotificationService"/> to show the simple notifications.
    /// </summary>
    public partial class NotificationAlert : BaseComponent, IDisposable, IAsyncDisposable
    {
        #region Methods

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            NotificationService.NotificationReceived += OnNotificationReceived;
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                DisposeResources();
            }

            base.Dispose( disposing );
        }

        /// <inheritdoc/>
        protected override ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing )
            {
                DisposeResources();
            }

            return base.DisposeAsync( disposing );
        }

        private void DisposeResources()
        {
            if ( NotificationService != null )
            {
                NotificationService.NotificationReceived -= OnNotificationReceived;
            }
        }

        private async void OnNotificationReceived( object sender, NotificationEventArgs e )
        {
            NotificationType = e.NotificationType;
            Message = e.Message;
            Title = e.Title;
            Options = e.Options;

            var okButtonText = Options?.OkButtonText ?? "OK";

            await SnackbarStack.PushAsync( Message, Title, GetSnackbarColor( e.NotificationType ), ( options ) =>
            {
                options.CloseButtonIcon = IconName.Times;
                options.ActionButtonText = okButtonText;

                if ( e.Options.IntervalBeforeClose > 0 )
                {
                    options.IntervalBeforeClose = e.Options.IntervalBeforeClose;
                }
            } );
        }

        /// <summary>
        /// Handles the <see cref="Snackbar.Snackbar"/> closing event.
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        protected virtual Task OnSnackbarClosed( SnackbarClosedEventArgs eventArgs )
        {
            return eventArgs.CloseReason == SnackbarCloseReason.UserClosed
                ? Okayed.InvokeAsync()
                : Closed.InvokeAsync();
        }

        /// <summary>
        /// Gets the snackbar color based on the predefined notification type.
        /// </summary>
        protected virtual SnackbarColor GetSnackbarColor( NotificationType notificationType )
        {
            return notificationType switch
            {
                NotificationType.Info => SnackbarColor.Info,
                NotificationType.Success => SnackbarColor.Success,
                NotificationType.Warning => SnackbarColor.Warning,
                NotificationType.Error => SnackbarColor.Danger,
                _ => SnackbarColor.None,
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Snackbar.SnackbarStack"/> reference.
        /// </summary>
        protected SnackbarStack SnackbarStack { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="INotificationService"/> to which this component is responding.
        /// </summary>
        [Inject] protected INotificationService NotificationService { get; set; }

        /// <summary>
        /// Gets or sets the notification type.
        /// </summary>
        [Parameter] public NotificationType NotificationType { get; set; }

        /// <summary>
        /// Gets or sets the message content.
        /// </summary>
        [Parameter] public MarkupString Message { get; set; }

        /// <summary>
        /// Gets or sets the message title.
        /// </summary>
        [Parameter] public string Title { get; set; }

        /// <summary>
        /// Defines the default interval (in milliseconds) after which the notification alert will be automatically closed (used if IntervalBeforeClose is not set on PushAsync call).
        /// </summary>
        [Parameter] public double? DefaultInterval { get; set; }

        /// <summary>
        /// Gets or sets the custom message options.
        /// </summary>
        [Parameter] public NotificationOptions Options { get; set; }

        /// <summary>
        /// Occurs after the user has responded with an OK action.
        /// </summary>
        [Parameter] public EventCallback Okayed { get; set; }

        /// <summary>
        /// Occurs after the user has responded with a Cancel action.
        /// </summary>
        [Parameter] public EventCallback Closed { get; set; }

        #endregion
    }
}
