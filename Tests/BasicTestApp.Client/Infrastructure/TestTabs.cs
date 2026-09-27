using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;

namespace BasicTestApp.Client.Infrastructure;

/// <summary>
/// Reports completion of the real Tabs initialization to the test fixture.
/// </summary>
public class TestTabs : Tabs
{
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await base.OnAfterRenderAsync( firstRender );

        if ( firstRender )
        {
            await Initialized.InvokeAsync();
        }
    }

    [Parameter] public EventCallback Initialized { get; set; }
}