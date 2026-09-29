#region Using directives
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
#endregion

namespace BasicTestApp.Client.Infrastructure;

/// <summary>
/// Reports completion of the real Steps keyboard initialization to the test fixture.
/// </summary>
public class TestSteps : Steps
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