using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Blazorise.ImageCropper;

public class JSObjectUrl
{
    private readonly IJSRuntime runtime;
    private readonly string url;

    public JSObjectUrl( string url, IJSRuntime runtime )
    {
        url = url;
        runtime = runtime;
    }

    public string Url => IsDisposed ? throw new ObjectDisposedException( nameof( Url ) ) : url;
    public bool IsDisposed { get; private set; }

    public async ValueTask DisposeAsync()
    {
        if ( IsDisposed )
            return;

        IsDisposed = true;
        await runtime.InvokeVoidAsync( "URL.revokeObjectURL", url );
    }
}