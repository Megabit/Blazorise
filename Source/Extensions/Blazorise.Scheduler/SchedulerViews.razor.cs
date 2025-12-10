#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// A container component that hosts one or more scheduler view components (e.g., Day, Week, Month).
/// Used to group all available views for the <see cref="Scheduler{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">
/// The type of the items to be scheduled and displayed within the views.
/// </typeparam>
public partial class SchedulerViews<TItem>
{
    #region Properties

    /// <summary>
    /// Gets or sets the scheduler component that the views belong to.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the component.
    /// </summary>
    /// <remarks>
    /// This property allows developers to define custom content within the <see cref="SchedulerViews{TItem}"/> component.
    /// </remarks>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
