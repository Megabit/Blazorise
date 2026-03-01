#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Defines a regular tree column in <see cref="Gantt{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public class GanttColumn<TItem> : BaseGanttColumn<TItem>
{
    /// <summary>
    /// Gets or sets custom display template.
    /// </summary>
    [Parameter]
    public new RenderFragment<GanttColumnDisplayContext<TItem>> DisplayTemplate
    {
        get => base.DisplayTemplate;
        set => base.DisplayTemplate = value;
    }
}