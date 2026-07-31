#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Blazorise.Utilities;
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
    /// <param name="inputMode">Input mode that determines how calendar values are presented and selected.</param>
    /// <param name="selectionMode">Active date selection mode.</param>
    /// <param name="selectedDates">Dates currently selected by the picker.</param>
    /// <param name="pendingRangeStart">Pending start date of an incomplete range.</param>
    /// <param name="hoveredRangeEnd">Date currently previewed as the end of a range.</param>
    /// <param name="hoveredWeekStart">ISO Monday of the week currently hovered.</param>
    /// <param name="isDateDisabled">Callback that determines whether a date is disabled.</param>
    /// <returns>The six weeks rendered in the calendar grid.</returns>
    public static IReadOnlyList<DatePickerCalendarWeek> BuildWeeks(
        DateTime visibleMonth,
        DateTime focusedDate,
        DayOfWeek firstDayOfWeek,
        DateInputMode inputMode,
        DateInputSelectionMode selectionMode,
        IReadOnlyList<DateTime> selectedDates,
        DateTime? pendingRangeStart,
        DateTime? hoveredRangeEnd,
        DateTime? hoveredWeekStart,
        Func<DateTime, bool> isDateDisabled )
    {
        List<DatePickerCalendarWeek> weeks = new();
        bool weekMode = inputMode == DateInputMode.Week;
        DateTime firstOfMonth = new( visibleMonth.Year, visibleMonth.Month, 1 );
        int leadingDays = ( 7 + (int)firstOfMonth.DayOfWeek - (int)firstDayOfWeek ) % 7;
        DateTime gridStart = firstOfMonth.AddDays( -leadingDays );
        ( DateTime? rangeStart, DateTime? rangeEnd ) = GetDisplayRange(
            selectionMode,
            selectedDates,
            pendingRangeStart,
            hoveredRangeEnd,
            focusedDate );

        if ( weekMode )
        {
            rangeStart = rangeStart.HasValue ? WeekDateFormat.GetWeekStart( rangeStart.Value ) : null;
            rangeEnd = rangeEnd.HasValue ? WeekDateFormat.GetWeekStart( rangeEnd.Value ) : null;
        }

        for ( int weekIndex = 0; weekIndex < 6; weekIndex++ )
        {
            List<DatePickerCalendarDay> days = new();
            DateTime renderedWeekStart = gridStart.AddDays( weekIndex * 7 );
            int mondayOffset = ( 7 + (int)DayOfWeek.Monday - (int)firstDayOfWeek ) % 7;
            DateTime representedWeekStart = renderedWeekStart.AddDays( mondayOffset );
            bool representedWeekSelected = weekMode && IsWeekSelected(
                representedWeekStart,
                selectionMode,
                selectedDates,
                rangeStart,
                rangeEnd );

            for ( int dayIndex = 0; dayIndex < 7; dayIndex++ )
            {
                DateTime date = renderedWeekStart.AddDays( dayIndex );
                bool selected;
                bool rangeStartDay;
                bool inRange;
                bool rangeEndDay;

                if ( weekMode )
                {
                    ( selected, rangeStartDay, inRange, rangeEndDay ) = GetWeekDayState(
                        date,
                        selectionMode,
                        selectedDates,
                        rangeStart,
                        rangeEnd );
                }
                else
                {
                    rangeStartDay = rangeStart.HasValue && date.Date == rangeStart.Value.Date;
                    rangeEndDay = rangeEnd.HasValue && date.Date == rangeEnd.Value.Date;
                    inRange = rangeStart.HasValue
                        && rangeEnd.HasValue
                        && date.Date >= rangeStart.Value.Date
                        && date.Date <= rangeEnd.Value.Date;
                    selected = selectionMode == DateInputSelectionMode.Multiple
                        ? selectedDates.Any( item => item.Date == date.Date )
                        : rangeStartDay || rangeEndDay || selectionMode == DateInputSelectionMode.Single
                            && selectedDates.Any( item => item.Date == date.Date );
                }

                days.Add( new DatePickerCalendarDay(
                    date,
                    date.Month != visibleMonth.Month,
                    date.Date == DateTime.Today,
                    selected,
                    rangeStartDay,
                    inRange,
                    rangeEndDay,
                    weekMode
                    && hoveredWeekStart.HasValue
                    && WeekDateFormat.GetWeekStart( date ) == hoveredWeekStart.Value,
                    isDateDisabled( date ),
                    date.Date == focusedDate.Date ) );
            }

            weeks.Add( new DatePickerCalendarWeek(
                ISOWeek.GetYear( representedWeekStart ),
                ISOWeek.GetWeekOfYear( representedWeekStart ),
                days,
                representedWeekSelected,
                weekMode && days.All( day => day.Selected ),
                weekMode && hoveredWeekStart == representedWeekStart,
                weekMode && IsWeekDisabled( representedWeekStart, isDateDisabled ),
                weekMode && WeekDateFormat.GetWeekStart( focusedDate ) == representedWeekStart ) );
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

    private static bool IsWeekSelected(
        DateTime weekStart,
        DateInputSelectionMode selectionMode,
        IReadOnlyList<DateTime> selectedDates,
        DateTime? rangeStart,
        DateTime? rangeEnd )
    {
        DateTime canonicalWeekStart = WeekDateFormat.GetWeekStart( weekStart );

        if ( selectionMode == DateInputSelectionMode.Range && rangeStart.HasValue && rangeEnd.HasValue )
        {
            DateTime start = WeekDateFormat.GetWeekStart( rangeStart.Value );
            DateTime end = WeekDateFormat.GetWeekStart( rangeEnd.Value );

            if ( end < start )
            {
                (start, end) = (end, start);
            }

            return canonicalWeekStart >= start && canonicalWeekStart <= end;
        }

        return selectedDates.Any( date => WeekDateFormat.GetWeekStart( date ) == canonicalWeekStart );
    }

    private static ( bool Selected, bool RangeStart, bool InRange, bool RangeEnd ) GetWeekDayState(
        DateTime date,
        DateInputSelectionMode selectionMode,
        IReadOnlyList<DateTime> selectedDates,
        DateTime? rangeStart,
        DateTime? rangeEnd )
    {
        DateTime weekStart = WeekDateFormat.GetWeekStart( date );

        if ( selectionMode == DateInputSelectionMode.Range && rangeStart.HasValue && rangeEnd.HasValue )
        {
            DateTime start = WeekDateFormat.GetWeekStart( rangeStart.Value );
            DateTime end = WeekDateFormat.GetWeekStart( rangeEnd.Value );

            if ( end < start )
            {
                (start, end) = (end, start);
            }

            bool selected = weekStart >= start && weekStart <= end;

            return (
                selected,
                selected && date.Date == start,
                selected,
                selected && date.Date == end.AddDays( 6 ) );
        }

        bool selectedWeek = selectedDates.Any( selectedDate => WeekDateFormat.GetWeekStart( selectedDate ) == weekStart );

        return (
            selectedWeek,
            selectedWeek && date.DayOfWeek == DayOfWeek.Monday,
            selectedWeek,
            selectedWeek && date.DayOfWeek == DayOfWeek.Sunday );
    }

    private static bool IsWeekDisabled( DateTime weekStart, Func<DateTime, bool> isDateDisabled )
    {
        for ( int dayOffset = 0; dayOffset < 7; dayOffset++ )
        {
            if ( !isDateDisabled( weekStart.AddDays( dayOffset ) ) )
                return false;
        }

        return true;
    }

    #endregion
}