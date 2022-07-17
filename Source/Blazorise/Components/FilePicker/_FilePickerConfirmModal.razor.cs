#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class _FilePickerConfirmModal : ComponentBase
    {
        #region Members

        protected Modal modalRef;

        private Func<Task> onConfirm;

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

        protected internal async Task OpenModal( ConfirmOperation confirmOperation, Func<Task> onConfirm )
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
            this.onConfirm = onConfirm;
            await modalRef.Show();
        }

        protected internal Task CloseModal()
            => modalRef.Hide();

        private async Task Confirm()
        {
            await onConfirm.Invoke();
            await modalRef.Hide();
        }


        #endregion

        #region Properties

        [CascadingParameter] public Blazorise.FilePicker ParentFilePicker { get; set; }

        #endregion
    }

    
}