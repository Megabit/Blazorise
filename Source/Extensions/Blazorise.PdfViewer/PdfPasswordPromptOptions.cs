namespace Blazorise.PdfViewer;

/// <summary>
/// Provides configuration options for the default password prompt shown by <see cref="PdfViewer"/>.
/// </summary>
public class PdfPasswordPromptOptions
{
    /// <summary>
    /// Gets or sets the title text displayed in the prompt.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the message text displayed when a password is required.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the message text displayed when the entered password is incorrect.
    /// </summary>
    public string IncorrectPasswordMessage { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text for the password input.
    /// </summary>
    public string PasswordPlaceholder { get; set; }

    /// <summary>
    /// Gets or sets the text of the confirm button.
    /// </summary>
    public string ConfirmButtonText { get; set; }

    /// <summary>
    /// Gets or sets the text of the cancel button.
    /// </summary>
    public string CancelButtonText { get; set; }

    /// <summary>
    /// Gets or sets the validation message shown when the password is not entered.
    /// </summary>
    public string RequiredPasswordValidationMessage { get; set; }

    /// <summary>
    /// Gets or sets modal customization options for the prompt.
    /// </summary>
    public ModalInstanceOptions ModalOptions { get; set; }
}