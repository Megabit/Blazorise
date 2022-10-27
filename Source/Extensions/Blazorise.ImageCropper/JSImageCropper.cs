using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Blazorise.ImageCropper;

internal class JSImageCropper
{
    private readonly IJSRuntime runtime;
    private readonly IJSObjectReference objReference;

    public JSImageCropper( IJSRuntime runtime, IJSObjectReference objReference )
    {
        this.runtime = runtime;
        this.objReference = objReference;
    }
    public bool IsDisposed { get; private set; }

    public async ValueTask DisposeAsync()
    {
        if ( IsDisposed )
            return;

        IsDisposed = true;

        await objReference.InvokeVoidAsync( "destroy" );
        await objReference.DisposeAsync();
    }

    public async ValueTask<string> CropImage( int width, int height )
    {
        if ( IsDisposed )
            throw new ObjectDisposedException( nameof( objReference ) );

        return await objReference.InvokeAsync<string>( "crop", width, height );
    }

    public async ValueTask UpdateAsync( JSCropperOptions options )
    {
        if ( IsDisposed )
            return;

        await objReference.InvokeVoidAsync( "update", options );
    }
}