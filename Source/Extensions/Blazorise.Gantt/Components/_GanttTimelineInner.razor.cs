#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt.Components;

/// <summary>
/// Internal inner timeline container used by the Gantt component.
/// </summary>
public partial class _GanttTimelineInner : BaseComponent
{
    #region Methods

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-gantt-timeline-inner" );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the content rendered inside the inner timeline container.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}