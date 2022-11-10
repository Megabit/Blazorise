using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Blazorise.Cropper;

internal class CropperAdapter
{
    private readonly Cropper cropper;

    public CropperAdapter( Cropper cropper )
    {
        this.cropper = cropper;
    }

    [JSInvokable]
    public async ValueTask CropStart() => await cropper.NotifyCropStart();

    [JSInvokable]
    public async ValueTask CropMove() => await cropper.NotifyCropMove();

    [JSInvokable]
    public async ValueTask CropEnd() => await cropper.NotifyCropEnd();

    [JSInvokable]
    public async ValueTask Crop() => await cropper.NotifyCrop();

    [JSInvokable]
    public async ValueTask Zoom() => await cropper.NotifyZoom();
}