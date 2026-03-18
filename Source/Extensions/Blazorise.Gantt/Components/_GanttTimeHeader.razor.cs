#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt.Components;

/// <summary>
/// Internal timeline header container used by the Gantt component.
/// </summary>
public partial class _GanttTimeHeader : BaseComponent
{
    #region Methods

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-gantt-time-header" );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the content rendered inside the timeline header.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}