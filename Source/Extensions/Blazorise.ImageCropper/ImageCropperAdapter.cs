using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Blazorise.ImageCropper;

internal class ImageCropperAdapter
{
    private readonly ImageCropper imageCropper;

    public ImageCropperAdapter( ImageCropper imageCropper )
    {
        this.imageCropper = imageCropper;
    }

    [JSInvokable]
    public async ValueTask CropStart() => await imageCropper.NotifyCropStart();

    [JSInvokable]
    public async ValueTask CropMove() => await imageCropper.NotifyCropMove();

    [JSInvokable]
    public async ValueTask CropEnd() => await imageCropper.NotifyCropEnd();

    [JSInvokable]
    public async ValueTask Crop() => await imageCropper.NotifyCrop();

    [JSInvokable]
    public async ValueTask Zoom() => await imageCropper.NotifyZoom();
}