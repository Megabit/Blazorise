#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Container for chart view definitions.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public partial class GanttViews<TItem>
{
    /// <summary>
    /// Parent chart.
    /// </summary>
    [CascadingParameter] public Gantt<TItem> Gantt { get; set; }

    /// <summary>
    /// Child views content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}