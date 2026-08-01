#region Using directives
using System;
using System.Globalization;
using System.Linq;
using Blazorise.Utilities;
#endregion

namespace Blazorise;

/// <summary>
/// Exposes calendar presentation state shared by the default and provider-specific date picker renderers.
/// </summary>
internal sealed class DatePickerCalendarContext<TValue>
{
    #region Members

    private readonly DatePicker<TValue> parent;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new calendar presentation context.
    /// </summary>
    /// <param name="parent">Date picker that owns the calendar.</param>
    public DatePickerCalendarContext( DatePicker<TValue> parent )
    {
        this.parent = parent;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the provider classes for a rendered calendar day.
    /// </summary>
    /// <param name="day">Calendar day being rendered.</param>
    /// <returns>The provider-specific class names.</returns>
    public string GetDayClassNames( DatePickerCalendarDay day )
        => parent.PickerClassProvider.DatePickerCalendarDay(
            day.Outside,
            day.Today,
            day.Selected,
            day.RangeStart,
            day.InRange,
            day.RangeEnd,
            day.Disabled,
            day.Focused && parent.FocusCalendarOnOpen );

    /// <summary>
    /// Gets the provider classes for a rendered calendar month.
    /// </summary>
    /// <param name="month">Calendar month being rendered.</param>
    /// <returns>The provider-specific class names.</returns>
    public string GetMonthClassNames( DatePickerCalendarMonth month )
        => parent.PickerClassProvider.DatePickerCalendarMonth(
            month.Selected,
            month.Disabled,
            month.Focused && parent.FocusCalendarOnOpen );

    /// <summary>
    /// Gets the provider classes for a rendered year or decade.
    /// </summary>
    /// <param name="period">Calendar period being rendered.</param>
    /// <returns>The provider-specific class names.</returns>
    public string GetPeriodClassNames( DatePickerCalendarPeriod period )
        => parent.PickerClassProvider.DatePickerCalendarMonth(
            period.Selected,
            period.Disabled,
            period.Focused && parent.FocusCalendarOnOpen );

    /// <summary>
    /// Gets the DOM identifier for a rendered calendar day.
    /// </summary>
    /// <param name="date">Rendered date.</param>
    /// <returns>The day element identifier.</returns>
    public string GetDayId( DateTime date )
        => $"{parent.ElementId}-day-{date:yyyyMMdd}";

    /// <summary>
    /// Gets the DOM identifier for a rendered calendar month.
    /// </summary>
    /// <param name="date">Rendered month.</param>
    /// <returns>The month element identifier.</returns>
    public string GetMonthId( DateTime date )
        => $"{parent.ElementId}-month-{date:yyyyMM}";

    /// <summary>
    /// Gets the DOM identifier for a rendered year or decade.
    /// </summary>
    /// <param name="period">Rendered calendar period.</param>
    /// <returns>The period element identifier.</returns>
    public string GetPeriodId( DatePickerCalendarPeriod period )
        => GetPeriodId( parent.CalendarView, period.StartYear );

    private string GetPeriodId( DatePickerCalendarView view, int startYear )
        => $"{parent.ElementId}-{view.ToString().ToLowerInvariant()}-{startYear.ToString( CultureInfo.InvariantCulture )}";

    /// <summary>
    /// Gets the localized accessible label for a date.
    /// </summary>
    /// <param name="date">Date being described.</param>
    /// <returns>The localized long-date label.</returns>
    public string GetDateAriaLabel( DateTime date )
        => date.ToString( "D", CultureInfo.CurrentCulture );

    /// <summary>
    /// Gets the localized accessible label for a rendered ISO week.
    /// </summary>
    /// <param name="week">Week being described.</param>
    /// <returns>The localized week and date-range label.</returns>
    public string GetWeekAriaLabel( DatePickerCalendarWeek week )
        => $"{WeekText} {week.WeekNumber}, {week.StartDate.ToString( "d", CultureInfo.CurrentCulture )} \u2013 {week.EndDate.ToString( "d", CultureInfo.CurrentCulture )}";

    /// <summary>
    /// Gets the localized accessible label for the ISO week containing a date.
    /// </summary>
    /// <param name="date">Date contained by the described ISO week.</param>
    /// <returns>The localized week and date-range label.</returns>
    public string GetWeekAriaLabel( DateTime date )
    {
        DateTime weekStart = WeekDateFormat.GetWeekStart( date );

        return $"{WeekText} {WeekDateFormat.GetWeekNumber( weekStart )}, {weekStart.ToString( "d", CultureInfo.CurrentCulture )} \u2013 {weekStart.AddDays( 6 ).ToString( "d", CultureInfo.CurrentCulture )}";
    }

    private static string FormatRange( int startYear, int length )
    {
        int endYear = Math.Min( startYear + length, DateTime.MaxValue.Year );
        return $"{startYear.ToString( CultureInfo.InvariantCulture )}-{endYear.ToString( CultureInfo.InvariantCulture )}";
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the tab index applied to interactive calendar controls.
    /// </summary>
    public int ControlTabIndex => parent.FocusCalendarOnOpen ? 0 : -1;

    /// <summary>
    /// Gets the accessible label for the visible calendar period.
    /// </summary>
    public string Label => parent.InputMode == DateInputMode.Month
        ? Title
        : $"{MonthNames[parent.CalendarVisibleMonth.Month - 1]} {parent.CalendarVisibleMonth.Year}";

    /// <summary>
    /// Gets the title for the active calendar panel.
    /// </summary>
    public string Title
    {
        get
        {
            int year = parent.CalendarVisibleMonth.Year;

            return parent.CalendarView switch
            {
                DatePickerCalendarView.Year => FormatRange( DatePickerCalendarBuilder.GetDecadeStart( year ), 9 ),
                DatePickerCalendarView.Decade => FormatRange( DatePickerCalendarBuilder.GetCenturyStart( year ), 99 ),
                _ => year.ToString( CultureInfo.InvariantCulture ),
            };
        }
    }

    /// <summary>
    /// Gets the value applied to the calendar panel data attribute.
    /// </summary>
    public string ViewName => parent.CalendarView.ToString().ToLowerInvariant();

    /// <summary>
    /// Gets the identifier of the item targeted by keyboard navigation.
    /// </summary>
    public string ActiveDescendantId
    {
        get
        {
            if ( parent.InputMode != DateInputMode.Month )
                return GetDayId( FocusedDate );

            if ( parent.CalendarView == DatePickerCalendarView.Month )
                return GetMonthId( FocusedDate );

            int startYear = parent.CalendarView == DatePickerCalendarView.Year
                ? FocusedDate.Year
                : DatePickerCalendarBuilder.GetDecadeStart( FocusedDate.Year );

            return GetPeriodId( parent.CalendarView, startYear );
        }
    }

    /// <summary>
    /// Gets the date targeted by keyboard navigation.
    /// </summary>
    public DateTime FocusedDate => parent.CalendarFocusedDate;

    /// <summary>
    /// Gets the number of the visible month.
    /// </summary>
    public int VisibleMonthNumber => parent.CalendarVisibleMonth.Month;

    /// <summary>
    /// Gets the year of the visible month.
    /// </summary>
    public int VisibleMonthYear => parent.CalendarVisibleMonth.Year;

    /// <summary>
    /// Gets the provider classes for the picker container.
    /// </summary>
    public string ContainerClassNames => parent.PickerClassProvider.DatePickerContainer( parent.Inline, parent.CalendarVisible );

    /// <summary>
    /// Gets the provider classes for the calendar.
    /// </summary>
    public string ClassNames => parent.PickerClassProvider.DatePickerCalendar( parent.Inline, parent.StaticPicker );

    /// <summary>
    /// Gets the provider classes for the calendar backdrop.
    /// </summary>
    public string BackdropClassNames => parent.PickerClassProvider.DatePickerCalendarBackdrop();

    /// <summary>
    /// Gets the provider classes for the calendar header.
    /// </summary>
    public string HeaderClassNames => parent.PickerClassProvider.DatePickerCalendarHeader();

    /// <summary>
    /// Gets the provider classes for calendar navigation.
    /// </summary>
    public string NavigationClassNames => parent.PickerClassProvider.DatePickerCalendarNavigation();

    /// <summary>
    /// Gets the provider classes for the calendar title.
    /// </summary>
    public string TitleClassNames => parent.PickerClassProvider.DatePickerCalendarTitle();

    /// <summary>
    /// Gets the provider classes for the calendar grid.
    /// </summary>
    public string GridClassNames => parent.PickerClassProvider.DatePickerCalendarGrid();

    /// <summary>
    /// Gets the provider classes for the weekday row.
    /// </summary>
    public string WeekdaysClassNames => parent.PickerClassProvider.DatePickerCalendarWeekdays();

    /// <summary>
    /// Gets the provider classes for a weekday label.
    /// </summary>
    public string WeekdayClassNames => parent.PickerClassProvider.DatePickerCalendarWeekday();

    /// <summary>
    /// Gets the provider classes for a calendar week.
    /// </summary>
    public string WeekClassNames => parent.PickerClassProvider.DatePickerCalendarWeek();

    /// <summary>
    /// Gets the provider classes for a week number.
    /// </summary>
    public string WeekNumberClassNames => parent.PickerClassProvider.DatePickerCalendarWeekNumber();

    /// <summary>
    /// Gets the provider classes for the month grid.
    /// </summary>
    public string MonthsClassNames => parent.PickerClassProvider.DatePickerCalendarMonths();

    /// <summary>
    /// Gets the provider classes for the calendar time controls.
    /// </summary>
    public string TimeClassNames => parent.PickerClassProvider.DatePickerCalendarTime();

    /// <summary>
    /// Gets the provider classes for a calendar time input.
    /// </summary>
    public string TimeInputClassNames => parent.PickerClassProvider.DatePickerCalendarTimeInput();

    /// <summary>
    /// Gets the provider classes for the calendar actions.
    /// </summary>
    public string ActionsClassNames => parent.PickerClassProvider.DatePickerCalendarActions();

    /// <summary>
    /// Gets the provider classes for a calendar action button.
    /// </summary>
    public string ButtonClassNames => parent.PickerClassProvider.DatePickerCalendarButton();

    /// <summary>
    /// Gets localized weekday names ordered according to the configured first day of the week.
    /// </summary>
    public string[] WeekdayNames
    {
        get
        {
            string[] names =
            {
                parent.PickerLocalizer["Sun"],
                parent.PickerLocalizer["Mon"],
                parent.PickerLocalizer["Tue"],
                parent.PickerLocalizer["Wed"],
                parent.PickerLocalizer["Thu"],
                parent.PickerLocalizer["Fri"],
                parent.PickerLocalizer["Sat"],
            };

            return Enumerable.Range( 0, 7 )
                .Select( index => names[( index + (int)parent.CalendarFirstDayOfWeek ) % 7] )
                .ToArray();
        }
    }

    /// <summary>
    /// Gets localized month names.
    /// </summary>
    public string[] MonthNames =>
    [
        parent.PickerLocalizer["January"],
        parent.PickerLocalizer["February"],
        parent.PickerLocalizer["March"],
        parent.PickerLocalizer["April"],
        parent.PickerLocalizer["May!"],
        parent.PickerLocalizer["June"],
        parent.PickerLocalizer["July"],
        parent.PickerLocalizer["August"],
        parent.PickerLocalizer["September"],
        parent.PickerLocalizer["October"],
        parent.PickerLocalizer["November"],
        parent.PickerLocalizer["December"],
    ];

    /// <summary>
    /// Gets the localized current-period action text for the active input mode.
    /// </summary>
    public string TodayText => parent.InputMode switch
    {
        DateInputMode.Month => parent.PickerLocalizer["ThisMonth"],
        DateInputMode.Week => parent.PickerLocalizer["ThisWeek"],
        _ => parent.PickerLocalizer["Today"],
    };

    /// <summary>
    /// Gets the localized Clear action text.
    /// </summary>
    public string ClearText => parent.PickerLocalizer["Clear"];

    /// <summary>
    /// Gets the localized month field label.
    /// </summary>
    public string MonthText => parent.PickerLocalizer["Month"];

    /// <summary>
    /// Gets the localized year field label.
    /// </summary>
    public string YearText => parent.PickerLocalizer["Year"];

    /// <summary>
    /// Gets the localized abbreviated week label.
    /// </summary>
    public string WeekText => parent.PickerLocalizer["Week"];

    /// <summary>
    /// Gets the localized time section label.
    /// </summary>
    public string TimeText => parent.PickerLocalizer["Time"];

    /// <summary>
    /// Gets the localized hour field label.
    /// </summary>
    public string HourText => parent.PickerLocalizer["Hour"];

    /// <summary>
    /// Gets the localized minute field label.
    /// </summary>
    public string MinuteText => parent.PickerLocalizer["Minute"];

    /// <summary>
    /// Gets the localized accessible label for navigating to the previous period.
    /// </summary>
    public string PreviousPeriodAriaLabel => parent.InputMode == DateInputMode.Month
        ? parent.CalendarView switch
        {
            DatePickerCalendarView.Year => parent.PickerLocalizer["PreviousDecade"],
            DatePickerCalendarView.Decade => parent.PickerLocalizer["PreviousCentury"],
            _ => parent.PickerLocalizer["PreviousYear"],
        }
        : parent.PickerLocalizer["PreviousMonth"];

    /// <summary>
    /// Gets the localized accessible label for navigating to the next period.
    /// </summary>
    public string NextPeriodAriaLabel => parent.InputMode == DateInputMode.Month
        ? parent.CalendarView switch
        {
            DatePickerCalendarView.Year => parent.PickerLocalizer["NextDecade"],
            DatePickerCalendarView.Decade => parent.PickerLocalizer["NextCentury"],
            _ => parent.PickerLocalizer["NextYear"],
        }
        : parent.PickerLocalizer["NextMonth"];

    /// <summary>
    /// Gets the localized accessible label for navigating to the previous year.
    /// </summary>
    public string PreviousYearAriaLabel => parent.PickerLocalizer["PreviousYear"];

    /// <summary>
    /// Gets the localized accessible label for navigating to the next year.
    /// </summary>
    public string NextYearAriaLabel => parent.PickerLocalizer["NextYear"];

    /// <summary>
    /// Gets the direction-aware icon for navigating to the previous period.
    /// </summary>
    public IconName PreviousPeriodIconName => parent.InputMode == DateInputMode.Month
        ? PreviousYearIconName
        : CultureInfo.CurrentCulture.TextInfo.IsRightToLeft
            ? IconName.ChevronRight
            : IconName.ChevronLeft;

    /// <summary>
    /// Gets the direction-aware icon for navigating to the next period.
    /// </summary>
    public IconName NextPeriodIconName => parent.InputMode == DateInputMode.Month
        ? NextYearIconName
        : CultureInfo.CurrentCulture.TextInfo.IsRightToLeft
            ? IconName.ChevronLeft
            : IconName.ChevronRight;

    /// <summary>
    /// Gets the direction-aware icon for navigating to the previous year.
    /// </summary>
    public IconName PreviousYearIconName => CultureInfo.CurrentCulture.TextInfo.IsRightToLeft
        ? IconName.ChevronDoubleRight
        : IconName.ChevronDoubleLeft;

    /// <summary>
    /// Gets the direction-aware icon for navigating to the next year.
    /// </summary>
    public IconName NextYearIconName => CultureInfo.CurrentCulture.TextInfo.IsRightToLeft
        ? IconName.ChevronDoubleLeft
        : IconName.ChevronDoubleRight;

    /// <summary>
    /// Gets the direction-aware fallback text for navigating to the previous period.
    /// </summary>
    public string PreviousText => CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? "\u203A" : "\u2039";

    /// <summary>
    /// Gets the direction-aware fallback text for navigating to the next period.
    /// </summary>
    public string NextText => CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? "\u2039" : "\u203A";

    /// <summary>
    /// Gets the selected hour in 24-hour form.
    /// </summary>
    public int CurrentHour => parent.CalendarTimeSource.Hour;

    /// <summary>
    /// Gets the selected minute.
    /// </summary>
    public int CurrentMinute => parent.CalendarTimeSource.Minute;

    /// <summary>
    /// Gets the hour represented according to the configured clock format.
    /// </summary>
    public int DisplayHour => parent.TimeAs24hr ? CurrentHour : CurrentHour % 12 == 0 ? 12 : CurrentHour % 12;

    /// <summary>
    /// Gets the zero-padded display hour.
    /// </summary>
    public string DisplayHourText => DisplayHour.ToString( "D2", CultureInfo.InvariantCulture );

    /// <summary>
    /// Gets the zero-padded selected minute.
    /// </summary>
    public string CurrentMinuteText => CurrentMinute.ToString( "D2", CultureInfo.InvariantCulture );

    /// <summary>
    /// Gets whether the selected time is post meridiem.
    /// </summary>
    public bool IsPostMeridiem => CurrentHour >= 12;

    /// <summary>
    /// Gets the localized ante meridiem text.
    /// </summary>
    public string AnteMeridiemText => parent.PickerLocalizer["AM"];

    /// <summary>
    /// Gets the localized post meridiem text.
    /// </summary>
    public string PostMeridiemText => parent.PickerLocalizer["PM"];

    /// <summary>
    /// Gets the localized meridiem text for the selected time.
    /// </summary>
    public string MeridiemText => IsPostMeridiem ? PostMeridiemText : AnteMeridiemText;

    #endregion
}