using System.Threading.Tasks;
using Blazorise.AntDesign.Modules;
using Microsoft.AspNetCore.Components;

namespace Blazorise.AntDesign.Components;

public partial class Switch<TValue>
{
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await JSWaveModule.Initialize( ElementRef );
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSWaveModule.Destroy( ElementRef );
        }

        await base.DisposeAsync( disposing );
    }

    [Inject] public AntDesignJSWaveModule JSWaveModule { get; set; }
}