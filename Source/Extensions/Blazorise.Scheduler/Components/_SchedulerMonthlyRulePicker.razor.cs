using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Scheduler.Components;

public partial class _SchedulerMonthlyRulePicker<TItem> : RadioGroup<string>, IDisposable
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
        if ( parameters.TryGetValue<List<int>>( nameof( ByMonthDay ), out var paramByMonthDay ) && !ByMonthDay.AreEqual( paramByMonthDay ) )
        {
            SingleByMonthDay = paramByMonthDay?.FirstOrDefault() ?? 1;
        }

        return base.SetParametersAsync( parameters );
    }

    protected override Task OnInternalValueChanged( string value )
    {
        InternalMonthlyRule = value;

        return base.OnInternalValueChanged( value );
    }

    private int SingleByMonthDay = 1;

    private Task OnSingleByMonthDayChanged( int value )
    {
        SingleByMonthDay = value;
        ByMonthDay = new List<int> { SingleByMonthDay };
        return ByMonthDayChanged.InvokeAsync( ByMonthDay );
    }

    string InternalMonthlyRule { get; set; } = "monthly-rule-by-day";

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

    [Parameter] public List<int> ByMonthDay { get; set; }

    [Parameter] public EventCallback<List<int>> ByMonthDayChanged { get; set; }

    [Parameter] public SchedulerWeek? ByWeek { get; set; }

    [Parameter] public EventCallback<SchedulerWeek?> ByWeekChanged { get; set; }

    [Parameter] public DayOfWeek? ByWeekDay { get; set; }

    [Parameter] public EventCallback<DayOfWeek?> ByWeekDayChanged { get; set; }

    [Parameter] public bool ShowMonth { get; set; }

    [Parameter] public SchedulerMonth? ByMonth { get; set; }

    [Parameter] public EventCallback<SchedulerMonth?> ByMonthChanged { get; set; }
}
