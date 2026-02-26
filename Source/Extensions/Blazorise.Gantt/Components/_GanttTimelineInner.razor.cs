#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt.Components;

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

    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}