#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Localization;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler.Components;

public partial class _SchedulerToolbar<TItem> : BaseComponent, IDisposable
{
    #region Members

    #endregion

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

    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    /// <inheritdoc/>
    override protected void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-scheduler-toolbar" );

        base.BuildClasses( builder );
    }

    protected async Task OnPreviousClick()
    {
        if ( SchedulerState?.PrevDayRequested is not null )
            await SchedulerState.PrevDayRequested.InvokeCallbackAsync();
    }

    protected async Task OnNextClick()
    {
        if ( SchedulerState?.NextDayRequested is not null )
            await SchedulerState.NextDayRequested.InvokeCallbackAsync();
    }

    protected async Task OnTodayClick()
    {
        if ( SchedulerState?.TodayRequested is not null )
            await SchedulerState.TodayRequested.InvokeCallbackAsync();
    }

    protected async Task OnDayViewClick()
    {
        if ( SchedulerState?.DayViewRequested is not null )
            await SchedulerState.DayViewRequested.InvokeCallbackAsync();
    }

    protected async Task OnWeekViewClick()
    {
        if ( SchedulerState?.WeekViewRequested is not null )
            await SchedulerState.WeekViewRequested.InvokeCallbackAsync();
    }

    protected async Task OnWorkWeekViewClick()
    {
        if ( SchedulerState?.WorkWeekViewRequested is not null )
            await SchedulerState.WorkWeekViewRequested.InvokeCallbackAsync();
    }

    protected async Task OnMonthViewClick()
    {
        if ( SchedulerState?.MonthViewRequested is not null )
            await SchedulerState.MonthViewRequested.InvokeCallbackAsync();
    }

    #endregion

    #region Properties

    protected bool DayViewSelected => SelectedView == SchedulerView.Day;

    protected bool WeekViewSelected => SelectedView == SchedulerView.Week;

    protected bool WorkWeekViewSelected => SelectedView == SchedulerView.WorkWeek;

    protected bool MonthViewSelected => SelectedView == SchedulerView.Month;

    /// <summary>
    /// Gets or sets the scheduler state.
    /// </summary>
    [CascadingParameter] public SchedulerState SchedulerState { get; set; }

    /// <summary>
    /// Injects an instance of <see cref="ITextLocalizerService"/> for localization.
    /// </summary>
    [Inject] protected ITextLocalizer<Scheduler<TItem>> Localizer { get; set; }

    /// <summary>
    /// Injects an instance of <see cref="ITextLocalizerService"/> for localization.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Cascades the <see cref="Scheduler{TItem}"/> instance to the modal.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the date that is currently selected in the scheduler.
    /// </summary>
    [Parameter] public DateOnly SelectedDate { get; set; }

    /// <summary>
    /// Gets or sets the view that is currently selected in the scheduler.
    /// </summary>
    [Parameter] public SchedulerView SelectedView { get; set; }

    /// <summary>
    /// Gets or sets the first day of the week.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }

    [Parameter] public DayOfWeek FirstDayOfWorkWeek { get; set; }

    [Parameter] public bool ShowDayViewButton { get; set; }

    [Parameter] public bool ShowWeekViewButton { get; set; }

    [Parameter] public bool ShowWorkWeekViewButton { get; set; }

    [Parameter] public bool ShowMonthViewButton { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the component.
    /// </summary>
    /// <remarks>
    /// This property allows developers to define custom content within the <see cref="SchedulerToolbar{TItem}"/> component.
    /// </remarks>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
