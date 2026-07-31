#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace Blazorise;

/// <summary>
/// Describes one rendered calendar row and the ISO week whose Monday occurs within it.
/// </summary>
internal sealed class DatePickerCalendarWeek
{
    #region Constructors

    /// <summary>
    /// Initializes a new calendar week.
    /// </summary>
    /// <param name="weekYear">ISO week-numbering year.</param>
    /// <param name="weekNumber">ISO number of the represented week.</param>
    /// <param name="days">Seven days contained in the rendered row.</param>
    /// <param name="selected">Whether the represented ISO week is selected.</param>
    /// <param name="fullySelected">Whether every rendered day belongs to a selected week.</param>
    /// <param name="hovered">Whether the represented week is hovered.</param>
    /// <param name="disabled">Whether every day in the represented ISO week is disabled.</param>
    /// <param name="focused">Whether the represented ISO week is targeted by keyboard navigation.</param>
    public DatePickerCalendarWeek(
        int weekYear,
        int weekNumber,
        IReadOnlyList<DatePickerCalendarDay> days,
        bool selected,
        bool fullySelected,
        bool hovered,
        bool disabled,
        bool focused )
    {
        WeekYear = weekYear;
        WeekNumber = weekNumber;
        Days = days;
        Selected = selected;
        FullySelected = fullySelected;
        Hovered = hovered;
        Disabled = disabled;
        Focused = focused;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the ISO week-numbering year.
    /// </summary>
    public int WeekYear { get; }

    /// <summary>
    /// Gets the ISO week number.
    /// </summary>
    public int WeekNumber { get; }

    /// <summary>
    /// Gets the seven days contained in the rendered calendar row.
    /// </summary>
    public IReadOnlyList<DatePickerCalendarDay> Days { get; }

    /// <summary>
    /// Gets whether the represented ISO week is selected.
    /// </summary>
    public bool Selected { get; }

    /// <summary>
    /// Gets whether every rendered day belongs to a selected ISO week.
    /// </summary>
    public bool FullySelected { get; }

    /// <summary>
    /// Gets whether the represented ISO week is hovered.
    /// </summary>
    public bool Hovered { get; }

    /// <summary>
    /// Gets whether every day in the represented ISO week is disabled.
    /// </summary>
    public bool Disabled { get; }

    /// <summary>
    /// Gets whether the represented ISO week is targeted by keyboard navigation.
    /// </summary>
    public bool Focused { get; }

    /// <summary>
    /// Gets the Monday starting the represented ISO week.
    /// </summary>
    public DateTime StartDate => ISOWeek.ToDateTime( WeekYear, WeekNumber, DayOfWeek.Monday );

    /// <summary>
    /// Gets the Sunday ending the represented ISO week.
    /// </summary>
    public DateTime EndDate => StartDate.AddDays( 6 );

    #endregion
}

/// <summary>
/// Describes one rendered day in a <see cref="DatePicker{TValue}"/> calendar.
/// </summary>
internal sealed class DatePickerCalendarDay
{
    #region Constructors

    /// <summary>
    /// Initializes a new calendar day.
    /// </summary>
    /// <param name="date">Represented date.</param>
    /// <param name="outside">Whether the date belongs to an adjacent month.</param>
    /// <param name="today">Whether the date is today.</param>
    /// <param name="selected">Whether the date is selected.</param>
    /// <param name="rangeStart">Whether the date starts a selected range.</param>
    /// <param name="inRange">Whether the date is contained in a selected or previewed range.</param>
    /// <param name="rangeEnd">Whether the date ends a selected range.</param>
    /// <param name="weekHovered">Whether the ISO week containing the date is hovered.</param>
    /// <param name="disabled">Whether the date cannot be selected.</param>
    /// <param name="focused">Whether the date is targeted by keyboard navigation.</param>
    public DatePickerCalendarDay(
        DateTime date,
        bool outside,
        bool today,
        bool selected,
        bool rangeStart,
        bool inRange,
        bool rangeEnd,
        bool weekHovered,
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
        WeekHovered = weekHovered;
        Disabled = disabled;
        Focused = focused;
    }

    #endregion

    #region Properties

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
    /// Gets whether the ISO week containing the day is hovered.
    /// </summary>
    public bool WeekHovered { get; }

    /// <summary>
    /// Gets whether the day cannot be selected.
    /// </summary>
    public bool Disabled { get; }

    /// <summary>
    /// Gets whether the day is the keyboard navigation target.
    /// </summary>
    public bool Focused { get; }

    #endregion
}

/// <summary>
/// Describes one rendered month in month-selection mode.
/// </summary>
internal sealed class DatePickerCalendarMonth
{
    #region Constructors

    /// <summary>
    /// Initializes a new calendar month.
    /// </summary>
    /// <param name="date">First day of the represented month.</param>
    /// <param name="name">Localized month name.</param>
    /// <param name="selected">Whether the month is selected.</param>
    /// <param name="disabled">Whether the month cannot be selected.</param>
    /// <param name="focused">Whether the month is targeted by keyboard navigation.</param>
    public DatePickerCalendarMonth( DateTime date, string name, bool selected, bool disabled, bool focused )
    {
        Date = date;
        Name = name;
        Selected = selected;
        Disabled = disabled;
        Focused = focused;
    }

    #endregion

    #region Properties

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

    #endregion
}