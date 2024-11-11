using System.Collections.Generic;

namespace Blazorise;

internal class DatePickerInitializeJSOptions
{
    public DateInputMode InputMode { get; set; }
    public string SelectionMode { get; set; }
    public int FirstDayOfWeek { get; set; }
    public string DisplayFormat { get; set; }
    public string InputFormat { get; set; }
    public bool TimeAs24hr { get; set; }
    public object DefaultDate { get; set; }
    public string Min { get; set; }
    public string Max { get; set; }
    public bool Disabled { get; set; }
    public bool ReadOnly { get; set; }
    public IEnumerable<string> DisabledDates { get; set; }
    public IEnumerable<string> EnabledDates { get; set; }
    public IEnumerable<int> DisabledDays { get; set; }
    public object Localization { get; set; }
    public bool Inline { get; set; }
    public bool DisableMobile { get; set; }
    public string Placeholder { get; set; }
    public bool StaticPicker { get; set; }
    public object ValidationStatus { get; set; }
}

internal class DatePickerUpdateJSOptions
{
    public JSOptionChange<int> FirstDayOfWeek { get; set; }
    public JSOptionChange<string> DisplayFormat { get; set; }
    public JSOptionChange<string> InputFormat { get; set; }
    public JSOptionChange<bool> TimeAs24hr { get; set; }
    public JSOptionChange<string> Min { get; set; }
    public JSOptionChange<string> Max { get; set; }
    public JSOptionChange<bool> Disabled { get; set; }
    public JSOptionChange<bool> ReadOnly { get; set; }
    public JSOptionChange<IEnumerable<string>> DisabledDates { get; set; }
    public JSOptionChange<IEnumerable<string>> EnabledDates { get; set; }
    public JSOptionChange<IEnumerable<int>> DisabledDays { get; set; }
    public JSOptionChange<DateInputSelectionMode> SelectionMode { get; set; }
    public JSOptionChange<bool> Inline { get; set; }
    public JSOptionChange<bool> DisableMobile { get; set; }
    public JSOptionChange<string> Placeholder { get; set; }
    public JSOptionChange<bool> StaticPicker { get; set; }
}

