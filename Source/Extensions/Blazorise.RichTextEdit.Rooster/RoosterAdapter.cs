using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Blazorise.RichTextEdit.Rooster;

internal class RoosterAdapter
{
    private readonly RichTextEdit rte;

    public RoosterAdapter( RichTextEdit rte )
    {
        this.rte = rte;
    }

    [JSInvokable]
    public Task OnContentChanged( string html )
        => rte.UpdateInternalContent( html );

    [JSInvokable]
    public Task OnFormatStateChanged( FormatState state )
        => rte.UpdateInternalFormatState( state );
}