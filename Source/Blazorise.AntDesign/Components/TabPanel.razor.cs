using System.Threading.Tasks;
using Blazorise;

namespace Blazorise.AntDesign.Components;

public partial class TabPanel : Blazorise.TabPanel
{
    private bool lazyLoaded;

    protected override Task OnParametersSetAsync()
    {
        if ( Active )
        {
            lazyLoaded = RenderMode == TabsRenderMode.LazyLoad;
        }

        return base.OnParametersSetAsync();
    }

    protected bool LazyLoaded => lazyLoaded;
}