namespace Blazorise;

/// <summary>
/// Identifies the source that initiated a date selection.
/// </summary>
internal enum DatePickerSelectionSource
{
    /// <summary>
    /// The selection originated from the calendar.
    /// </summary>
    Calendar,

    /// <summary>
    /// The selection originated from the Today button.
    /// </summary>
    TodayButton,
}