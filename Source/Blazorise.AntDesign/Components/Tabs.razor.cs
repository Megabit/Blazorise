#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise.AntDesign.Components;

public partial class Tabs : Blazorise.Tabs
{
    #region Methods

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            // All child tab items and panels must be loaded before we can be sure that we can refresh state.
            // This is needed because selected tab will not work unless we know all properly initialized.
            if ( TabItems.Count > 0 && TabItems.Count == TabPanels.Count )
            {
                await InvokeAsync( StateHasChanged );
            }
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    #endregion

    #region Properties

    protected string RootClassNames => $"{ClassNames} {PositionClassNames}".Trim();

    string TabsNavClassNames => "ant-tabs-nav";

    string PositionClassNames => TabPosition switch
    {
        TabPosition.Start => "ant-tabs-left",
        TabPosition.End => "ant-tabs-right",
        TabPosition.Bottom => "ant-tabs-bottom",
        _ => "ant-tabs-top",
    };

    string AriaOrientation => TabPosition is TabPosition.Start or TabPosition.End ? "vertical" : "horizontal";

    #endregion
}