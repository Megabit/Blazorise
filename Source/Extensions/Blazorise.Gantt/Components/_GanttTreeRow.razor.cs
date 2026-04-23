#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Gantt.Components;

/// <summary>
/// Internal tree row renderer used by the Gantt component.
/// </summary>
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
    /// Notifies when row is clicked.
    /// </summary>
    [Parameter] public EventCallback Clicked { get; set; }

    /// <summary>
    /// Notifies when row is double-clicked.
    /// </summary>
    [Parameter] public EventCallback DoubleClicked { get; set; }

    /// <summary>
    /// Determines whether row is currently selected.
    /// </summary>
    [Parameter] public bool Selected { get; set; }

    /// <summary>
    /// Defines the content rendered inside the tree row.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}