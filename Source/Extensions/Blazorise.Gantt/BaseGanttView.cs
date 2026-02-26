#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Base class for all Gantt chart view definitions.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public class BaseGanttView<TItem> : ComponentBase
{
    /// <summary>
    /// Parent Gantt chart component.
    /// </summary>
    [CascadingParameter] public Gantt<TItem> Gantt { get; set; }

    /// <summary>
    /// Height of each timeline row in pixels.
    /// </summary>
    [Parameter] public double RowHeight { get; set; } = 44;

    /// <summary>
    /// Width of each timeline cell in pixels.
    /// </summary>
    [Parameter] public double TimelineCellWidth { get; set; } = 72;

    /// <summary>
    /// Template for rendering the row cell.
    /// </summary>
    [Parameter] public RenderFragment<GanttRowContext<TItem>> RowTemplate { get; set; }

    /// <summary>
    /// Template for rendering the timeline item bar.
    /// </summary>
    [Parameter] public RenderFragment<GanttItemContext<TItem>> ItemTemplate { get; set; }
}