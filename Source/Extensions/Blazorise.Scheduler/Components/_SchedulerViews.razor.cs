#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Scheduler.Components;

/// <summary>
/// Container component used to wrap and organize multiple scheduler views
/// (e.g., day, week, month) within the <see cref="Scheduler{TItem}"/>.
/// </summary>
/// <typeparam name="TItem">The type of the scheduler item.</typeparam>
public partial class _SchedulerViews<TItem> : BaseComponent
{
    #region Methods

    /// <summary>
    /// Builds the CSS classes for the scheduler views container.
    /// </summary>
    /// <param name="builder">The class builder.</param>
    override protected void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-scheduler-views" );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the content to be rendered inside the component.
    /// </summary>
    /// <remarks>
    /// This property allows developers to define custom content within the <see cref="_SchedulerViews{TItem}"/> component.
    /// Typically, this includes one or more view components like day, week, or month views.
    /// </remarks>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}
