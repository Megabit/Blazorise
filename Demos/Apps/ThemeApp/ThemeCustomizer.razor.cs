#region Using directives
using Blazorise;
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
        #endregion
    }
}