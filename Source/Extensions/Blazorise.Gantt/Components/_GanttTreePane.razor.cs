#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt.Components;

/// <summary>
/// Internal tree pane container used by the Gantt component.
/// </summary>
public partial class _GanttTreePane : BaseComponent
{
    #region Methods

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-gantt-tree-pane" );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the content rendered inside the tree pane.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}