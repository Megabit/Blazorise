#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Year view configuration.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public partial class GanttYearView<TItem> : BaseGanttView<TItem>
{
    /// <summary>
    /// Specifies the timeline scale used to render the year view columns.
    /// </summary>
    [Parameter] public GanttYearViewTimelineScale TimelineScale { get; set; } = GanttYearViewTimelineScale.Month;

    /// <summary>
    /// Specifies the first day of the week used when <see cref="TimelineScale"/> is <see cref="GanttYearViewTimelineScale.Week"/>.
    /// </summary>
    [Parameter] public DayOfWeek? FirstDayOfWeek { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Gantt?.NotifyGanttYearView( this );

        base.OnInitialized();
    }
}