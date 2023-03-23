using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.RichTextEdit.Rooster;

public partial class RichTextEdit : BaseComponent
{
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await base.OnAfterRenderAsync( firstRender );

        if ( firstRender )
        {
            JSModule ??= new JSRoosterModule( JSRuntime, VersionProvider );
            var adapter = DotNetObjectReference.Create( this );

            await JSModule.Initialize( adapter, ElementRef, ElementId, default );
        }
    }

    internal JSRoosterModule JSModule { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private IVersionProvider VersionProvider { get; set; }
}

