#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Options to override message dialog appearance.
/// </summary>
public class MessageOptions
{
    /// <summary>
    /// If true, the message dialogue will be centered on the screen.
    /// </summary>
    public bool CenterMessage { get; set; }

    /// <summary>
    /// If true, the message dialogue will be scrollable.
    /// </summary>
    public bool ScrollableMessage { get; set; }

    /// <summary>
    /// If true, the message dialogue will show the large icon for the current message type.
    /// </summary>
    public bool ShowMessageIcon { get; set; }

    /// <summary>
    /// Overrides the build-in message icon.
    /// </summary>
    public object MessageIcon { get; set; }

    /// <summary>
    /// Overrides the build-in message icon color.
    /// </summary>
    public TextColor MessageIconColor { get; set; }

    /// <summary>
    /// Custom text for the Ok button.
    /// </summary>
    public string OkButtonText { get; set; }

    /// <summary>
    /// Custom icon for the Ok button.
    /// </summary>
    public object OkButtonIcon { get; set; }

    /// <summary>
    /// Custom icon color for the Ok button.
    /// </summary>
    public TextColor OkButtonIconColor { get; set; }

    /// <summary>
    /// Custom color of the Ok button.
    /// </summary>
    public Color OkButtonColor { get; set; }

    /// <summary>
    /// Custom OK button CSS class.
    /// </summary>
    public string OkButtonClass { get; set; }

    /// <summary>
    /// Custom OK button padding.
    /// </summary>
    public IFluentSpacing OkButtonPadding { get; set; }

    /// <summary>
    /// Custom cancel button padding.
    /// </summary>
    public IFluentSpacing CancelButtonPadding { get; set; }

    /// <summary>
    /// Custom confirm button padding.
    /// </summary>
    public IFluentSpacing ConfirmButtonPadding { get; set; }

    /// <summary>
    /// Custom text for the Confirmation button.
    /// </summary>
    public string ConfirmButtonText { get; set; }

    /// <summary>
    /// Custom icon for the Confirmation button.
    /// </summary>
    public object ConfirmButtonIcon { get; set; }

    /// <summary>
    /// Custom icon color for the Confirmation button.
    /// </summary>
    public TextColor ConfirmButtonIconColor { get; set; }

    /// <summary>
    /// Custom color of the Confirmation button.
    /// </summary>
    public Color ConfirmButtonColor { get; set; }

    /// <summary>
    /// Custom Confirmation button CSS class.
    /// </summary>
    public string ConfirmButtonClass { get; set; }

    /// <summary>
    /// Custom text for the Cancel button.
    /// </summary>
    public string CancelButtonText { get; set; }

    /// <summary>
    /// Custom icon for the Cancel button.
    /// </summary>
    public object CancelButtonIcon { get; set; }

    /// <summary>
    /// Custom icon color for the Cancel button.
    /// </summary>
    public TextColor CancelButtonIconColor { get; set; }

    /// <summary>
    /// Custom color of the Cancel button.
    /// </summary>
    public Color CancelButtonColor { get; set; }

    /// <summary>
    /// Custom Cancel button CSS class.
    /// </summary>
    public string CancelButtonClass { get; set; }

    /// <summary>
    /// If true, the modal close button will be visible.
    /// </summary>
    public bool ShowCloseButton { get; set; }

    /// <summary>
    /// Custom title CSS class.
    /// </summary>
    public string TitleClass { get; set; }

    /// <summary>
    /// Custom message CSS class.
    /// </summary>
    public string MessageClass { get; set; }

    /// <summary>
    /// Defines the message dialog size.
    /// </summary>
    public ModalSize Size { get; set; }

    /// <summary>
    /// Represents a collection of options for message buttons. Each option is of type <see cref="MessageOptionsChoice"/>.
    /// </summary>
    public IEnumerable<MessageOptionsChoice> Choices { get; set; }

    /// <summary>
    /// Creates the default message options.
    /// </summary>
    /// <returns>Default message options.</returns>
    public static MessageOptions Default => new()
    {
        CenterMessage = true,
        ScrollableMessage = true,
        ShowMessageIcon = true,
        ShowCloseButton = true,
        OkButtonText = null,
        OkButtonColor = Color.Primary,
        OkButtonPadding = Padding.Is2.OnX,
        CancelButtonText = null,
        CancelButtonColor = Color.Secondary,
        CancelButtonPadding = Padding.Is2.OnX,
        ConfirmButtonText = null,
        ConfirmButtonColor = Color.Primary,
        ConfirmButtonPadding = Padding.Is2.OnX,
        Size = ModalSize.Default,
    };
}