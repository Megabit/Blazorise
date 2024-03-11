#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise.AntDesign;

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

    string TabsBarClassNames
        => $"ant-tabs-nav {ItemsPositionClassNames} {( IsCards && TabPosition == TabPosition.Top || TabPosition == TabPosition.Bottom ? "ant-tabs-card-bar" : "" )}";

    protected string ItemsPositionClassNames => TabPosition switch
    {
        TabPosition.Start => "ant-tabs-left-bar",
        TabPosition.End => "ant-tabs-right-bar",
        TabPosition.Bottom => "ant-tabs-bottom-bar",
        _ => "ant-tabs-top-bar",
    };

    protected string ContentPositionClassNames => TabPosition switch
    {
        TabPosition.Start => "ant-tabs-content-left",
        TabPosition.End => "ant-tabs-content-right",
        TabPosition.Bottom => "ant-tabs-content-bottom",
        _ => "ant-tabs-content-top",
    };

    #endregion
}