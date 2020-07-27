#region Using directives
using Blazorise;
using Blazorise.Snackbar;
using Microsoft.AspNetCore.Components;
#endregion

namespace ThemeApp
{
    public partial class ThemeSnackbarOptionsFragment : ComponentBase
    {
        #region Members
        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion

        #region Properties
        [CascadingParameter] public Theme Theme { get; set; }
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
