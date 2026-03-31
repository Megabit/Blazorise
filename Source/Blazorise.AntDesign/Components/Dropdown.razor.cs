#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class Dropdown : Blazorise.Dropdown
{
    #region Methods

    protected override void OnInitialized()
    {
        ParentAddon?.Register( this );

        base.OnInitialized();
    }

    #endregion

    #region Properties

    [CascadingParameter] public AntDesign.Addon ParentAddon { get; set; }

    #endregion
}