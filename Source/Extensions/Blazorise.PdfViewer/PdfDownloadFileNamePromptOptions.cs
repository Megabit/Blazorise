namespace Blazorise.PdfViewer;

/// <summary>
/// Provides configuration options for the default download filename prompt shown by <see cref="PdfViewer"/>.
/// </summary>
public class PdfDownloadFileNamePromptOptions
{
    /// <summary>
    /// Gets or sets the title text displayed in the prompt.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the message text displayed in the prompt.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text for the filename input.
    /// </summary>
    public string FileNamePlaceholder { get; set; }

    /// <summary>
    /// Gets or sets the text of the confirm button.
    /// </summary>
    public string ConfirmButtonText { get; set; }

    /// <summary>
    /// Gets or sets the text of the cancel button.
    /// </summary>
    public string CancelButtonText { get; set; }

    /// <summary>
    /// Gets or sets the validation message shown when the filename is not entered.
    /// </summary>
    public string RequiredFileNameValidationMessage { get; set; }

    /// <summary>
    /// Gets or sets modal customization options for the prompt.
    /// </summary>
    public ModalInstanceOptions ModalOptions { get; set; }
}