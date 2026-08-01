namespace Blazorise;

/// <summary>
/// Identifies the active panel displayed by a month-selection calendar.
/// </summary>
internal enum DatePickerCalendarView
{
    /// <summary>
    /// Displays the months in the active year.
    /// </summary>
    Month,

    /// <summary>
    /// Displays the years in the active decade.
    /// </summary>
    Year,

    /// <summary>
    /// Displays the decades in the active century.
    /// </summary>
    Decade,
}