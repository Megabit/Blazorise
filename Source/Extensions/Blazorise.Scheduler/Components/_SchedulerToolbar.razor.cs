#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Localization;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler.Components;

/// <summary>
/// A toolbar for the <see cref="Scheduler{TItem}"/> component providing navigation controls
/// and view-switching options (day, week, work week, month).
/// </summary>
/// <typeparam name="TItem">The type of the scheduler item.</typeparam>
public partial class _SchedulerToolbar<TItem> : BaseComponent, IDisposable
{
    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            LocalizerService.LocalizationChanged -= OnLocalizationChanged;
        }
    }

    /// <inheritdoc/>
    override protected void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-scheduler-toolbar" );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Re-renders the toolbar when the localization changes.
    /// </summary>
    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Invokes the callback to navigate to the previous date range.
    /// </summary>
    protected async Task OnPreviousClick()
    {
        if ( SchedulerState?.PrevDayRequested is not null )
            await SchedulerState.PrevDayRequested.InvokeCallbackAsync();
    }

    /// <summary>
    /// Invokes the callback to navigate to the next date range.
    /// </summary>
    protected async Task OnNextClick()
    {
        if ( SchedulerState?.NextDayRequested is not null )
            await SchedulerState.NextDayRequested.InvokeCallbackAsync();
    }

    /// <summary>
    /// Invokes the callback to jump to today's date.
    /// </summary>
    protected async Task OnTodayClick()
    {
        if ( SchedulerState?.TodayRequested is not null )
            await SchedulerState.TodayRequested.InvokeCallbackAsync();
    }

    /// <summary>
    /// Invokes the callback to switch to the day view.
    /// </summary>
    protected async Task OnDayViewClick()
    {
        if ( SchedulerState?.DayViewRequested is not null )
            await SchedulerState.DayViewRequested.InvokeCallbackAsync();
    }

    /// <summary>
    /// Invokes the callback to switch to the week view.
    /// </summary>
    protected async Task OnWeekViewClick()
    {
        if ( SchedulerState?.WeekViewRequested is not null )
            await SchedulerState.WeekViewRequested.InvokeCallbackAsync();
    }

    /// <summary>
    /// Invokes the callback to switch to the work week view.
    /// </summary>
    protected async Task OnWorkWeekViewClick()
    {
        if ( SchedulerState?.WorkWeekViewRequested is not null )
            await SchedulerState.WorkWeekViewRequested.InvokeCallbackAsync();
    }

    /// <summary>
    /// Invokes the callback to switch to the month view.
    /// </summary>
    protected async Task OnMonthViewClick()
    {
        if ( SchedulerState?.MonthViewRequested is not null )
            await SchedulerState.MonthViewRequested.InvokeCallbackAsync();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if the Day view is currently selected.
    /// </summary>
    protected bool DayViewSelected => SelectedView == SchedulerView.Day;

    /// <summary>
    /// Indicates if the Week view is currently selected.
    /// </summary>
    protected bool WeekViewSelected => SelectedView == SchedulerView.Week;

    /// <summary>
    /// Indicates if the Work Week view is currently selected.
    /// </summary>
    protected bool WorkWeekViewSelected => SelectedView == SchedulerView.WorkWeek;

    /// <summary>
    /// Indicates if the Month view is currently selected.
    /// </summary>
    protected bool MonthViewSelected => SelectedView == SchedulerView.Month;

    /// <summary>
    /// Gets or sets the current state of the scheduler (events, callbacks).
    /// </summary>
    [CascadingParameter] public SchedulerState SchedulerState { get; set; }

    /// <summary>
    /// Provides localized strings for toolbar labels.
    /// </summary>
    [Inject] protected ITextLocalizer<Scheduler<TItem>> Localizer { get; set; }

    /// <summary>
    /// Tracks global localization changes.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Gets or sets the parent <see cref="Scheduler{TItem}"/> instance.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the currently selected date in the scheduler.
    /// </summary>
    [Parameter] public DateOnly SelectedDate { get; set; }

    /// <summary>
    /// Gets or sets the currently active scheduler view.
    /// </summary>
    [Parameter] public SchedulerView SelectedView { get; set; }

    /// <summary>
    /// Gets or sets the first day of the week (e.g. Monday or Sunday).
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }

    /// <summary>
    /// Gets or sets the first day of the work week (e.g. Monday).
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWorkWeek { get; set; }

    /// <summary>
    /// Gets or sets whether the Day View button should be displayed.
    /// </summary>
    [Parameter] public bool ShowDayViewButton { get; set; }

    /// <summary>
    /// Gets or sets whether the Week View button should be displayed.
    /// </summary>
    [Parameter] public bool ShowWeekViewButton { get; set; }

    /// <summary>
    /// Gets or sets whether the Work Week View button should be displayed.
    /// </summary>
    [Parameter] public bool ShowWorkWeekViewButton { get; set; }

    /// <summary>
    /// Gets or sets whether the Month View button should be displayed.
    /// </summary>
    [Parameter] public bool ShowMonthViewButton { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the toolbar.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
