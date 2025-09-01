namespace Blazorise.Modules;

/// <summary>
/// Represents JavaScript options for initializing a time picker component.
/// </summary>
public class TimePickerJSOptions
{
    /// <summary>
    /// Gets or sets the format in which the time is displayed in the picker.
    /// </summary>
    public string DisplayFormat { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the time is displayed in 24-hour format.
    /// </summary>
    public bool TimeAs24hr { get; set; }

    /// <summary>
    /// Gets or sets the default time value initially selected in the picker.
    /// </summary>
    public string Default { get; set; }

    /// Gets or sets the initial value of the hour element.
    /// </summary>
    public int DefaultHour { get; set; }

    /// <summary>
    /// Gets or sets the initial value of the minute element.
    /// </summary>
    public int DefaultMinute { get; set; }

    /// <summary>
    /// Gets or sets the minimum time that can be selected.
    /// </summary>
    public string Min { get; set; }

    /// <summary>
    /// Gets or sets the maximum time that can be selected.
    /// </summary>
    public string Max { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the time picker is disabled.
    /// </summary>
    public bool Disabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the time picker is read-only.
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets localization settings for the time picker.
    /// </summary>
    public object Localization { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the time picker should be displayed inline.
    /// </summary>
    public bool Inline { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to disable the mobile version of the time picker.
    /// </summary>
    public bool DisableMobile { get; set; }

    /// <summary>
    /// Gets or sets the placeholder text displayed in the time input field.
    /// </summary>
    public string Placeholder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to display the time picker as a static element.
    /// </summary>
    public bool StaticPicker { get; set; }

    /// <summary>
    /// Adjusts the step for the hour input.
    /// </summary>
    public int HourIncrement { get; set; }

    /// <summary>
    /// Adjusts the step for the minute input.
    /// </summary>
    public int MinuteIncrement { get; set; }
}

/// <summary>
/// Represents JavaScript options for updating specific settings of a time picker component dynamically.
/// </summary>
public class TimePickerUpdateJSOptions
{
    /// <summary>
    /// Gets or sets the option for updating the display format of the time.
    /// </summary>
    public JSOptionChange<string> DisplayFormat { get; set; }

    /// <summary>
    /// Gets or sets the option for switching between 24-hour and 12-hour format.
    /// </summary>
    public JSOptionChange<bool> TimeAs24hr { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the minimum selectable time.
    /// </summary>
    public JSOptionChange<string> Min { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the maximum selectable time.
    /// </summary>
    public JSOptionChange<string> Max { get; set; }

    /// <summary>
    /// Gets or sets the option for enabling or disabling the time picker.
    /// </summary>
    public JSOptionChange<bool> Disabled { get; set; }

    /// <summary>
    /// Gets or sets the option for setting the time picker as read-only.
    /// </summary>
    public JSOptionChange<bool> ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the option for displaying the time picker inline.
    /// </summary>
    public JSOptionChange<bool> Inline { get; set; }

    /// <summary>
    /// Gets or sets the option for disabling the mobile version of the time picker.
    /// </summary>
    public JSOptionChange<bool> DisableMobile { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the placeholder text.
    /// </summary>
    public JSOptionChange<string> Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the option for displaying the time picker as a static element.
    /// </summary>
    public JSOptionChange<bool> StaticPicker { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the hour increment step.
    /// </summary>
    public JSOptionChange<int> HourIncrement { get; set; }

    /// <summary>
    /// Gets or sets the option for updating the minute increment step.
    /// </summary>
    public JSOptionChange<int> MinuteIncrement { get; set; }

    /// <summary>
    /// Gets or sets the initial value of the hour element.
    /// </summary>
    public JSOptionChange<int> DefaultHour { get; set; }

    /// <summary>
    /// Gets or sets the initial value of the minute element.
    /// </summary>
    public JSOptionChange<int> DefaultMinute { get; set; }
}