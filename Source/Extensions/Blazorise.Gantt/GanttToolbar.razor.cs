#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt;

/// <summary>
/// Marker component used to enable the default toolbar in <see cref="Gantt{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public partial class GanttToolbar<TItem>
{
    /// <summary>
    /// Registers toolbar with parent chart.
    /// </summary>
    protected override void OnInitialized()
    {
        Gantt?.NotifyGanttToolbar( this );

        base.OnInitialized();
    }

    /// <summary>
    /// Parent chart.
    /// </summary>
    [CascadingParameter] public Gantt<TItem> Gantt { get; set; }

    /// <summary>
    /// Optional child content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}