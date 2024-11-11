using Blazorise.Modules.JSOptions;

namespace Blazorise.SignaturePad;

public class SignaturePadInitializeJSOptions
{
    public string DataUrl { get; set; }
    public double DotSize { get; set; }
    public double MinLineWidth { get; set; }
    public double MaxLineWidth { get; set; }
    public int Throttle { get; set; }
    public int MinDistance { get; set; }
    public string BackgroundColor { get; set; }
    public string PenColor { get; set; }
    public double VelocityFilterWeight { get; set; }
    public string ImageType { get; set; }
    public double? ImageQuality { get; set; }
    public bool IncludeImageBackgroundColor { get; set; }
    public bool ReadOnly { get; set; }
}

public class SignaturePadUpdateJSOptions
{
    public JSOptionChange<string> DataUrl { get; set; }
    public JSOptionChange<double> DotSize { get; set; }
    public JSOptionChange<double> MinLineWidth { get; set; }
    public JSOptionChange<double> MaxLineWidth { get; set; }
    public JSOptionChange<int> Throttle { get; set; }
    public JSOptionChange<int> MinDistance { get; set; }
    public JSOptionChange<string> BackgroundColor { get; set; }
    public JSOptionChange<string> PenColor { get; set; }
    public JSOptionChange<double> VelocityFilterWeight { get; set; }
    public JSOptionChange<string> ImageType { get; set; }
    public JSOptionChange<double?> ImageQuality { get; set; }
    public JSOptionChange<bool> IncludeImageBackgroundColor { get; set; }
    public JSOptionChange<bool> ReadOnly { get; set; }
}