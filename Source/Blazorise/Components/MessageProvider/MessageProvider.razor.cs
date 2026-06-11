#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Component that handles the <see cref="IMessageService"/> to show the message dialog.
/// </summary>
public partial class MessageProvider : BaseComponent, IDisposable
{
    #region Members

    private readonly List<MessageInstance> messages = [];

    private int messageSequence;

    #endregion

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
            if ( MessageService is not null )
            {
                MessageService.MessageReceived -= OnMessageReceived;
            }
        }

        base.Dispose( disposing );
    }

    private async void OnMessageReceived( object sender, MessageEventArgs e )
    {
        await InvokeAsync( () =>
        {
            ShowMessage( e );
        } );
    }

    private void ShowMessage( MessageEventArgs e )
    {
        messages.Add( new( ++messageSequence, e ) );

        StateHasChanged();
    }

    private Task OnModalClosed( MessageInstance message )
    {
        ScheduleMessageRemoval( message );

        return Task.CompletedTask;
    }

    private void ScheduleMessageRemoval( MessageInstance message )
    {
        if ( message.Removing )
            return;

        message.Removing = true;

        ExecuteAfterRender( () =>
        {
            messages.Remove( message );

            return InvokeAsync( StateHasChanged );
        } );

        InvokeAsync( StateHasChanged );
    }

    private Task HideMessage( MessageInstance message )
    {
        if ( message.Closing )
            return Task.CompletedTask;

        message.Closing = true;
        message.Visible = false;

        _ = message.ModalRef.Hide();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the OK button click event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnOkClicked( MessageInstance message )
    {
        return InvokeAsync( async () =>
        {
            await Okayed.InvokeAsync();

            await HideMessage( message );
        } );
    }

    /// <summary>
    /// Handles the OK button click event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnOkClicked()
    {
        var message = LastMessage;

        if ( message is null )
        {
            return Task.CompletedTask;
        }

        return OnOkClicked( message );
    }

    /// <summary>
    /// Handles the Confirm button click event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnConfirmClicked( MessageInstance message )
    {
        return InvokeAsync( async () =>
        {
            await HideMessage( message );

            if ( IsConfirmationMessage( message ) && message.Callback is not null && !message.Callback.Task.IsCompleted )
            {
                await InvokeAsync( () => message.Callback.SetResult( true ) );
            }

            await Confirmed.InvokeAsync();
        } );
    }

    /// <summary>
    /// Handles the Confirm button click event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnConfirmClicked()
    {
        var message = LastMessage;

        if ( message is null )
        {
            return Task.CompletedTask;
        }

        return OnConfirmClicked( message );
    }

    /// <summary>
    /// Handles the Cancel button click event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnCancelClicked( MessageInstance message )
    {
        return InvokeAsync( async () =>
        {
            await HideMessage( message );

            await NotifyCanceled( message );
        } );
    }

    /// <summary>
    /// Handles the Cancel button click event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnCancelClicked()
    {
        var message = LastMessage;

        if ( message is null )
        {
            return Task.CompletedTask;
        }

        return OnCancelClicked( message );
    }

    /// <summary>
    /// Handles the event when a choice button is clicked, hiding the modal and invoking callbacks as necessary.
    /// </summary>
    /// <param name="message">Message instance that owns the choice.</param>
    /// <param name="choice">Represents the button that was clicked, providing its key for further processing.</param>
    /// <returns>Returns a task that represents the asynchronous operation of handling the button click.</returns>
    protected Task OnChoiceClicked( MessageInstance message, MessageOptionsChoice choice )
    {
        return InvokeAsync( async () =>
        {
            await HideMessage( message );

            if ( IsChoiceMessage( message ) && message.Callback is not null && !message.Callback.Task.IsCompleted )
            {
                await InvokeAsync( () => message.Callback.SetResult( choice.Key ) );
            }

            await Confirmed.InvokeAsync();
        } );
    }

    /// <summary>
    /// Handles the event when a choice button is clicked, hiding the modal and invoking callbacks as necessary.
    /// </summary>
    /// <param name="choice">Represents the button that was clicked, providing its key for further processing.</param>
    /// <returns>Returns a task that represents the asynchronous operation of handling the button click.</returns>
    protected Task OnChoiceClicked( MessageOptionsChoice choice )
    {
        var message = LastMessage;

        if ( message is null )
        {
            return Task.CompletedTask;
        }

        return OnChoiceClicked( message, choice );
    }

    /// <summary>
    /// Handles the <see cref="Modal"/> closing event.
    /// </summary>
    /// <param name="message">Message instance that owns the modal.</param>
    /// <param name="eventArgs">Provides the data for the modal closing event.</param>
    protected virtual async Task OnModalClosing( MessageInstance message, ModalClosingEventArgs eventArgs )
    {
        var isEscapeClosing = eventArgs.CloseReason == CloseReason.EscapeClosing;
        var isFocusLostClosing = eventArgs.CloseReason == CloseReason.FocusLostClosing;

        if ( isEscapeClosing && ( message.Options?.CloseOnEscape ?? CloseOnEscape ) )
        {
            await NotifyCanceled( message );

            return;
        }

        eventArgs.Cancel = ( message.Options?.BackgroundCancel ?? BackgroundCancel )
            && ( isEscapeClosing || isFocusLostClosing );
    }

    /// <summary>
    /// Handles the <see cref="Modal"/> closing event.
    /// </summary>
    /// <param name="eventArgs">Provides the data for the modal closing event.</param>
    protected virtual async Task OnModalClosing( ModalClosingEventArgs eventArgs )
    {
        var message = LastMessage;

        if ( message is not null )
        {
            await OnModalClosing( message, eventArgs );
        }
    }

    /// <summary>
    /// Notifies that the message dialog was canceled.
    /// </summary>
    /// <param name="message">Message instance that was canceled.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task NotifyCanceled( MessageInstance message )
    {
        if ( IsConfirmationMessage( message ) && message.Callback is not null && !message.Callback.Task.IsCompleted )
        {
            await InvokeAsync( () => message.Callback.SetResult( false ) );
        }
        else if ( IsChoiceMessage( message ) && message.Callback is not null && !message.Callback.Task.IsCompleted )
        {
            await InvokeAsync( () => message.Callback.SetResult( null ) );
        }

        await Canceled.InvokeAsync();
    }

    private static bool IsConfirmationMessage( MessageInstance message )
        => message.MessageType == MessageType.Confirmation;

    private static bool IsChoiceMessage( MessageInstance message )
        => message.MessageType == MessageType.Choice;

    private bool GetCenterMessage( MessageInstance message )
        => GetSize( message ) == ModalSize.Fullscreen ? false : message.Options?.CenterMessage ?? true;

    private bool GetScrollableMessage( MessageInstance message )
        => GetSize( message ) == ModalSize.Fullscreen ? false : message.Options?.ScrollableMessage ?? true;

    private static bool GetShowMessageIcon( MessageInstance message )
        => message.Options?.ShowMessageIcon ?? true;

    private static bool GetShowCloseButton( MessageInstance message )
        => message.Options?.ShowCloseButton ?? false;

    private static object GetMessageIcon( MessageInstance message )
        => message.Options?.MessageIcon ?? message.MessageType switch
        {
            MessageType.Info => IconName.Info,
            MessageType.Success => IconName.Check,
            MessageType.Warning => IconName.Exclamation,
            MessageType.Error => IconName.Times,
            MessageType.Confirmation => IconName.QuestionCircle,
            _ => null,
        };

    private static TextColor GetMessageIconColor( MessageInstance message )
        => message.Options?.MessageIconColor ?? message.MessageType switch
        {
            MessageType.Info => TextColor.Info,
            MessageType.Success => TextColor.Success,
            MessageType.Warning => TextColor.Warning,
            MessageType.Error => TextColor.Danger,
            MessageType.Confirmation => TextColor.Secondary,
            _ => TextColor.Default,
        };

    private string GetOkButtonText( MessageInstance message )
        => message.Options?.OkButtonText ?? Localizer["OK"] ?? "OK";

    private static Color GetOkButtonColor( MessageInstance message )
        => message.Options?.OkButtonColor ?? Color.Primary;

    private static string GetOkButtonClass( MessageInstance message )
        => message.Options?.OkButtonClass;

    private static string GetTitleClass( MessageInstance message )
        => message.Options?.TitleClass;

    private static string GetMessageClass( MessageInstance message )
        => message.Options?.MessageClass;

    private string GetConfirmButtonText( MessageInstance message )
        => message.Options?.ConfirmButtonText ?? Localizer["Confirm"] ?? "Confirm";

    private static Color GetConfirmButtonColor( MessageInstance message )
        => message.Options?.ConfirmButtonColor ?? Color.Primary;

    private static string GetConfirmButtonClass( MessageInstance message )
        => message.Options?.ConfirmButtonClass;

    private string GetCancelButtonText( MessageInstance message )
        => message.Options?.CancelButtonText ?? Localizer["Cancel"] ?? "Cancel";

    private static Color GetCancelButtonColor( MessageInstance message )
        => message.Options?.CancelButtonColor ?? Color.Secondary;

    private static string GetCancelButtonClass( MessageInstance message )
        => message.Options?.CancelButtonClass;

    private static IFluentSpacing GetOkButtonPadding( MessageInstance message )
        => message.Options?.OkButtonPadding ?? Blazorise.Padding.Is2.OnX;

    private static IFluentSpacing GetCancelButtonPadding( MessageInstance message )
        => message.Options?.CancelButtonPadding ?? Blazorise.Padding.Is2.OnX;

    private static IFluentSpacing GetConfirmButtonPadding( MessageInstance message )
        => message.Options?.ConfirmButtonPadding ?? Blazorise.Padding.Is2.OnX;

    private static ModalSize GetSize( MessageInstance message )
        => message.Options?.Size ?? ModalSize.Default;

    private static IEnumerable<MessageOptionsChoice> GetChoices( MessageInstance message )
        => message.Options?.Choices ?? [];

    #endregion

    #region Properties

    /// <summary>
    /// Gets the active message instances.
    /// </summary>
    protected IEnumerable<MessageInstance> Messages
        => messages;

    /// <summary>
    /// Gets the latest message instance.
    /// </summary>
    protected MessageInstance LastMessage
        => messages.LastOrDefault();

    /// <summary>
    /// If true, modal will act as a prompt dialog.
    /// </summary>
    protected virtual bool IsConfirmation
        => MessageType == MessageType.Confirmation;

    /// <summary>
    /// If true, modal will act as a choice dialog.
    /// </summary>
    protected virtual bool IsChoice
        => MessageType == MessageType.Choice;

    /// <summary>
    /// If true, message will be centered.
    /// </summary>
    protected virtual bool CenterMessage
        => Size == ModalSize.Fullscreen ? false : Options?.CenterMessage ?? true;

    /// <summary>
    /// If true, message will be scrollable.
    /// </summary>
    protected virtual bool ScrollableMessage
        => Size == ModalSize.Fullscreen ? false : Options?.ScrollableMessage ?? true;

    /// <summary>
    /// If true, an icon will be shown along with the message.
    /// </summary>
    protected virtual bool ShowMessageIcon
        => Options?.ShowMessageIcon ?? true;

    /// <summary>
    /// If true, the close button will be visible.
    /// </summary>
    protected virtual bool ShowCloseButton
        => Options?.ShowCloseButton ?? false;

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
    protected virtual TextColor MessageIconColor => Options?.MessageIconColor ?? MessageType switch
    {
        MessageType.Info => TextColor.Info,
        MessageType.Success => TextColor.Success,
        MessageType.Warning => TextColor.Warning,
        MessageType.Error => TextColor.Danger,
        MessageType.Confirmation => TextColor.Secondary,
        _ => TextColor.Default,
    };

    /// <summary>
    /// Gets the OK button text.
    /// </summary>
    protected virtual string OkButtonText
        => Options?.OkButtonText ?? Localizer["OK"] ?? "OK";

    /// <summary>
    /// Gets the OK button color.
    /// </summary>
    protected virtual Color OkButtonColor
        => Options?.OkButtonColor ?? Color.Primary;

    /// <summary>
    /// Gets the OK button CSS class.
    /// </summary>
    protected virtual string OkButtonClass
        => Options?.OkButtonClass;

    /// <summary>
    /// Gets the css class for the title.
    /// </summary>
    protected virtual string TitleClass
        => Options?.TitleClass;

    /// <summary>
    /// Gets the css class for the message text.
    /// </summary>
    protected virtual string MessageClass
        => Options?.MessageClass;

    /// <summary>
    /// Gets the Confirm button text.
    /// </summary>
    protected virtual string ConfirmButtonText
        => Options?.ConfirmButtonText ?? Localizer["Confirm"] ?? "Confirm";

    /// <summary>
    /// Gets the confirm button color.
    /// </summary>
    protected virtual Color ConfirmButtonColor
        => Options?.ConfirmButtonColor ?? Color.Primary;

    /// <summary>
    /// Gets the confirm button CSS class.
    /// </summary>
    protected virtual string ConfirmButtonClass
        => Options?.ConfirmButtonClass;

    /// <summary>
    /// Gets the Cancel button text.
    /// </summary>
    protected virtual string CancelButtonText
        => Options?.CancelButtonText ?? Localizer["Cancel"] ?? "Cancel";

    /// <summary>
    /// Gets the cancel button color.
    /// </summary>
    protected virtual Color CancelButtonColor
        => Options?.CancelButtonColor ?? Color.Secondary;

    /// <summary>
    /// Gets the cancel button CSS class.
    /// </summary>
    protected virtual string CancelButtonClass
        => Options?.CancelButtonClass;

    /// <summary>
    /// Gets the OK button padding.
    /// </summary>
    protected virtual IFluentSpacing OkButtonPadding
        => Options?.OkButtonPadding ?? Blazorise.Padding.Is2.OnX;

    /// <summary>
    /// Gets the cancel button padding.
    /// </summary>
    protected virtual IFluentSpacing CancelButtonPadding
        => Options?.CancelButtonPadding ?? Blazorise.Padding.Is2.OnX;

    /// <summary>
    /// Gets the confirm button padding.
    /// </summary>
    protected virtual IFluentSpacing ConfirmButtonPadding
        => Options?.ConfirmButtonPadding ?? Blazorise.Padding.Is2.OnX;

    /// <summary>
    /// Gets the modal size.
    /// </summary>
    protected virtual ModalSize Size
        => Options?.Size ?? ModalSize.Default;

    /// <summary>
    /// Gets the choice buttons.
    /// </summary>
    protected virtual IEnumerable<MessageOptionsChoice> Choices
        => Options?.Choices ?? Enumerable.Empty<MessageOptionsChoice>();

    /// <summary>
    /// Specifies the <see cref="IMessageService"/> to which this dialog is responding.
    /// </summary>
    [Inject] protected IMessageService MessageService { get; set; }

    /// <summary>
    /// Specifies the DI registered <see cref="ITextLocalizerService"/>.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Specifies the DI registered <see cref="ITextLocalizer{MessageProvider}"/>.
    /// </summary>
    [Inject] protected ITextLocalizer<MessageProvider> Localizer { get; set; }

    /// <summary>
    /// Specifies the message type.
    /// </summary>
    [Parameter] public MessageType MessageType { get; set; }

    /// <summary>
    /// Specifies the message title.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Specifies the message content.
    /// </summary>
    [Parameter] public MarkupString Message { get; set; }

    /// <summary>
    /// Specifies the custom message options.
    /// </summary>
    [Parameter] public MessageOptions Options { get; set; }

    /// <summary>
    /// Occurs after the user respond with an action button.
    /// </summary>
    [Parameter] public TaskCompletionSource<object> Callback { get; set; }

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

    /// <summary>
    /// If true, the message dialog will be closed when the user presses the Escape key.
    /// Confirmation dialogs will treat Escape as a cancel action.
    /// </summary>
    [Parameter] public bool CloseOnEscape { get; set; }

    #endregion

    #region Classes

    /// <summary>
    /// Represents a single message dialog instance.
    /// </summary>
    protected sealed class MessageInstance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageInstance"/> class based on the provided message event arguments.    
        /// </summary>
        /// <param name="id">The unique identifier for the message instance.</param>
        /// <param name="messageEventArgs">The message event arguments.</param>
        public MessageInstance( int id, MessageEventArgs messageEventArgs )
        {
            Id = id;
            MessageType = messageEventArgs.MessageType;
            Message = messageEventArgs.Message;
            Title = messageEventArgs.Title;
            Options = messageEventArgs.Options;
            Callback = messageEventArgs.Callback;
            Visible = true;
        }

        /// <summary>
        /// Gets the unique message instance id.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        public MessageType MessageType { get; }

        /// <summary>
        /// Gets the message title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the message content.
        /// </summary>
        public MarkupString Message { get; }

        /// <summary>
        /// Gets the custom message options.
        /// </summary>
        public MessageOptions Options { get; }

        /// <summary>
        /// Gets the callback that completes when the user responds.
        /// </summary>
        public TaskCompletionSource<object> Callback { get; }

        /// <summary>
        /// Gets or sets whether the message modal is visible.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets whether the message modal has started closing.
        /// </summary>
        public bool Closing { get; set; }

        /// <summary>
        /// Gets or sets whether the message modal has been scheduled for removal.
        /// </summary>
        public bool Removing { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Modal"/> reference.
        /// </summary>
        public Modal ModalRef { get; set; }
    }

    #endregion
}