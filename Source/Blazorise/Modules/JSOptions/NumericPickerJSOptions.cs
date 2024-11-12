namespace Blazorise.Modules;

/// <summary>
/// Represents JavaScript options for initializing a numeric picker component.
/// </summary>
public class NumericPickerInitializeJSOptions
{
    /// <summary>
    /// Gets or sets the initial value of the numeric picker.
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether changes should be applied immediately.
    /// </summary>
    public bool Immediate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether input changes should be debounced.
    /// </summary>
    public bool Debounce { get; set; }

    /// <summary>
    /// Gets or sets the debounce interval in milliseconds.
    /// </summary>
    public int DebounceInterval { get; set; }

    /// <summary>
    /// Gets or sets the number of decimal places allowed in the input.
    /// </summary>
    public int Decimals { get; set; }

    /// <summary>
    /// Gets or sets the character used as the decimal separator.
    /// </summary>
    public string DecimalSeparator { get; set; }

    /// <summary>
    /// Gets or sets an alternative character that can be used as the decimal separator.
    /// </summary>
    public string AlternativeDecimalSeparator { get; set; }

    /// <summary>
    /// Gets or sets the character used as the group separator (e.g., for thousands).
    /// </summary>
    public string GroupSeparator { get; set; }

    /// <summary>
    /// Gets or sets the spacing pattern for grouped numbers.
    /// </summary>
    public string GroupSpacing { get; set; }

    /// <summary>
    /// Gets or sets the currency symbol to display alongside the value.
    /// </summary>
    public string CurrencySymbol { get; set; }

    /// <summary>
    /// Gets or sets the placement of the currency symbol (e.g., before or after the value).
    /// </summary>
    public string CurrencySymbolPlacement { get; set; }

    /// <summary>
    /// Gets or sets the rounding method to apply to the value.
    /// </summary>
    public string RoundingMethod { get; set; }

    /// <summary>
    /// Gets or sets the behavior for padding decimal places.
    /// </summary>
    public object AllowDecimalPadding { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to always allow a decimal separator in the input.
    /// </summary>
    public bool AlwaysAllowDecimalSeparator { get; set; }

    /// <summary>
    /// Gets or sets the minimum allowed value.
    /// </summary>
    public object Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed value.
    /// </summary>
    public object Max { get; set; }

    /// <summary>
    /// Gets or sets an override for the minimum and maximum limits.
    /// </summary>
    public object MinMaxLimitsOverride { get; set; }

    /// <summary>
    /// Gets or sets the minimum allowed value by type constraint.
    /// </summary>
    public object TypeMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum allowed value by type constraint.
    /// </summary>
    public object TypeMax { get; set; }

    /// <summary>
    /// Gets or sets the step value for incrementing or decrementing the numeric picker.
    /// </summary>
    public decimal? Step { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether all text should be selected when the input gains focus.
    /// </summary>
    public bool SelectAllOnFocus { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the value can be modified using the mouse wheel.
    /// </summary>
    public bool ModifyValueOnWheel { get; set; }

    /// <summary>
    /// Gets or sets the element or elements on which the wheel modification applies.
    /// </summary>
    public object WheelOn { get; set; }
}

/// <summary>
/// Represents JavaScript options for updating specific settings of a numeric picker component dynamically.
/// </summary>
public class NumericPickerUpdateJSOptions
{
    /// <summary>
    /// Gets or sets the option for updating the number of decimal places.
    /// </summary>
    public JSOptionChange<int> Decimals { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the decimal separator.
    /// </summary>
    public JSOptionChange<string> DecimalSeparator { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the alternative decimal separator.
    /// </summary>
    public JSOptionChange<string> AlternativeDecimalSeparator { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the group separator.
    /// </summary>
    public JSOptionChange<string> GroupSeparator { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the group spacing.
    /// </summary>
    public JSOptionChange<string> GroupSpacing { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the currency symbol.
    /// </summary>
    public JSOptionChange<string> CurrencySymbol { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the placement of the currency symbol.
    /// </summary>
    public JSOptionChange<string> CurrencySymbolPlacement { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the rounding method.
    /// </summary>
    public JSOptionChange<string> RoundingMethod { get; set; }

    /// <summary>
    /// Gets or sets the option for updating decimal padding behavior.
    /// </summary>
    public JSOptionChange<object> AllowDecimalPadding { get; set; }

    /// <summary>
    /// Gets or sets the option for allowing a decimal separator.
    /// </summary>
    public JSOptionChange<bool> AlwaysAllowDecimalSeparator { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the minimum allowed value.
    /// </summary>
    public JSOptionChange<object> Min { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the maximum allowed value.
    /// </summary>
    public JSOptionChange<object> Max { get; set; }

    /// <summary>
    /// Gets or sets the option for overriding minimum and maximum limits.
    /// </summary>
    public JSOptionChange<object> MinMaxLimitsOverride { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the behavior of selecting all text on focus.
    /// </summary>
    public JSOptionChange<bool> SelectAllOnFocus { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the mouse wheel modification behavior.
    /// </summary>
    public JSOptionChange<bool> ModifyValueOnWheel { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the element(s) on which the wheel modification applies.
    /// </summary>
    public JSOptionChange<object> WheelOn { get; set; }
}