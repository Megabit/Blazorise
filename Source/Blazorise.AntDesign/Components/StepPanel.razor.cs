using System.Threading.Tasks;
using Blazorise;

namespace Blazorise.AntDesign.Components;

public partial class StepPanel : Blazorise.StepPanel
{
    private bool lazyLoaded;

    protected override Task OnParametersSetAsync()
    {
        if ( Active )
        {
            lazyLoaded = RenderMode == StepsRenderMode.LazyLoad;
        }

        return base.OnParametersSetAsync();
    }

    protected bool LazyLoaded => lazyLoaded;
}