#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt.Components;

/// <summary>
/// Internal timeline row container used by the Gantt component.
/// </summary>
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

    /// <summary>
    /// Gets or sets the content rendered inside the timeline row.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}