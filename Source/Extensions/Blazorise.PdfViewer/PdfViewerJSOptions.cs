using Blazorise.Modules.JSOptions;

namespace Blazorise.PdfViewer;

public class PdfViewerInitializeJSOptions
{
    public string Source { get; set; }
    public int PageNumber { get; set; }
    public double Scale { get; set; }
    public double Rotation { get; set; }
}

public class PdfViewerUpdateJSOptions
{
    public JSOptionChange<string> Source { get; set; }
    public JSOptionChange<int> PageNumber { get; set; }
    public JSOptionChange<double> Scale { get; set; }
    public JSOptionChange<double> Rotation { get; set; }
}