using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Localization;
using Microsoft.AspNetCore.Components;

namespace Blazorise.Scheduler.Components;

public partial class _SchedulerWeeklyRulePicker<TItem> : BaseComponent, IDisposable
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

    protected bool IsDaySelected( DayOfWeek day )
        => ByDay?.Contains( day ) == true;

    protected Background GetBackgroundColor( DayOfWeek day )
        => IsDaySelected( day ) ? Background.Secondary : Background.Default;

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

    [Parameter] public List<DayOfWeek> ByDay { get; set; } = new() { DateTime.Today.AddDays( 1 ).DayOfWeek };

    [Parameter] public EventCallback<List<DayOfWeek>> ByDayChanged { get; set; }

    [Parameter] public DayOfWeek FirstDayOfWeek { get; set; }
}
