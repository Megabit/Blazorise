#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a node in a <see cref="DockLayout"/> tree.
/// </summary>
public partial class _DockNodeRenderer : BaseComponent
{
    #region Methods

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        DirtyClasses();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        if ( Node?.Kind == DockNodeKind.Split )
        {
            builder.Append( ClassProvider.DockSplit() );
            builder.Append( ClassProvider.DockSplitOrientation( Node.Orientation ) );
        }

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    private string SplitStyle => Context?.GetDockSplitStyle( Node );

    private DockPanePosition FirstSplitterDock => Node?.Orientation == DockSplitOrientation.Vertical
        ? DockPanePosition.Top
        : DockPanePosition.Left;

    private DockPanePosition SecondSplitterDock => Node?.Orientation == DockSplitOrientation.Vertical
        ? DockPanePosition.Bottom
        : DockPanePosition.Right;

    private bool CanResize => Context is not null
        && Node?.Kind == DockNodeKind.Split
        && SplitterDock is not null
        && !string.IsNullOrWhiteSpace( SplitNodeId )
        && Context.CanResizeDockNode( Node );

    private DockNodeState Node => Context?.GetNode( NodeId );

    [CascadingParameter] internal DockLayoutContext Context { get; set; }

    /// <summary>
    /// Gets or sets the rendered node id.
    /// </summary>
    [Parameter] public string NodeId { get; set; }

    /// <summary>
    /// Gets or sets the local splitter side for the rendered node.
    /// </summary>
    [Parameter] public DockPanePosition? SplitterDock { get; set; }

    /// <summary>
    /// Gets or sets the split node that owns the rendered node splitter.
    /// </summary>
    [Parameter] public string SplitNodeId { get; set; }

    #endregion
}