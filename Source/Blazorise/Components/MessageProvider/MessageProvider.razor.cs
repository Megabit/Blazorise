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
            MessageType = e.MessageType;
            Message = e.Message;
            Title = e.Title;
            Options = e.Options;
            Callback = e.Callback;
            ModalVisible = true;

            StateHasChanged();
        } );
    }

    /// <summary>
    /// Handles the OK button click event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnOkClicked()
    {
        return InvokeAsync( async () =>
        {
            await Okayed.InvokeAsync();

            await ModalRef.Hide();
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
            await ModalRef.Hide();

            if ( IsConfirmation && Callback is not null && !Callback.Task.IsCompleted )
            {
                await InvokeAsync( () => Callback.SetResult( true ) );
            }

            await Confirmed.InvokeAsync();
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
            await ModalRef.Hide();
            await NotifyCanceled();
        } );
    }

    /// <summary>
    /// Handles the event when a choice button is clicked, hiding the modal and invoking callbacks as necessary.
    /// </summary>
    /// <param name="choice">Represents the button that was clicked, providing its key for further processing.</param>
    /// <returns>Returns a task that represents the asynchronous operation of handling the button click.</returns>
    protected Task OnChoiceClicked( MessageOptionsChoice choice )
    {
        return InvokeAsync( async () =>
        {
            await ModalRef.Hide();

            if ( IsChoice && Callback is not null && !Callback.Task.IsCompleted )
            {
                await InvokeAsync( () => Callback.SetResult( choice.Key ) );
            }

            await Confirmed.InvokeAsync();
        } );
    }

    /// <summary>
    /// Handles the <see cref="Modal"/> closing event.
    /// </summary>
    /// <param name="eventArgs">Provides the data for the modal closing event.</param>
    protected virtual async Task OnModalClosing( ModalClosingEventArgs eventArgs )
    {
        var isEscapeClosing = eventArgs.CloseReason == CloseReason.EscapeClosing;
        var isFocusLostClosing = eventArgs.CloseReason == CloseReason.FocusLostClosing;

        if ( isEscapeClosing && ( Options?.CloseOnEscape ?? CloseOnEscape ) )
        {
            await NotifyCanceled();

            return;
        }

        eventArgs.Cancel = ( Options?.BackgroundCancel ?? BackgroundCancel )
            && ( isEscapeClosing || isFocusLostClosing );
    }

    /// <summary>
    /// Notifies that the message dialog was canceled.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task NotifyCanceled()
    {
        if ( IsConfirmation && Callback is not null && !Callback.Task.IsCompleted )
        {
            await InvokeAsync( () => Callback.SetResult( false ) );
        }
        else if ( IsChoice && Callback is not null && !Callback.Task.IsCompleted )
        {
            await InvokeAsync( () => Callback.SetResult( null ) );
        }

        await Canceled.InvokeAsync();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="Modal"/> reference.
    /// </summary>
    protected Modal ModalRef { get; set; }

    /// <summary>
    /// Gets or sets whether the message modal is visible.
    /// </summary>
    protected bool ModalVisible { get; set; }

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
}