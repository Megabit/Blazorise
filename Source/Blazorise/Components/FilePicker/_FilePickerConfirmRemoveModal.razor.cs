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
    public partial class _FilePickerConfirmRemoveModal : ComponentBase
    {
        #region Members

        protected Modal modalRef;

        private Func<Task> onConfirm;

        #endregion

        #region Methods

        protected internal async Task OpenModal(Func<Task> onConfirm )
        {
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