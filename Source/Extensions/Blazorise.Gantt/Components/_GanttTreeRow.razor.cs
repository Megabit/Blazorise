#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt.Components;

public partial class _GanttTreeRow : BaseComponent
{
    #region Methods

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-gantt-tree-row" );
        builder.Append( "b-gantt-tree-row-selected", Selected );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets callback invoked when row is clicked.
    /// </summary>
    [Parameter] public EventCallback Clicked { get; set; }

    /// <summary>
    /// Gets or sets callback invoked when row is double-clicked.
    /// </summary>
    [Parameter] public EventCallback DoubleClicked { get; set; }

    /// <summary>
    /// Gets or sets whether row is currently selected.
    /// </summary>
    [Parameter] public bool Selected { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}