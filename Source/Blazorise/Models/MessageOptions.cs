namespace Blazorise
{
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
        /// Creates the default message options.
        /// </summary>
        /// <returns>Default message options.</returns>
        public static MessageOptions Default => new()
        {
            CenterMessage = true,
            ScrollableMessage = true,
            ShowMessageIcon = true,
            ShowCloseButton = true,
            OkButtonText = "Ok",
            OkButtonColor = Color.Primary,
            CancelButtonText = "Cancel",
            CancelButtonColor = Color.Secondary,
            ConfirmButtonText = "Yes",
            ConfirmButtonColor = Color.Primary,
        };
    }
}
