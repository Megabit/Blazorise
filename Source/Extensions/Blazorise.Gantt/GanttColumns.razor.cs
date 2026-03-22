#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Container for declarative gantt columns.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public partial class GanttColumns<TItem>
{
    /// <summary>
    /// Parent gantt component.
    /// </summary>
    [CascadingParameter] public Gantt<TItem> Gantt { get; set; }

    /// <summary>
    /// Child content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}