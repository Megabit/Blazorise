#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt.Components;

/// <summary>
/// Internal timeline pane container used by the Gantt component.
/// </summary>
public partial class _GanttTimelinePane : BaseComponent
{
    #region Methods

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-gantt-timeline-pane" );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the content rendered inside the timeline pane.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}