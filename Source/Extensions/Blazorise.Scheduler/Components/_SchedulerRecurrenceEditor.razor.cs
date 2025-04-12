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

public partial class _SchedulerRecurrenceEditor<TItem> : BaseComponent, IDisposable
{
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

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

    protected Task OnIntervalChanged( int interval )
    {
        Interval = interval;
        return NotifyRecurrenceRuleChanged();
    }

    protected Task OnPatternChanged( SchedulerRecurrencePattern pattern )
    {
        Pattern = pattern;
        return NotifyRecurrenceRuleChanged();
    }

    protected Task OnEndDateChanged( DateTime? endDate )
    {
        EndDate = endDate;
        return NotifyRecurrenceRuleChanged();
    }

    protected Task OnCountChanged( int? count )
    {
        Count = count;
        return NotifyRecurrenceRuleChanged();
    }

    protected Task OnWeeklyRuleOnDayChanged( List<DayOfWeek> byDay )
    {
        ByDay = byDay;
        return NotifyRecurrenceRuleChanged();
    }

    protected Task OnMonthlyRuleByMonthDayChanged( List<int> byMonthDay )
    {
        ByMonthDay = byMonthDay;
        return NotifyRecurrenceRuleChanged();
    }

    protected Task OnMonthlyRuleByWeekChanged( SchedulerWeek? byWeek )
    {
        ByWeek = byWeek;
        return NotifyRecurrenceRuleChanged();
    }

    protected Task OnMonthlyRuleByWeekDayChanged( DayOfWeek? byWeekDay )
    {
        ByWeekDay = byWeekDay;
        return NotifyRecurrenceRuleChanged();
    }

    protected Task OnYearlyRuleByMonthChanged( SchedulerMonth? byMonth )
    {
        ByMonth = byMonth;
        return NotifyRecurrenceRuleChanged();
    }

    protected Task OnInternalMonthlyRuleChanged( string value )
    {
        InternalMonthlyRule = value;
        return NotifyRecurrenceRuleChanged();
    }

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

    protected string InternalMonthlyRule = "monthly-rule-by-day";

    protected SchedulerRecurrencePattern Pattern { get; set; }

    protected int Interval { get; set; } = 1;

    protected DateTime? EndDate { get; set; }

    protected int? Count { get; set; }

    protected List<DayOfWeek> ByDay { get; set; }

    protected List<int> ByMonthDay { get; set; }

    protected SchedulerWeek? ByWeek { get; set; }

    protected DayOfWeek? ByWeekDay { get; set; }

    protected SchedulerMonth? ByMonth { get; set; }

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

    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }

    [Parameter] public string RecurrenceRule { get; set; }

    [Parameter] public EventCallback<string> RecurrenceRuleChanged { get; set; }
}
