namespace Blazorise;

internal class MemoEditInitializeJSOptions
{
    public bool ReplaceTab { get; set; }
    public int TabSize { get; set; }
    public bool SoftTabs { get; set; }
    public bool AutoSize { get; set; }
}

internal class MemoEditUpdateJSOptions
{
    public JSOptionChange<bool> ReplaceTab { get; set; }
    public JSOptionChange<int> TabSize { get; set; }
    public JSOptionChange<bool> SoftTabs { get; set; }
    public JSOptionChange<bool> AutoSize { get; set; }
}