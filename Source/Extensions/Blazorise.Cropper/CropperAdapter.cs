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
    public async ValueTask Crop( int startX, int startY, int endX, int endY ) => await cropper.NotifyCrop( startX, startY, endX, endY );

    [JSInvokable]
    public async ValueTask Zoom( double scale ) => await cropper.NotifyZoom( scale );

    [JSInvokable]
    public async ValueTask SelectionChanged( int x, int y, int width, int height ) => await cropper.NotifySelectionChanged( x, y, width, height );
}