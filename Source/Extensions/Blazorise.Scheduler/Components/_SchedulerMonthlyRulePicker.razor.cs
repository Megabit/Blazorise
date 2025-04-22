using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Scheduler.Components;

/// <summary>
/// A UI component used to select a monthly recurrence rule pattern,
/// either by specific day of the month or by week and weekday.
/// </summary>
/// <typeparam name="TItem">The type of the scheduler item.</typeparam>
public partial class _SchedulerMonthlyRulePicker<TItem> : RadioGroup<string>, IDisposable
{
    #region Members

    private int SingleByMonthDay = 1;

    #endregion

    #region Methods

    /// <summary>
    /// Subscribes to localization change events.
    /// </summary>
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
    /// Called when the localization language changes. Triggers UI update.
    /// </summary>
    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Updates internal state when parameters are set or changed externally.
    /// </summary>
    /// <param name="parameters">The parameter view.</param>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<List<int>>( nameof( ByMonthDay ), out var paramByMonthDay ) && !ByMonthDay.AreEqual( paramByMonthDay ) )
        {
            SingleByMonthDay = paramByMonthDay?.FirstOrDefault() ?? 1;
        }

        return base.SetParametersAsync( parameters );
    }

    /// <summary>
    /// Updates the internal rule value when the selected radio changes.
    /// </summary>
    /// <param name="value">The selected value.</param>
    protected override Task OnInternalValueChanged( string value )
    {
        InternalMonthlyRule = value;

        return base.OnInternalValueChanged( value );
    }

    /// <summary>
    /// Handles changes to the single "ByMonthDay" value from the UI.
    /// </summary>
    /// <param name="value">The selected day of month.</param>
    private Task OnSingleByMonthDayChanged( int value )
    {
        SingleByMonthDay = value;
        ByMonthDay = new List<int> { SingleByMonthDay };
        return ByMonthDayChanged.InvokeAsync( ByMonthDay );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Stores the currently selected monthly rule ("by-day" or "by-week").
    /// </summary>
    string InternalMonthlyRule { get; set; } = "monthly-rule-by-day";

    /// <summary>
    /// Injected localizer for localized display text.
    /// </summary>
    [Inject] protected ITextLocalizer<Scheduler<TItem>> Localizer { get; set; }

    /// <summary>
    /// Service to detect localization changes at runtime.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Gets or sets the parent scheduler component.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the selected days of the month (e.g. 1, 15, 30).
    /// </summary>
    [Parameter] public List<int> ByMonthDay { get; set; }

    /// <summary>
    /// Fires when the selected "ByMonthDay" value changes.
    /// </summary>
    [Parameter] public EventCallback<List<int>> ByMonthDayChanged { get; set; }

    /// <summary>
    /// Gets or sets the week number in the month (e.g. first, second, last).
    /// </summary>
    [Parameter] public SchedulerWeek? ByWeek { get; set; }

    /// <summary>
    /// Fires when the selected week position changes.
    /// </summary>
    [Parameter] public EventCallback<SchedulerWeek?> ByWeekChanged { get; set; }

    /// <summary>
    /// Gets or sets the day of the week (e.g. Monday, Friday).
    /// </summary>
    [Parameter] public DayOfWeek? ByWeekDay { get; set; }

    /// <summary>
    /// Fires when the selected weekday changes.
    /// </summary>
    [Parameter] public EventCallback<DayOfWeek?> ByWeekDayChanged { get; set; }

    /// <summary>
    /// Gets or sets whether the month dropdown is visible.
    /// </summary>
    [Parameter] public bool ShowMonth { get; set; }

    /// <summary>
    /// Gets or sets the selected month (optional).
    /// </summary>
    [Parameter] public SchedulerMonth? ByMonth { get; set; }

    /// <summary>
    /// Fires when the selected month changes.
    /// </summary>
    [Parameter] public EventCallback<SchedulerMonth?> ByMonthChanged { get; set; }

    #endregion
}
