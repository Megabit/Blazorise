namespace Blazorise;

internal class TimePickerInitializeJSOptions
{
    public string DisplayFormat { get; set; }
    public bool TimeAs24hr { get; set; }
    public string Default { get; set; }
    public string Min { get; set; }
    public string Max { get; set; }
    public bool Disabled { get; set; }
    public bool ReadOnly { get; set; }
    public object Localization { get; set; }
    public bool Inline { get; set; }
    public string Placeholder { get; set; }
    public bool StaticPicker { get; set; }
}

internal class TimePickerUpdateJSOptions
{
    public JSOptionChange<string> DisplayFormat { get; set; }
    public JSOptionChange<bool> TimeAs24hr { get; set; }
    public JSOptionChange<string> Min { get; set; }
    public JSOptionChange<string> Max { get; set; }
    public JSOptionChange<bool> Disabled { get; set; }
    public JSOptionChange<bool> ReadOnly { get; set; }
    public JSOptionChange<bool> Inline { get; set; }
    public JSOptionChange<string> Placeholder { get; set; }
    public JSOptionChange<bool> StaticPicker { get; set; }
}