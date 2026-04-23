#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.PdfViewer;

/// <summary>
/// Internal component used by <see cref="PdfViewer"/> to collect a download filename from the user.
/// </summary>
public partial class _PdfViewerDownloadPrompt : BaseComponent
{
    #region Members

    private string fileName = string.Empty;

    private Validations validations;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        fileName = FileName ?? string.Empty;

        base.OnInitialized();
    }

    private async Task OnSubmitClicked()
    {
        if ( validations is not null && !await validations.ValidateAll() )
            return;

        await SubmitRequested.InvokeAsync( fileName );
    }

    private Task OnFileNameChanged( string value )
    {
        fileName = value;

        return Task.CompletedTask;
    }

    private void ValidateFileName( ValidatorEventArgs eventArgs )
    {
        var fileNameValue = eventArgs.Value?.ToString();

        eventArgs.Status = string.IsNullOrWhiteSpace( fileNameValue )
            ? ValidationStatus.Error
            : ValidationStatus.None;

        eventArgs.ErrorText = eventArgs.Status == ValidationStatus.Error
            ? RequiredFileNameValidationMessage ?? "Filename is required."
            : null;
    }

    private Task OnCancelClicked()
        => CancelRequested.InvokeAsync();

    #endregion

    #region Properties

    /// <summary>
    /// Specifies the title shown to the user.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Specifies the message shown to the user.
    /// </summary>
    [Parameter] public string Message { get; set; }

    /// <summary>
    /// Specifies the initial filename shown inside the input.
    /// </summary>
    [Parameter] public string FileName { get; set; }

    /// <summary>
    /// Specifies the placeholder shown inside the filename input.
    /// </summary>
    [Parameter] public string FileNamePlaceholder { get; set; }

    /// <summary>
    /// Specifies the submit button text.
    /// </summary>
    [Parameter] public string ConfirmButtonText { get; set; }

    /// <summary>
    /// Specifies the cancel button text.
    /// </summary>
    [Parameter] public string CancelButtonText { get; set; }

    /// <summary>
    /// Specifies the message shown when a filename is not entered.
    /// </summary>
    [Parameter] public string RequiredFileNameValidationMessage { get; set; }

    /// <summary>
    /// Notifies when user confirms the filename input.
    /// </summary>
    [Parameter] public EventCallback<string> SubmitRequested { get; set; }

    /// <summary>
    /// Notifies when user cancels the prompt.
    /// </summary>
    [Parameter] public EventCallback CancelRequested { get; set; }

    #endregion
}