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
/// Internal confirmation dialog used by the <see cref="FilePicker"/> component.
/// </summary>
public partial class _FilePickerConfirmModal : ComponentBase
{
    #region Members

    /// <summary>
    /// Reference to the modal component.
    /// </summary>
    protected Modal modalRef;

    /// <summary>
    /// Callback action for the confirm button.
    /// </summary>
    private Func<Task> confirmed;

    /// <summary>
    /// Identifies the FilePicker's popup confirm operation.
    /// </summary>
    public enum ConfirmOperation
    {
        /// <summary>
        /// Remove file confirm operation.
        /// </summary>
        RemoveFile,

        /// <summary>
        /// Clear confirm operation.
        /// </summary>
        Clear,

        /// <summary>
        /// Cancels ongoing upload.
        /// </summary>
        CancelUpload
    }

    private string title = string.Empty;

    private string body = string.Empty;

    #endregion

    #region Methods

    /// <summary>
    /// Opens the modal.
    /// </summary>
    /// <param name="confirmOperation">Type of the confirm operation.</param>
    /// <param name="confirmed">Function to execute after the action was confirmed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task OpenModal( ConfirmOperation confirmOperation, Func<Task> confirmed )
    {
        switch ( confirmOperation )
        {
            case ConfirmOperation.RemoveFile:
                title = "Remove";
                body = "Are you sure you want to remove the file?";
                break;
            case ConfirmOperation.Clear:
                title = "Clear";
                body = "Are you sure you want to clear all files?";
                break;
            case ConfirmOperation.CancelUpload:
                title = "Cancel";
                body = "Are you sure you want to cancel the upload?";
                break;
            default:
                break;
        }

        this.confirmed = confirmed;

        await modalRef.Show();
    }

    /// <summary>
    /// Closes the modal.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected internal Task OnCloseModalClicked()
        => modalRef.Hide();

    private async Task OnConfirmClicked()
    {
        if ( confirmed is not null )
            await confirmed.Invoke();

        if ( modalRef is not null )
            await modalRef.Hide();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Parent file picker.
    /// </summary>
    [CascadingParameter] public Blazorise.FilePicker ParentFilePicker { get; set; }

    #endregion
}