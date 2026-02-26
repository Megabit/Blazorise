#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt.Components;

public partial class _GanttTimeRow : BaseComponent
{
    #region Methods

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-gantt-time-row" );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}