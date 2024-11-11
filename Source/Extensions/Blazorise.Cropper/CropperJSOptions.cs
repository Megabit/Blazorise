using Blazorise.Modules.JSOptions;

namespace Blazorise.Cropper;

public class CropperInitializeJSOptions
{
    public string Source { get; set; }
    public string Alt { get; set; }
    public bool Enabled { get; set; }
    public bool ShowBackground { get; set; }
    public CropperImageOptions Image { get; set; }
    public CropperSelectionJSOptions Selection { get; set; }
    public CropperGridOptions Grid { get; set; }
}

public class CropperUpdateJSOptions
{
    public JSOptionChange<string> Source { get; set; }
    public JSOptionChange<string> Alt { get; set; }
    public JSOptionChange<string> CrossOrigin { get; set; }
    public JSOptionChange<CropperImageOptions> Image { get; set; }
    public JSOptionChange<CropperSelectionJSOptions> Selection { get; set; }
    public JSOptionChange<CropperGridOptions> Grid { get; set; }
    public JSOptionChange<bool> Enabled { get; set; }
}



public class CropperSelectionJSOptions
{
    public double? AspectRatio { get; set; }
    public double? InitialAspectRatio { get; set; }
    public bool? InitialCoverage { get; set; }
    public bool Movable { get; set; }
    public bool Resizable { get; set; }
    public bool Zoomable { get; set; }
    public bool Keyboard { get; set; }
    public bool Outlined { get; set; }
}
