using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Scheduler.Components;

/// <summary>
/// A component for selecting days of the week as part of a weekly recurrence rule.
/// Used within the <see cref="Scheduler{TItem}"/> recurrence editor.
/// </summary>
/// <typeparam name="TItem">The type of the scheduler item.</typeparam>
public partial class _SchedulerWeeklyRulePicker<TItem> : BaseComponent, IDisposable
{
    /// <summary>
    /// Subscribes to localization change notifications.
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
    /// Refreshes the UI when the localization language changes.
    /// </summary>
    private async void OnLocalizationChanged( object sender, EventArgs e )
    {
        await InvokeAsync( StateHasChanged );
    }

    /// <summary>
    /// Toggles selection of the given day in the weekly recurrence rule.
    /// </summary>
    /// <param name="day">The day of the week to toggle.</param>
    /// <returns>A task that completes when the change is propagated.</returns>
    Task OnDayClicked( DayOfWeek day )
    {
        ByDay ??= new List<DayOfWeek>();

        if ( ByDay.Contains( day ) )
        {
            if ( ByDay.Count > 1 )
            {
                ByDay.Remove( day );
            }
        }
        else
        {
            ByDay.Add( day );
        }

        return ByDayChanged.InvokeAsync( ByDay );
    }

    /// <summary>
    /// Determines the color of a button based on whether a specific day is selected.
    /// </summary>
    /// <param name="day">Represents a day of the week to check if it is selected.</param>
    /// <returns>Returns a primary color if the day is selected, otherwise returns a secondary color.</returns>
    protected Color GetButtonColor( DayOfWeek day )
        => IsDaySelected( day ) ? Color.Primary : Color.Secondary;

    /// <summary>
    /// Checks whether the given day is selected.
    /// </summary>
    /// <param name="day">The day of the week to check.</param>
    /// <returns>True if the day is selected, otherwise false.</returns>
    protected bool IsDaySelected( DayOfWeek day )
        => ByDay?.Contains( day ) == true;

    /// <summary>
    /// Returns the background color based on selection state.
    /// </summary>
    /// <param name="day">The day to check.</param>
    /// <returns>A background color indicating whether the day is selected.</returns>
    protected Background GetBackgroundColor( DayOfWeek day )
        => IsDaySelected( day ) ? Background.Secondary : Background.Default;

    /// <summary>
    /// Injected localization service for scheduler-specific text.
    /// </summary>
    [Inject] protected ITextLocalizer<Scheduler<TItem>> Localizer { get; set; }

    /// <summary>
    /// Injected global localization change service.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Gets or sets the parent <see cref="Scheduler{TItem}"/> instance.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the list of selected days of the week for the recurrence rule.
    /// </summary>
    [Parameter] public List<DayOfWeek> ByDay { get; set; } = new() { DateTime.Today.AddDays( 1 ).DayOfWeek };

    /// <summary>
    /// Callback invoked when the selected days of the week change.
    /// </summary>
    [Parameter] public EventCallback<List<DayOfWeek>> ByDayChanged { get; set; }

    /// <summary>
    /// Gets or sets the first day of the week, used for rendering order.
    /// </summary>
    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }
}
