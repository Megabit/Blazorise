using System.Threading.Tasks;
using Blazorise.AntDesign.Modules;
using Microsoft.AspNetCore.Components;

namespace Blazorise.AntDesign.Components;

public partial class Check<TValue>
{
    private ElementReference wrapperRef;

    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        if ( firstRender )
        {
            await JSWaveModule.Initialize( wrapperRef, ".ant-wave-target" );
        }

        await base.OnAfterRenderAsync( firstRender );
    }

    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSWaveModule.Destroy( wrapperRef );
        }

        await base.DisposeAsync( disposing );
    }

    [Inject] public AntDesignJSWaveModule JSWaveModule { get; set; }
}