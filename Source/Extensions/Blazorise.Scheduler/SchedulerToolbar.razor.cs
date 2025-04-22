#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler;

/// <summary>
/// Represents a container for custom toolbar content within the <see cref="Scheduler{TItem}"/> component.
/// Allows developers to override or extend the default toolbar UI.
/// </summary>
/// <typeparam name="TItem">The type of the items used in the scheduler.</typeparam>
public partial class SchedulerToolbar<TItem>
{
    #region Methods

    /// <summary>
    /// Called when the component is initialized. Notifies the scheduler that a custom toolbar has been provided.
    /// </summary>
    protected override void OnInitialized()
    {
        Scheduler?.NotifySchedulerToolbar( this );

        base.OnInitialized();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the scheduler component that the toolbar belongs to.
    /// </summary>
    [CascadingParameter] public Scheduler<TItem> Scheduler { get; set; }

    /// <summary>
    /// Gets or sets the content to be rendered inside the toolbar.
    /// </summary>
    /// <remarks>
    /// This property allows developers to define custom content within the <see cref="SchedulerToolbar{TItem}"/> component,
    /// such as navigation buttons, view selectors, or custom controls.
    /// </remarks>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
