using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Localization;
using Blazorise.Scheduler.Utilities;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Scheduler.Components;

/// <summary>
/// A component for editing recurrence rules in iCalendar (RFC 5545) format,
/// used by the <see cref="Scheduler{TItem}"/> component.
/// </summary>
/// <typeparam name="TItem">The type of the scheduler item.</typeparam>
public partial class _SchedulerRecurrenceEditor<TItem> : BaseComponent, IDisposable
{
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

    /// <summary>
    /// Re-renders the component when localization changes.
    /// </summary>
    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Parses and applies recurrence rule parameters when they change.
    /// </summary>
    /// <param name="parameters">Component parameters.</param>
    /// <returns>A <see cref="Task"/> that completes when parameters are set.</returns>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<string>( nameof( RecurrenceRule ), out var paramRecurrenceRule ) && !RecurrenceRule.IsEqual( paramRecurrenceRule ) )
        {
            if ( !string.IsNullOrEmpty( paramRecurrenceRule ) )
            {
                var rule = RecurringRuleParser.Parse( paramRecurrenceRule );

                Pattern = rule.Pattern;
                Interval = rule.Interval;
                EndDate = rule.EndDate;
                Count = rule.Count;
                ByDay = rule.ByDay;
                ByMonthDay = rule.ByMonthDay ?? new List<int> { 1 };
                ByWeek = rule.ByWeek ?? SchedulerWeek.First;
                ByWeekDay = rule.ByWeekDay ?? FirstDayOfWeek;
                ByMonth = rule.ByMonth ?? SchedulerMonth.January;

                if ( Pattern == SchedulerRecurrencePattern.Monthly || Pattern == SchedulerRecurrencePattern.Yearly )
                {
                    InternalMonthlyRule = rule.ByMonthDay?.Any() == true
                        ? "monthly-rule-by-day"
                        : "monthly-rule-by-week";
                }
            }
            else
            {
                Pattern = SchedulerRecurrencePattern.Never;
                Interval = 1;
                EndDate = null;
                Count = null;
                ByDay = new() { DateTime.Today.AddDays( 1 ).DayOfWeek };
                ByMonthDay = new() { 1 };
                ByWeek = SchedulerWeek.First;
                ByWeekDay = FirstDayOfWeek;
                ByMonth = SchedulerMonth.January;
            }
        }

        return base.SetParametersAsync( parameters );
    }

    /// <summary>
    /// Called when the recurrence interval changes.
    /// </summary>
    /// <param name="interval">The new interval value.</param>
    protected Task OnIntervalChanged( int interval )
    {
        Interval = interval;
        return NotifyRecurrenceRuleChanged();
    }

    /// <summary>
    /// Called when the recurrence pattern changes (e.g., daily, weekly).
    /// </summary>
    /// <param name="pattern">The selected recurrence pattern.</param>
    protected Task OnPatternChanged( SchedulerRecurrencePattern pattern )
    {
        Pattern = pattern;
        return NotifyRecurrenceRuleChanged();
    }

    /// <summary>
    /// Called when the end date of the recurrence rule is modified.
    /// </summary>
    /// <param name="endDate">The new end date, or null to unset it.</param>
    protected Task OnEndDateChanged( DateTime? endDate )
    {
        EndDate = endDate;
        return NotifyRecurrenceRuleChanged();
    }

    /// <summary>
    /// Called when the recurrence count (number of occurrences) is modified.
    /// </summary>
    /// <param name="count">The new count value.</param>
    protected Task OnCountChanged( int? count )
    {
        Count = count;
        return NotifyRecurrenceRuleChanged();
    }

    /// <summary>
    /// Called when the list of days for a weekly rule changes.
    /// </summary>
    /// <param name="byDay">The selected days of the week.</param>
    protected Task OnWeeklyRuleOnDayChanged( List<DayOfWeek> byDay )
    {
        ByDay = byDay;
        return NotifyRecurrenceRuleChanged();
    }

    /// <summary>
    /// Called when the list of days in the month changes for a monthly rule.
    /// </summary>
    /// <param name="byMonthDay">The list of selected days of the month.</param>
    protected Task OnMonthlyRuleByMonthDayChanged( List<int> byMonthDay )
    {
        ByMonthDay = byMonthDay;
        return NotifyRecurrenceRuleChanged();
    }

    /// <summary>
    /// Called when the week index (e.g., first, last) changes for a monthly rule.
    /// </summary>
    /// <param name="byWeek">The selected week index.</param>
    protected Task OnMonthlyRuleByWeekChanged( SchedulerWeek? byWeek )
    {
        ByWeek = byWeek;
        return NotifyRecurrenceRuleChanged();
    }

    /// <summary>
    /// Called when the weekday changes for a monthly rule using week-based pattern.
    /// </summary>
    /// <param name="byWeekDay">The selected day of the week.</param>
    protected Task OnMonthlyRuleByWeekDayChanged( DayOfWeek? byWeekDay )
    {
        ByWeekDay = byWeekDay;
        return NotifyRecurrenceRuleChanged();
    }

    /// <summary>
    /// Called when the month changes for a yearly rule.
    /// </summary>
    /// <param name="byMonth">The selected month.</param>
    protected Task OnYearlyRuleByMonthChanged( SchedulerMonth? byMonth )
    {
        ByMonth = byMonth;
        return NotifyRecurrenceRuleChanged();
    }

    /// <summary>
    /// Called when the internal monthly rule mode changes (day-based or week-based).
    /// </summary>
    /// <param name="value">The selected internal rule identifier.</param>
    protected Task OnInternalMonthlyRuleChanged( string value )
    {
        InternalMonthlyRule = value;
        return NotifyRecurrenceRuleChanged();
    }

    /// <summary>
    /// Constructs a new recurrence rule based on the current UI state,
    /// converts it to an RFC 5545 string, and notifies the parent component.
    /// </summary>
    /// <returns>A task representing the asynchronous update.</returns>
    protected Task NotifyRecurrenceRuleChanged()
    {
        var rule = new SchedulerRecurrenceRule
        {
            Pattern = Pattern,
            Interval = Interval,
            EndDate = EndDate,
            Count = Count,
            ByDay = ByDay,
        };

        if ( rule.Pattern == SchedulerRecurrencePattern.Monthly || rule.Pattern == SchedulerRecurrencePattern.Yearly )
        {
            if ( InternalMonthlyRule == "monthly-rule-by-day" )
            {
                rule.ByMonthDay = ByMonthDay ?? new List<int> { 1 };
            }
            else
            {
                rule.ByWeek = ByWeek ?? SchedulerWeek.First;
                rule.ByWeekDay = ByWeekDay ?? FirstDayOfWeek;
            }
        }

        if ( rule.Pattern == SchedulerRecurrencePattern.Yearly )
        {
            rule.ByMonth = ByMonth ?? SchedulerMonth.January;
        }

        RecurrenceRule = rule.ToRRuleString();
        return RecurrenceRuleChanged.InvokeAsync( RecurrenceRule );
    }

    /// <summary>
    /// Gets or sets the internal mode for monthly rule:
    /// "monthly-rule-by-day" or "monthly-rule-by-week".
    /// </summary>
    protected string InternalMonthlyRule = "monthly-rule-by-day";

    /// <summary>Gets or sets the selected recurrence pattern.</summary>
    protected SchedulerRecurrencePattern Pattern { get; set; }

    /// <summary>Gets or sets the recurrence interval (e.g., every 2 weeks).</summary>
    protected int Interval { get; set; } = 1;

    /// <summary>Gets or sets the optional end date for the recurrence.</summary>
    protected DateTime? EndDate { get; set; }

    /// <summary>Gets or sets the optional number of occurrences.</summary>
    protected int? Count { get; set; }

    /// <summary>Gets or sets the days of the week used in weekly rules.</summary>
    protected List<DayOfWeek> ByDay { get; set; }

    /// <summary>Gets or sets the days of the month used in monthly rules.</summary>
    protected List<int> ByMonthDay { get; set; }

    /// <summary>Gets or sets the week number used in monthly rules.</summary>
    protected SchedulerWeek? ByWeek { get; set; }

    /// <summary>Gets or sets the weekday used in monthly rules.</summary>
    protected DayOfWeek? ByWeekDay { get; set; }

    /// <summary>Gets or sets the month used in yearly rules.</summary>
    protected SchedulerMonth? ByMonth { get; set; }

    /// <summary>
    /// Injects an instance of <see cref="ITextLocalizer{T}"/> for localization of scheduler UI.
    /// </summary>
    [Inject] protected ITextLocalizer<Scheduler<TItem>> Localizer { get; set; }

    /// <summary>
    /// Injects a service to monitor and respond to localization changes.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Gets or sets the parent scheduler component.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the first day of the week, used in recurrence rules.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }

    /// <summary>
    /// Gets or sets the recurrence rule string (in RFC 5545 format).
    /// </summary>
    [Parameter] public string RecurrenceRule { get; set; }

    /// <summary>
    /// Callback triggered when the recurrence rule string changes.
    /// </summary>
    [Parameter] public EventCallback<string> RecurrenceRuleChanged { get; set; }
}
