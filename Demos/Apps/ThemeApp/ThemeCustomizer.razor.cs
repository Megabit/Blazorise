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
        private string SelectedTab { get; set; } = "General";
        [CascadingParameter] public Theme Theme { get; set; }
        #endregion
    }
}