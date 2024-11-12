using System.Collections.Generic;

namespace Blazorise.Modules;

/// <summary>
/// Represents JavaScript options for initializing a date picker component.
/// </summary>
public class DatePickerInitializeJSOptions
{
    /// <summary>
    /// Gets or sets the input mode for the date picker (e.g., text input or dropdown).
    /// </summary>
    public DateInputMode InputMode { get; set; }

    /// <summary>
    /// Gets or sets the selection mode for the date picker (e.g., single, range).
    /// </summary>
    public string SelectionMode { get; set; }

    /// <summary>
    /// Gets or sets the first day of the week (0 = Sunday, 1 = Monday, etc.).
    /// </summary>
    public int FirstDayOfWeek { get; set; }

    /// <summary>
    /// Gets or sets the format in which dates are displayed in the picker.
    /// </summary>
    public string DisplayFormat { get; set; }

    /// <summary>
    /// Gets or sets the format for date input by the user.
    /// </summary>
    public string InputFormat { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the time should be displayed in 24-hour format.
    /// </summary>
    public bool TimeAs24hr { get; set; }

    /// <summary>
    /// Gets or sets the default date to be selected initially.
    /// </summary>
    public object DefaultDate { get; set; }

    /// <summary>
    /// Gets or sets the minimum date that can be selected.
    /// </summary>
    public string Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum date that can be selected.
    /// </summary>
    public string Max { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the date picker is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the date picker is read-only.
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets a collection of dates that are disabled and cannot be selected.
    /// </summary>
    public IEnumerable<string> DisabledDates { get; set; }

    /// <summary>
    /// Gets or sets a collection of dates that are explicitly enabled.
    /// </summary>
    public IEnumerable<string> EnabledDates { get; set; }

    /// <summary>
    /// Gets or sets a collection of days of the week that are disabled (0 = Sunday, 1 = Monday, etc.).
    /// </summary>
    public IEnumerable<int> DisabledDays { get; set; }

    /// <summary>
    /// Gets or sets localization settings for the date picker.
    /// </summary>
    public object Localization { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the date picker should be displayed inline.
    /// </summary>
    public bool Inline { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to disable the mobile version of the date picker.
    /// </summary>
    public bool DisableMobile { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text for the date input field.
    /// </summary>
    public string Placeholder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display the date picker as a static element.
    /// </summary>
    public bool StaticPicker { get; set; }

    /// <summary>
    /// Gets or sets the validation status for the date picker.
    /// </summary>
    public object ValidationStatus { get; set; }
}

/// <summary>
/// Represents JavaScript options for updating specific settings of a date picker dynamically.
/// </summary>
public class DatePickerUpdateJSOptions
{
    /// <summary>
    /// Gets or sets the option for updating the first day of the week.
    /// </summary>
    public JSOptionChange<int> FirstDayOfWeek { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the display format of the date.
    /// </summary>
    public JSOptionChange<string> DisplayFormat { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the input format for date entry.
    /// </summary>
    public JSOptionChange<string> InputFormat { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the time format to 24-hour.
    /// </summary>
    public JSOptionChange<bool> TimeAs24hr { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the minimum selectable date.
    /// </summary>
    public JSOptionChange<string> Min { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the maximum selectable date.
    /// </summary>
    public JSOptionChange<string> Max { get; set; }

    /// <summary>
    /// Gets or sets the option for enabling or disabling the date picker.
    /// </summary>
    public JSOptionChange<bool> Disabled { get; set; }

    /// <summary>
    /// Gets or sets the option for setting the date picker as read-only.
    /// </summary>
    public JSOptionChange<bool> ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the option for updating disabled dates.
    /// </summary>
    public JSOptionChange<IEnumerable<string>> DisabledDates { get; set; }

    /// <summary>
    /// Gets or sets the option for updating enabled dates.
    /// </summary>
    public JSOptionChange<IEnumerable<string>> EnabledDates { get; set; }

    /// <summary>
    /// Gets or sets the option for updating disabled days of the week.
    /// </summary>
    public JSOptionChange<IEnumerable<int>> DisabledDays { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the date selection mode.
    /// </summary>
    public JSOptionChange<DateInputSelectionMode> SelectionMode { get; set; }

    /// <summary>
    /// Gets or sets the option for displaying the date picker inline.
    /// </summary>
    public JSOptionChange<bool> Inline { get; set; }

    /// <summary>
    /// Gets or sets the option for disabling the mobile version of the date picker.
    /// </summary>
    public JSOptionChange<bool> DisableMobile { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the placeholder text.
    /// </summary>
    public JSOptionChange<string> Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the option for displaying the date picker as a static element.
    /// </summary>
    public JSOptionChange<bool> StaticPicker { get; set; }
}