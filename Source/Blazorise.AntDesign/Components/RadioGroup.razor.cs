using System.Threading.Tasks;
using Blazorise.AntDesign.Modules;
using Microsoft.AspNetCore.Components;

namespace Blazorise.AntDesign.Components;

public partial class RadioGroup<TValue> : Blazorise.RadioGroup<TValue>
{
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( Buttons )
        {
            await JSSegmentedModule.Update( ElementRef, ElementId );
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    [Inject] public AntDesignJSSegmentedModule JSSegmentedModule { get; set; }
}