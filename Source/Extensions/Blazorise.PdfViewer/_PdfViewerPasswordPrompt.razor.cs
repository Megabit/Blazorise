#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.PdfViewer;

/// <summary>
/// Internal component used by <see cref="PdfViewer"/> to collect a password from the user.
/// </summary>
public partial class _PdfViewerPasswordPrompt : BaseComponent
{
    #region Members

    private string password = string.Empty;

    private Validations validations;

    #endregion

    #region Methods

    private async Task OnSubmitClicked()
    {
        if ( validations is not null && !await validations.ValidateAll() )
            return;

        await SubmitRequested.InvokeAsync( password );
    }

    private Task OnPasswordChanged( string value )
    {
        password = value;

        return Task.CompletedTask;
    }

    private void ValidatePassword( ValidatorEventArgs eventArgs )
    {
        var passwordValue = eventArgs.Value?.ToString();

        eventArgs.Status = string.IsNullOrWhiteSpace( passwordValue )
            ? ValidationStatus.Error
            : ValidationStatus.None;

        eventArgs.ErrorText = eventArgs.Status == ValidationStatus.Error
            ? RequiredPasswordValidationMessage ?? "Password is required."
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
    /// Specifies the placeholder shown inside the password input.
    /// </summary>
    [Parameter] public string PasswordPlaceholder { get; set; }

    /// <summary>
    /// Specifies the submit button text.
    /// </summary>
    [Parameter] public string ConfirmButtonText { get; set; }

    /// <summary>
    /// Specifies the cancel button text.
    /// </summary>
    [Parameter] public string CancelButtonText { get; set; }

    /// <summary>
    /// Specifies the message shown when a password is not entered.
    /// </summary>
    [Parameter] public string RequiredPasswordValidationMessage { get; set; }

    /// <summary>
    /// Notifies when user confirms the password input.
    /// </summary>
    [Parameter] public EventCallback<string> SubmitRequested { get; set; }

    /// <summary>
    /// Notifies when user cancels the prompt.
    /// </summary>
    [Parameter] public EventCallback CancelRequested { get; set; }

    #endregion
}