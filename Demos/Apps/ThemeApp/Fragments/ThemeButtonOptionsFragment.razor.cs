#region Using directives
using Blazorise;
using Microsoft.AspNetCore.Components;
#endregion

namespace ThemeApp
{
    public partial class ThemeButtonOptionsFragment : ComponentBase
    {
        #region Members
        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion

        #region Properties
        [CascadingParameter] public Theme Theme { get; set; }
        private bool ButtonOutline { get; set; } = false;
        private bool ButtonActive { get; set; } = false;
        private bool ButtonDisabled { get; set; } = false;
        private bool ButtonLoading { get; set; } = false;
        #endregion
    }
}
