#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <summary>
/// Describes one rendered week in a <see cref="DatePicker{TValue}"/> calendar.
/// </summary>
internal sealed class DatePickerCalendarWeek
{
    /// <summary>
    /// Initializes a new calendar week.
    /// </summary>
    public DatePickerCalendarWeek( int weekNumber, IReadOnlyList<DatePickerCalendarDay> days )
    {
        WeekNumber = weekNumber;
        Days = days;
    }

    /// <summary>
    /// Gets the localized week number.
    /// </summary>
    public int WeekNumber { get; }

    /// <summary>
    /// Gets the seven days contained in the week.
    /// </summary>
    public IReadOnlyList<DatePickerCalendarDay> Days { get; }
}

/// <summary>
/// Describes one rendered day in a <see cref="DatePicker{TValue}"/> calendar.
/// </summary>
internal sealed class DatePickerCalendarDay
{
    /// <summary>
    /// Initializes a new calendar day.
    /// </summary>
    public DatePickerCalendarDay(
        DateTime date,
        bool outside,
        bool today,
        bool selected,
        bool rangeStart,
        bool inRange,
        bool rangeEnd,
        bool disabled,
        bool focused )
    {
        Date = date;
        Outside = outside;
        Today = today;
        Selected = selected;
        RangeStart = rangeStart;
        InRange = inRange;
        RangeEnd = rangeEnd;
        Disabled = disabled;
        Focused = focused;
    }

    /// <summary>
    /// Gets the represented date.
    /// </summary>
    public DateTime Date { get; }

    /// <summary>
    /// Gets whether the day belongs to an adjacent month.
    /// </summary>
    public bool Outside { get; }

    /// <summary>
    /// Gets whether the day is today.
    /// </summary>
    public bool Today { get; }

    /// <summary>
    /// Gets whether the day is selected.
    /// </summary>
    public bool Selected { get; }

    /// <summary>
    /// Gets whether the day starts a selected range.
    /// </summary>
    public bool RangeStart { get; }

    /// <summary>
    /// Gets whether the day is contained in a selected or previewed range.
    /// </summary>
    public bool InRange { get; }

    /// <summary>
    /// Gets whether the day ends a selected range.
    /// </summary>
    public bool RangeEnd { get; }

    /// <summary>
    /// Gets whether the day cannot be selected.
    /// </summary>
    public bool Disabled { get; }

    /// <summary>
    /// Gets whether the day is the keyboard navigation target.
    /// </summary>
    public bool Focused { get; }
}

/// <summary>
/// Describes one rendered month in month-selection mode.
/// </summary>
internal sealed class DatePickerCalendarMonth
{
    /// <summary>
    /// Initializes a new calendar month.
    /// </summary>
    public DatePickerCalendarMonth( DateTime date, string name, bool selected, bool disabled, bool focused )
    {
        Date = date;
        Name = name;
        Selected = selected;
        Disabled = disabled;
        Focused = focused;
    }

    /// <summary>
    /// Gets the first day of the represented month.
    /// </summary>
    public DateTime Date { get; }

    /// <summary>
    /// Gets the localized month name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets whether the month is selected.
    /// </summary>
    public bool Selected { get; }

    /// <summary>
    /// Gets whether the month cannot be selected.
    /// </summary>
    public bool Disabled { get; }

    /// <summary>
    /// Gets whether the month is the keyboard navigation target.
    /// </summary>
    public bool Focused { get; }
}