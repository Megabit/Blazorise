namespace Blazorise.ImageCropper;

internal sealed class JSCropperOptions
{
    public double? AspectRatio { get; set; } = 1.0;
    public int ViewMode { get; set; } = 1;
    public string Preview { get; set; }
    public int? Radius { get; set; }
}