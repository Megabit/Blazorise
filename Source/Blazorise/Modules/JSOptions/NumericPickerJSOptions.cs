namespace Blazorise;

internal class NumericPickerInitializeJSOptions
{
    public object Value { get; set; }
    public bool Immediate { get; set; }
    public bool Debounce { get; set; }
    public int DebounceInterval { get; set; }
    public int Decimals { get; set; }
    public string DecimalSeparator { get; set; }
    public string AlternativeDecimalSeparator { get; set; }
    public string GroupSeparator { get; set; }
    public string GroupSpacing { get; set; }
    public string CurrencySymbol { get; set; }
    public string CurrencySymbolPlacement { get; set; }
    public string RoundingMethod { get; set; }
    public object AllowDecimalPadding { get; set; }
    public bool AlwaysAllowDecimalSeparator { get; set; }
    public object Min { get; set; }
    public object Max { get; set; }
    public object MinMaxLimitsOverride { get; set; }
    public object TypeMin { get; set; }
    public object TypeMax { get; set; }
    public decimal? Step { get; set; }
    public bool SelectAllOnFocus { get; set; }
    public bool ModifyValueOnWheel { get; set; }
    public object WheelOn { get; set; }
}

internal class NumericPickerUpdateJSOptions
{
    public JSOptionChange<int> Decimals { get; set; }
    public JSOptionChange<string> DecimalSeparator { get; set; }
    public JSOptionChange<string> AlternativeDecimalSeparator { get; set; }
    public JSOptionChange<string> GroupSeparator { get; set; }
    public JSOptionChange<string> GroupSpacing { get; set; }
    public JSOptionChange<string> CurrencySymbol { get; set; }
    public JSOptionChange<string> CurrencySymbolPlacement { get; set; }
    public JSOptionChange<string> RoundingMethod { get; set; }
    public JSOptionChange<object> AllowDecimalPadding { get; set; }
    public JSOptionChange<bool> AlwaysAllowDecimalSeparator { get; set; }
    public JSOptionChange<object> Min { get; set; }
    public JSOptionChange<object> Max { get; set; }
    public JSOptionChange<object> MinMaxLimitsOverride { get; set; }
    public JSOptionChange<bool> SelectAllOnFocus { get; set; }
    public JSOptionChange<bool> ModifyValueOnWheel { get; set; }
    public JSOptionChange<object> WheelOn { get; set; }
}
