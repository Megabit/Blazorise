#region Using directives
using System;
using System.Globalization;
#endregion

namespace Blazorise;

/// <summary>
/// Manages the hierarchical navigation used by a month-selection calendar.
/// </summary>
internal sealed class DatePickerCalendarNavigation
{
    #region Methods

    /// <summary>
    /// Restores the calendar to its month view.
    /// </summary>
    public void Reset()
        => View = DatePickerCalendarView.Month;

    /// <summary>
    /// Advances from months to years or from years to decades.
    /// </summary>
    public void ShowBroaderView()
    {
        View = View switch
        {
            DatePickerCalendarView.Month => DatePickerCalendarView.Year,
            DatePickerCalendarView.Year => DatePickerCalendarView.Decade,
            _ => DatePickerCalendarView.Decade,
        };
    }

    /// <summary>
    /// Gets the number of months represented by one panel navigation action.
    /// </summary>
    /// <param name="direction">Navigation direction, expressed as <c>-1</c> or <c>1</c>.</param>
    /// <returns>The signed number of months to navigate.</returns>
    public int GetNavigationMonths( int direction )
        => direction * ( View switch
        {
            DatePickerCalendarView.Year => 120,
            DatePickerCalendarView.Decade => 1200,
            _ => 12,
        } );

    /// <summary>
    /// Gets the number of columns in the active calendar view.
    /// </summary>
    /// <returns>The active grid column count.</returns>
    public int GetColumnCount()
        => View == DatePickerCalendarView.Month ? 4 : 3;

    /// <summary>
    /// Gets the first or last keyboard target in the active calendar view.
    /// </summary>
    /// <param name="visibleYear">Year currently represented by the calendar.</param>
    /// <param name="beginning">Whether the first target is requested.</param>
    /// <returns>The boundary year for the active view.</returns>
    public int GetBoundaryYear( int visibleYear, bool beginning )
    {
        if ( View == DatePickerCalendarView.Year )
        {
            int start = DatePickerCalendarBuilder.GetDecadeStart( visibleYear );
            return beginning ? start : Math.Min( start + 9, DateTime.MaxValue.Year );
        }

        if ( View == DatePickerCalendarView.Decade )
        {
            int start = DatePickerCalendarBuilder.GetCenturyStart( visibleYear );
            return beginning ? start : Math.Min( start + 99, DateTime.MaxValue.Year );
        }

        return visibleYear;
    }

    /// <summary>
    /// Tries to move the keyboard target within a year or decade grid.
    /// </summary>
    /// <param name="visibleMonth">Month currently represented by the calendar.</param>
    /// <param name="focusedDate">Current keyboard target.</param>
    /// <param name="amount">Number of grid cells to move.</param>
    /// <param name="isYearDisabled">Callback that determines whether a year is disabled.</param>
    /// <param name="isPeriodDisabled">Callback that determines whether a year range is disabled.</param>
    /// <param name="targetVisibleMonth">Resulting visible month.</param>
    /// <param name="targetFocusedDate">Resulting keyboard target.</param>
    /// <returns><see langword="true"/> when a valid target was found.</returns>
    public bool TryMoveFocus(
        DateTime visibleMonth,
        DateTime focusedDate,
        int amount,
        Func<int, bool> isYearDisabled,
        Func<int, int, bool> isPeriodDisabled,
        out DateTime targetVisibleMonth,
        out DateTime targetFocusedDate )
    {
        int yearStep = View == DatePickerCalendarView.Year ? 1 : 10;
        int yearChange = amount * yearStep;
        long candidateYear = (long)focusedDate.Year + yearChange;
        int attempts = 0;

        while ( candidateYear is >= 1 and <= 9999 && attempts++ < 10000 )
        {
            int year = (int)candidateYear;
            int decadeStart = DatePickerCalendarBuilder.GetDecadeStart( year );
            bool disabled = View == DatePickerCalendarView.Year
                ? isYearDisabled( year )
                : isPeriodDisabled( decadeStart, Math.Min( decadeStart + 9, DateTime.MaxValue.Year ) );

            if ( !disabled )
            {
                targetVisibleMonth = new DateTime( year, visibleMonth.Month, 1 );
                targetFocusedDate = DatePickerDateUtilities.MoveIntoMonth( focusedDate, targetVisibleMonth );
                return true;
            }

            candidateYear += Math.Sign( yearChange ) * yearStep;
        }

        targetVisibleMonth = visibleMonth;
        targetFocusedDate = focusedDate;
        return false;
    }

    /// <summary>
    /// Tries to select an intermediate period and move to the next narrower view.
    /// </summary>
    /// <param name="period">Calendar period to select.</param>
    /// <param name="visibleMonth">Month currently represented by the calendar.</param>
    /// <param name="focusedDate">Current keyboard target.</param>
    /// <param name="targetVisibleMonth">Resulting visible month.</param>
    /// <param name="targetFocusedDate">Resulting keyboard target.</param>
    /// <returns><see langword="true"/> when the active view accepted the period.</returns>
    public bool TrySelectPeriod(
        DatePickerCalendarPeriod period,
        DateTime visibleMonth,
        DateTime focusedDate,
        out DateTime targetVisibleMonth,
        out DateTime targetFocusedDate )
    {
        if ( period is null || period.Disabled )
        {
            targetVisibleMonth = visibleMonth;
            targetFocusedDate = focusedDate;
            return false;
        }

        if ( View == DatePickerCalendarView.Year )
        {
            targetVisibleMonth = new DateTime( period.StartYear, visibleMonth.Month, 1 );
            targetFocusedDate = DatePickerDateUtilities.MoveIntoMonth( focusedDate, targetVisibleMonth );
            View = DatePickerCalendarView.Month;
            return true;
        }

        if ( View == DatePickerCalendarView.Decade )
        {
            int year = Math.Clamp( period.StartYear, DateTime.MinValue.Year, DateTime.MaxValue.Year );
            targetVisibleMonth = new DateTime( year, visibleMonth.Month, 1 );
            targetFocusedDate = DatePickerDateUtilities.MoveIntoMonth( focusedDate, targetVisibleMonth );
            View = DatePickerCalendarView.Year;
            return true;
        }

        targetVisibleMonth = visibleMonth;
        targetFocusedDate = focusedDate;
        return false;
    }

    /// <summary>
    /// Creates the intermediate period containing the current keyboard target.
    /// </summary>
    /// <param name="focusedDate">Current keyboard target.</param>
    /// <param name="isYearDisabled">Callback that determines whether a year is disabled.</param>
    /// <param name="isPeriodDisabled">Callback that determines whether a year range is disabled.</param>
    /// <returns>The focused year or decade.</returns>
    public DatePickerCalendarPeriod GetFocusedPeriod(
        DateTime focusedDate,
        Func<int, bool> isYearDisabled,
        Func<int, int, bool> isPeriodDisabled )
    {
        if ( View == DatePickerCalendarView.Decade )
        {
            int start = DatePickerCalendarBuilder.GetDecadeStart( focusedDate.Year );
            int end = Math.Min( start + 9, DateTime.MaxValue.Year );
            return new DatePickerCalendarPeriod(
                start,
                end,
                $"{start.ToString( CultureInfo.InvariantCulture )}-{end.ToString( CultureInfo.InvariantCulture )}",
                false,
                false,
                isPeriodDisabled( start, end ),
                true );
        }

        return new DatePickerCalendarPeriod(
            focusedDate.Year,
            focusedDate.Year,
            focusedDate.Year.ToString( CultureInfo.InvariantCulture ),
            false,
            false,
            isYearDisabled( focusedDate.Year ),
            true );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the active month-selection calendar view.
    /// </summary>
    public DatePickerCalendarView View { get; private set; }

    #endregion
}