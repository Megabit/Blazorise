#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#endregion

namespace Blazorise;

/// <summary>
/// Builds the calendar presentation models used by a <see cref="DatePicker{TValue}"/>.
/// </summary>
internal static class DatePickerCalendarBuilder
{
    #region Methods

    /// <summary>
    /// Builds the weeks displayed for the visible calendar month.
    /// </summary>
    /// <param name="visibleMonth">Month currently displayed by the calendar.</param>
    /// <param name="focusedDate">Date targeted by keyboard navigation.</param>
    /// <param name="firstDayOfWeek">First day rendered in each week.</param>
    /// <param name="selectionMode">Active date selection mode.</param>
    /// <param name="selectedDates">Dates currently selected by the picker.</param>
    /// <param name="pendingRangeStart">Pending start date of an incomplete range.</param>
    /// <param name="hoveredRangeEnd">Date currently previewed as the end of a range.</param>
    /// <param name="isDateDisabled">Callback that determines whether a date is disabled.</param>
    /// <returns>The six weeks rendered in the calendar grid.</returns>
    public static IReadOnlyList<DatePickerCalendarWeek> BuildWeeks(
        DateTime visibleMonth,
        DateTime focusedDate,
        DayOfWeek firstDayOfWeek,
        DateInputSelectionMode selectionMode,
        IReadOnlyList<DateTime> selectedDates,
        DateTime? pendingRangeStart,
        DateTime? hoveredRangeEnd,
        Func<DateTime, bool> isDateDisabled )
    {
        List<DatePickerCalendarWeek> weeks = new();
        DateTime firstOfMonth = new( visibleMonth.Year, visibleMonth.Month, 1 );
        int leadingDays = ( 7 + (int)firstOfMonth.DayOfWeek - (int)firstDayOfWeek ) % 7;
        DateTime gridStart = firstOfMonth.AddDays( -leadingDays );
        ( DateTime? rangeStart, DateTime? rangeEnd ) = GetDisplayRange(
            selectionMode,
            selectedDates,
            pendingRangeStart,
            hoveredRangeEnd,
            focusedDate );

        for ( int weekIndex = 0; weekIndex < 6; weekIndex++ )
        {
            List<DatePickerCalendarDay> days = new();
            DateTime weekStart = gridStart.AddDays( weekIndex * 7 );

            for ( int dayIndex = 0; dayIndex < 7; dayIndex++ )
            {
                DateTime date = weekStart.AddDays( dayIndex );
                bool rangeStartDay = rangeStart.HasValue && date.Date == rangeStart.Value.Date;
                bool rangeEndDay = rangeEnd.HasValue && date.Date == rangeEnd.Value.Date;
                bool inRange = rangeStart.HasValue
                    && rangeEnd.HasValue
                    && date.Date >= rangeStart.Value.Date
                    && date.Date <= rangeEnd.Value.Date;
                bool selected = selectionMode == DateInputSelectionMode.Multiple
                    ? selectedDates.Any( item => item.Date == date.Date )
                    : rangeStartDay || rangeEndDay || selectionMode == DateInputSelectionMode.Single
                        && selectedDates.Any( item => item.Date == date.Date );

                days.Add( new DatePickerCalendarDay(
                    date,
                    date.Month != visibleMonth.Month,
                    date.Date == DateTime.Today,
                    selected,
                    rangeStartDay,
                    inRange,
                    rangeEndDay,
                    isDateDisabled( date ),
                    date.Date == focusedDate.Date ) );
            }

            weeks.Add( new DatePickerCalendarWeek( ISOWeek.GetWeekOfYear( weekStart ), days ) );
        }

        return weeks;
    }

    /// <summary>
    /// Builds the months displayed by the month-selection view.
    /// </summary>
    /// <param name="visibleMonth">Month whose year is currently displayed.</param>
    /// <param name="focusedDate">Month targeted by keyboard navigation.</param>
    /// <param name="selectedDates">Dates currently selected by the picker.</param>
    /// <param name="monthNames">Localized month names.</param>
    /// <param name="isMonthDisabled">Callback that determines whether a month is disabled.</param>
    /// <returns>The twelve months rendered in the month grid.</returns>
    public static IReadOnlyList<DatePickerCalendarMonth> BuildMonths(
        DateTime visibleMonth,
        DateTime focusedDate,
        IReadOnlyList<DateTime> selectedDates,
        IReadOnlyList<string> monthNames,
        Func<DateTime, bool> isMonthDisabled )
    {
        List<DatePickerCalendarMonth> months = new();

        for ( int monthIndex = 1; monthIndex <= 12; monthIndex++ )
        {
            DateTime month = new( visibleMonth.Year, monthIndex, 1 );

            months.Add( new DatePickerCalendarMonth(
                month,
                monthNames[monthIndex - 1],
                selectedDates.Any( item => item.Year == month.Year && item.Month == month.Month ),
                isMonthDisabled( month ),
                focusedDate.Year == month.Year && focusedDate.Month == month.Month ) );
        }

        return months;
    }

    private static ( DateTime? Start, DateTime? End ) GetDisplayRange(
        DateInputSelectionMode selectionMode,
        IReadOnlyList<DateTime> selectedDates,
        DateTime? pendingRangeStart,
        DateTime? hoveredRangeEnd,
        DateTime focusedDate )
    {
        if ( selectionMode == DateInputSelectionMode.Range )
        {
            if ( pendingRangeStart.HasValue )
            {
                DateTime end = hoveredRangeEnd ?? focusedDate;
                return end < pendingRangeStart.Value
                    ? ( end, pendingRangeStart.Value )
                    : ( pendingRangeStart.Value, end );
            }

            if ( selectedDates.Count > 0 )
            {
                return ( selectedDates[0], selectedDates.Count > 1 ? selectedDates[1] : selectedDates[0] );
            }
        }

        return ( null, null );
    }

    #endregion
}