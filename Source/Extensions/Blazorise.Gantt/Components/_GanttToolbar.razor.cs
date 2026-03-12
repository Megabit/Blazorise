#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Gantt.Extensions;
using Blazorise.Localization;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt.Components;

/// <summary>
/// Internal toolbar renderer for <see cref="Gantt{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public partial class _GanttToolbar<TItem> : BaseComponent, IDisposable
{
    #region Members

    private bool dropdownColumnChooserVisible;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            LocalizerService.LocalizationChanged -= OnLocalizationChanged;
        }
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-gantt-toolbar" );

        base.BuildClasses( builder );
    }

    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    private Task OnPreviousClick() => PreviousClicked.InvokeAsync();

    private Task OnNextClick() => NextClicked.InvokeAsync();

    private Task OnTodayClick() => TodayClicked.InvokeAsync();

    private Task OnDayViewClick() => DayViewClicked.InvokeAsync();

    private Task OnWeekViewClick() => WeekViewClicked.InvokeAsync();

    private Task OnMonthViewClick() => MonthViewClicked.InvokeAsync();

    private Task OnYearViewClick() => YearViewClicked.InvokeAsync();

    private Task OnAddTaskClick() => AddTaskClicked.InvokeAsync();

    private Task OnExpandAllClick() => ExpandAllClicked.InvokeAsync();

    private Task OnCollapseAllClick() => CollapseAllClicked.InvokeAsync();

    private Task OnSearchTextValueChanged( string value ) => SearchTextChanged.InvokeAsync( value );

    private Task OnColumnVisibilityChanged( string key, bool value )
        => ColumnVisibilityChanged.InvokeAsync( new GanttColumnVisibilityChangedEventArgs( key, value ) );

    #endregion

    #region Properties

    private string RangeText
    {
        get
        {
            if ( TimelineRangeStart.HasValue && TimelineRangeEnd.HasValue )
            {
                return FormatTimelineRangeText( TimelineRangeStart.Value, TimelineRangeEnd.Value );
            }

            if ( SelectedView == GanttView.Week )
            {
                var start = SelectedDate.StartOfWeek( FirstDayOfWeek );
                var end = start.AddDays( 6 );

                return $"{start.ToString( "MMM dd", CultureInfo.InvariantCulture )} - {end.ToString( "MMM dd, yyyy", CultureInfo.InvariantCulture )}";
            }

            if ( SelectedView == GanttView.Month )
            {
                return SelectedDate.ToString( "MMMM yyyy", CultureInfo.InvariantCulture );
            }

            if ( SelectedView == GanttView.Year )
            {
                return SelectedDate.ToString( "yyyy", CultureInfo.InvariantCulture );
            }

            return SelectedDate.ToString( "MMMM dd, yyyy", CultureInfo.InvariantCulture );
        }
    }

    private bool ShowViewButtons => ShowDayViewButton || ShowWeekViewButton || ShowMonthViewButton || ShowYearViewButton;

    private string FormatTimelineRangeText( DateTime rangeStart, DateTime rangeEnd )
    {
        if ( SelectedView == GanttView.Day )
        {
            var displayEnd = rangeEnd > rangeStart
                ? rangeEnd.AddHours( -1 )
                : rangeEnd;

            return $"{rangeStart.ToString( "MMM dd, yyyy HH:mm", CultureInfo.InvariantCulture )} - {displayEnd.ToString( "MMM dd, yyyy HH:mm", CultureInfo.InvariantCulture )}";
        }

        if ( SelectedView == GanttView.Year )
        {
            var displayEnd = rangeEnd > rangeStart
                ? rangeEnd.AddMonths( -1 )
                : rangeEnd;

            return rangeStart.Year == displayEnd.Year
                ? rangeStart.ToString( "yyyy", CultureInfo.InvariantCulture )
                : $"{rangeStart.ToString( "yyyy", CultureInfo.InvariantCulture )} - {displayEnd.ToString( "yyyy", CultureInfo.InvariantCulture )}";
        }

        var end = rangeEnd > rangeStart
            ? rangeEnd.AddDays( -1 )
            : rangeEnd;

        return $"{rangeStart.ToString( "MMM dd", CultureInfo.InvariantCulture )} - {end.ToString( "MMM dd, yyyy", CultureInfo.InvariantCulture )}";
    }

    /// <summary>
    /// Gets the text localizer used for toolbar labels.
    /// </summary>
    [Inject] protected ITextLocalizer<Gantt<TItem>> Localizer { get; set; }

    /// <summary>
    /// Gets the localization service used to react to culture changes.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Gets the parent Gantt component.
    /// </summary>
    [CascadingParameter] public Gantt<TItem> Gantt { get; set; }

    /// <summary>
    /// Gets or sets the currently displayed date.
    /// </summary>
    [Parameter] public DateOnly SelectedDate { get; set; }

    /// <summary>
    /// Gets or sets the currently selected Gantt view.
    /// </summary>
    [Parameter] public GanttView SelectedView { get; set; }

    /// <summary>
    /// Gets or sets the visible timeline range start used for toolbar range text.
    /// </summary>
    [Parameter] public DateTime? TimelineRangeStart { get; set; }

    /// <summary>
    /// Gets or sets the visible timeline range end used for toolbar range text.
    /// </summary>
    [Parameter] public DateTime? TimelineRangeEnd { get; set; }

    /// <summary>
    /// Gets or sets the first day of the week used for range calculations.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }

    /// <summary>
    /// Gets or sets whether toolbar navigation buttons are enabled.
    /// </summary>
    [Parameter] public bool NavigationEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the current search text.
    /// </summary>
    [Parameter] public string SearchText { get; set; }

    /// <summary>
    /// Gets or sets the width of search input in pixels.
    /// </summary>
    [Parameter] public double SearchInputWidth { get; set; } = 220d;

    /// <summary>
    /// Gets or sets the column picker entries shown in the toolbar.
    /// </summary>
    [Parameter] public IReadOnlyList<GanttColumnPickerItem> ColumnPickerItems { get; set; } = Array.Empty<GanttColumnPickerItem>();

    /// <summary>
    /// Gets or sets whether Add Task button is visible.
    /// </summary>
    [Parameter] public bool ShowAddTaskButton { get; set; } = true;

    /// <summary>
    /// Gets or sets whether expand-all and collapse-all commands are visible.
    /// </summary>
    [Parameter] public bool ShowToggleAllCommands { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the day view button is visible.
    /// </summary>
    [Parameter] public bool ShowDayViewButton { get; set; }

    /// <summary>
    /// Gets or sets whether the week view button is visible.
    /// </summary>
    [Parameter] public bool ShowWeekViewButton { get; set; }

    /// <summary>
    /// Gets or sets whether the month view button is visible.
    /// </summary>
    [Parameter] public bool ShowMonthViewButton { get; set; }

    /// <summary>
    /// Gets or sets whether the year view button is visible.
    /// </summary>
    [Parameter] public bool ShowYearViewButton { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the previous-period button is clicked.
    /// </summary>
    [Parameter] public EventCallback PreviousClicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the next-period button is clicked.
    /// </summary>
    [Parameter] public EventCallback NextClicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the today button is clicked.
    /// </summary>
    [Parameter] public EventCallback TodayClicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the day view button is clicked.
    /// </summary>
    [Parameter] public EventCallback DayViewClicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the week view button is clicked.
    /// </summary>
    [Parameter] public EventCallback WeekViewClicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the month view button is clicked.
    /// </summary>
    [Parameter] public EventCallback MonthViewClicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the year view button is clicked.
    /// </summary>
    [Parameter] public EventCallback YearViewClicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the add-task button is clicked.
    /// </summary>
    [Parameter] public EventCallback AddTaskClicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the expand-all button is clicked.
    /// </summary>
    [Parameter] public EventCallback ExpandAllClicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the collapse-all button is clicked.
    /// </summary>
    [Parameter] public EventCallback CollapseAllClicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when the search text changes.
    /// </summary>
    [Parameter] public EventCallback<string> SearchTextChanged { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when a column visibility option changes.
    /// </summary>
    [Parameter] public EventCallback<GanttColumnVisibilityChangedEventArgs> ColumnVisibilityChanged { get; set; }

    #endregion
}