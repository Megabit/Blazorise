using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazorise.RichTextEdit.Rooster;

/// <summary>
/// RichTextEdit based on rooster.js
/// </summary>
public partial class RichTextEdit : BaseComponent
{
    private DotNetObjectReference<RoosterAdapter> adapter;

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await base.OnAfterRenderAsync( firstRender );

        if ( firstRender )
        {
            JSModule ??= new JSRoosterModule( JSRuntime, VersionProvider );
            adapter = DotNetObjectReference.Create( new RoosterAdapter( this ) );

            await JSModule.Initialize( adapter, ElementRef, ElementId, default );
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-rte-rooster" );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

            await JSModule.SafeDisposeAsync();

            if ( adapter != null )
            {
                adapter.Dispose();
                adapter = null;
            }
        }

        await base.DisposeAsync( disposing );
    }

    internal JSRoosterModule JSModule { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private IVersionProvider VersionProvider { get; set; }
}

