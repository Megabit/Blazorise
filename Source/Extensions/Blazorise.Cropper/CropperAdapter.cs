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

    [JSInvokable( "CropStart" )]
    public async ValueTask CropStart() => await cropper.NotifyCropStart();

    [JSInvokable( "CropMove" )]
    public async ValueTask CropMove() => await cropper.NotifyCropMove();

    [JSInvokable( "CropEnd" )]
    public async ValueTask CropEnd() => await cropper.NotifyCropEnd();

    [JSInvokable( "Crop" )]
    public async ValueTask Crop( double startX, double startY, double endX, double endY ) => await cropper.NotifyCrop( startX, startY, endX, endY );

    [JSInvokable( "Zoom" )]
    public async ValueTask Zoom( double scale ) => await cropper.NotifyZoom( scale );

    [JSInvokable( "SelectionChanged" )]
    public async ValueTask SelectionChanged( double x, double y, double width, double height ) => await cropper.NotifySelectionChanged( x, y, width, height );

    [JSInvokable( "ImageReady" )]
    public async ValueTask ImageReady() => await cropper.NotifyImageReady();
}
