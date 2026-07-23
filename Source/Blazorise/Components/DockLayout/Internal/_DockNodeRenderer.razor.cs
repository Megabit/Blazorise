#region Using directives
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders a node in a <see cref="DockLayout"/> tree.
/// </summary>
public partial class _DockNodeRenderer : _BaseDockRenderer
{
    #region Members

    private int version;

    #endregion

    #region Methods

    /// <inheritdoc/>
    private protected override bool IsAffected( DockLayoutChange change )
        => change.Kind == DockLayoutChangeKind.Tree
            || change.Kind == DockLayoutChangeKind.Node
                && DockLayoutTreeQuery.FindNodeById( Node, change.NodeId ) is not null;

    /// <inheritdoc/>
    private protected override void OnDockLayoutChanged( DockLayoutChange change )
    {
        version++;

        if ( change.Kind == DockLayoutChangeKind.Tree )
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

    private bool CanResizeSplit => Context?.CanResizeDockSplit( Node?.Id ) == true;

    private bool FirstTrackResizable => Context?.CanResizeDockNode( Node?.First ) == true;

    private bool SecondTrackResizable => Context?.CanResizeDockNode( Node?.Second ) == true;

    private ( string SplitId, string StartId, string EndId ) SplitterKey
        => ( Node?.Id, Node?.First?.Id, Node?.Second?.Id );

    private DockNodeState Node => Context?.GetNode( NodeId );

    private int Version => version;

    /// <summary>
    /// Gets or sets the rendered node id.
    /// </summary>
    [Parameter] public string NodeId { get; set; }

    /// <summary>
    /// Indicates whether the rendered leaf belongs to a resizable split track.
    /// </summary>
    [Parameter] public bool Resizable { get; set; }

    #endregion
}