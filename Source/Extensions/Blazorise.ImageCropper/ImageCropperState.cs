namespace Blazorise.ImageCropper;

internal class ImageCropperState
{
    public JSCropperOptions Options { get; set; }
    public bool Enabled { get; set; } = true;
    public string Source { get; set; }
}