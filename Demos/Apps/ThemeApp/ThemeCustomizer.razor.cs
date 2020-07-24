#region Using directives
using Blazorise;
using Blazorise.Snackbar;
using Microsoft.AspNetCore.Components;
#endregion

namespace ThemeApp
{
    public partial class ThemeCustomizer : ComponentBase
    {
        #region Members
        #endregion

        #region Constructors
        #endregion

        #region Methods
        private void OnSelectedTabChanged( string name )
        {
            SelectedTab = name;
        }
        #endregion

        #region Properties
        [CascadingParameter] public Theme Theme { get; set; }
        private string SelectedTab { get; set; } = "General";
        private bool AlertSuccessVisible { get; set; } = true;
        private bool AlertInformationVisible { get; set; } = true;
        private bool AlertWarningVisible { get; set; } = true;
        private bool AlertErrorVisible { get; set; } = true;
        private bool ButtonOutline { get; set; } = false;
        private bool ButtonActive { get; set; } = false;
        private bool ButtonDisabled { get; set; } = false;
        private bool ButtonLoading { get; set; } = false;
        private Modal ModalPreview { get; set; }
        private Snackbar SnackbarPreview { get; set; }
        private Snackbar SnackbarPreviewPrimary { get; set; }
        private Snackbar SnackbarPreviewSecondary { get; set; }
        private Snackbar SnackbarPreviewSuccess { get; set; }
        private Snackbar SnackbarPreviewDanger { get; set; }
        private Snackbar SnackbarPreviewWarning { get; set; }
        private Snackbar SnackbarPreviewInfo { get; set; }
        private Snackbar SnackbarPreviewLight { get; set; }
        private Snackbar SnackbarPreviewDark { get; set; }
        #endregion
    }
}