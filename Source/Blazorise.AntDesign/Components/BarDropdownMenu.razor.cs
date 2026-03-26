#region Using directives
using System.Threading.Tasks;
using Blazorise.AntDesign.Modules;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class BarDropdownMenu
{
    #region Members

    protected ElementReference ContainerRef;

    #endregion

    #region Methods

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await base.OnAfterRenderAsync( firstRender );

        if ( UsePopupContainer && !UseHorizontalPopupPlacement )
        {
            await JSBarModule.UpdatePopupPlacement( ContainerRef, ParentDropdownState?.Visible == true );
        }
    }

    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && UsePopupContainer && !UseHorizontalPopupPlacement )
        {
            await JSBarModule.ResetPopupPlacement( ContainerRef );
        }

        await base.DisposeAsync( disposing );
    }

    #endregion

    #region Properties

    [Inject] public AntDesignJSBarModule JSBarModule { get; set; }

    #endregion
}