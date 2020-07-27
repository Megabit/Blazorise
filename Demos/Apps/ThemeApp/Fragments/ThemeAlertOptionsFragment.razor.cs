#region Using directives
using Blazorise;
using Microsoft.AspNetCore.Components;
#endregion

namespace ThemeApp
{
    public partial class ThemeAlertOptionsFragment : ComponentBase
    {
        #region Members
        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion

        #region Properties
        [CascadingParameter] public Theme Theme { get; set; }
        private bool AlertSuccessVisible { get; set; } = true;
        private bool AlertInformationVisible { get; set; } = true;
        private bool AlertWarningVisible { get; set; } = true;
        private bool AlertErrorVisible { get; set; } = true;
        #endregion
    }
}
