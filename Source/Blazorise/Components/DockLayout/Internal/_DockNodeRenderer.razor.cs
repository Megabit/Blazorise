#region Using directives
using System.Threading.Tasks;
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
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( ( parameters.TryGetValue<string>( nameof( NodeId ), out var nodeId ) && NodeId != nodeId )
             || ( parameters.TryGetValue<int>( nameof( RenderVersion ), out var renderVersion ) && RenderVersion != renderVersion ) )
            DirtyClasses();

        return base.SetParametersAsync( parameters );
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

    [CascadingParameter] internal DockLayoutContext Context { get; set; }

    private DockNodeState Node => Context?.GetNode( NodeId );

    /// <summary>
    /// Gets or sets the rendered node id.
    /// </summary>
    [Parameter] public string NodeId { get; set; }

    /// <summary>
    /// Gets or sets the layout render version.
    /// </summary>
    [Parameter] public int RenderVersion { get; set; }

    /// <summary>
    /// Gets or sets the layout content render version.
    /// </summary>
    [Parameter] public int ContentRenderVersion { get; set; }

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