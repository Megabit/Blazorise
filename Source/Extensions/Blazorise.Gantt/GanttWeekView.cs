#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Week view configuration.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public partial class GanttWeekView<TItem> : BaseGanttView<TItem>
{
    /// <summary>
    /// Optional first day of week override for week view calculations.
    /// </summary>
    [Parameter] public DayOfWeek? FirstDayOfWeek { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Gantt?.NotifyGanttWeekView( this );

        base.OnInitialized();
    }
}