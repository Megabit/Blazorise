#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Component that handles the <see cref="IMessageService"/> to show the message dialog.
    /// </summary>
    public partial class MessageAlert : BaseComponent
    {
        #region Methods

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            MessageService.MessageReceived += OnMessageReceived;
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
            if ( MessageService != null )
            {
                MessageService.MessageReceived -= OnMessageReceived;
            }
        }

        private async void OnMessageReceived( object sender, MessageEventArgs e )
        {
            MessageType = e.MessageType;
            Message = e.Message;
            Title = e.Title;
            Options = e.Options;
            Callback = e.Callback;

            await InvokeAsync( ModalRef.Show );
        }

        /// <summary>
        /// Handles the OK button click event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task OnOkClicked()
        {
            return InvokeAsync( async () =>
            {
                await Okayed.InvokeAsync( null );

                ModalRef.Hide();
            } );
        }

        /// <summary>
        /// Handles the Confirm button click event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task OnConfirmClicked()
        {
            return InvokeAsync( async () =>
            {
                ModalRef.Hide();

                if ( IsConfirmation && Callback != null )
                {
                    await InvokeAsync( () => Callback.SetResult( true ) );
                }

                await Confirmed.InvokeAsync( null );
            } );
        }

        /// <summary>
        /// Handles the Cancel button click event.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task OnCancelClicked()
        {
            return InvokeAsync( async () =>
            {
                ModalRef.Hide();

                if ( IsConfirmation && Callback != null )
                {
                    await InvokeAsync( () => Callback.SetResult( false ) );
                }

                await Canceled.InvokeAsync( null );
            } );
        }

        /// <summary>
        /// Handles the <see cref="Modal"/> closing event.
        /// </summary>
        /// <param name="eventArgs">Provides the data for the modal closing event.</param>
        protected virtual void OnModalClosing( ModalClosingEventArgs eventArgs )
        {
            eventArgs.Cancel = BackgroundCancel && ( eventArgs.CloseReason == CloseReason.EscapeClosing
                || eventArgs.CloseReason == CloseReason.FocusLostClosing );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Modal"/> reference.
        /// </summary>
        protected Modal ModalRef { get; set; }

        /// <summary>
        /// If true, modal will act as a prompt dialog.
        /// </summary>
        protected virtual bool IsConfirmation
            => MessageType == MessageType.Confirmation;

        /// <summary>
        /// If true, message will be centered.
        /// </summary>
        protected virtual bool CenterMessage
           => Options?.CenterMessage ?? true;

        /// <summary>
        /// If true, message will be scrollable.
        /// </summary>
        protected virtual bool ScrollableMessage
           => Options?.ScrollableMessage ?? true;

        /// <summary>
        /// If true, an icon will be shown along with the message.
        /// </summary>
        protected virtual bool ShowMessageIcon
           => Options?.ShowMessageIcon ?? true;

        /// <summary>
        /// Gets the message icon based on the predefined message type.
        /// </summary>
        protected virtual object MessageIcon => Options?.MessageIcon ?? MessageType switch
        {
            MessageType.Info => IconName.Info,
            MessageType.Success => IconName.Check,
            MessageType.Warning => IconName.Exclamation,
            MessageType.Error => IconName.Times,
            MessageType.Confirmation => IconName.QuestionCircle,
            _ => null,
        };

        /// <summary>
        /// Gets the icon color based on the predefined message type.
        /// </summary>
        protected virtual TextColor MessageIconColor => MessageType switch
        {
            MessageType.Info => TextColor.Info,
            MessageType.Success => TextColor.Success,
            MessageType.Warning => TextColor.Warning,
            MessageType.Error => TextColor.Danger,
            MessageType.Confirmation => TextColor.Secondary,
            _ => TextColor.None,
        };

        /// <summary>
        /// Gets the OK button text.
        /// </summary>
        protected virtual string OkButtonText
            => Options?.OkButtonText ?? "OK";

        /// <summary>
        /// Gets the Confirm button text.
        /// </summary>
        protected virtual string ConfirmButtonText
            => Options?.ConfirmButtonText ?? "Confirm";

        /// <summary>
        /// Gets the Cancel button text.
        /// </summary>
        protected virtual string CancelButtonText
            => Options?.CancelButtonText ?? "Cancel";

        /// <summary>
        /// Gets or sets the <see cref="IMessageService"/> to which this dialog is responding.
        /// </summary>
        [Inject] protected IMessageService MessageService { get; set; }

        /// <summary>
        /// Gets or sets the message type.
        /// </summary>
        [Parameter] public MessageType MessageType { get; set; }

        /// <summary>
        /// Gets or sets the message title.
        /// </summary>
        [Parameter] public string Title { get; set; }

        /// <summary>
        /// Gets or sets the message content.
        /// </summary>
        [Parameter] public MarkupString Message { get; set; }

        /// <summary>
        /// Gets or sets the custom message options.
        /// </summary>
        [Parameter] public MessageOptions Options { get; set; }

        /// <summary>
        /// Occurs after the user respond with an action button.
        /// </summary>
        [Parameter] public TaskCompletionSource<bool> Callback { get; set; }

        /// <summary>
        /// Occurs after the user has responded with an OK action.
        /// </summary>
        [Parameter] public EventCallback Okayed { get; set; }

        /// <summary>
        /// Occurs after the user has responded with a Confirm action.
        /// </summary>
        [Parameter] public EventCallback Confirmed { get; set; }

        /// <summary>
        /// Occurs after the user has responded with a Cancel action.
        /// </summary>
        [Parameter] public EventCallback Canceled { get; set; }

        /// <summary>
        /// By default, a modal is cancelled if the user clicks anywhere outside the modal.
        /// This behavior can be disabled by setting <see cref="BackgroundCancel"/> to false.
        /// </summary>
        [Parameter] public bool BackgroundCancel { get; set; } = true;

        #endregion
    }
}
