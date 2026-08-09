#region Using directives
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Defines an initial split node inside a <see cref="DockLayout"/>.
/// </summary>
public partial class DockSplit : BaseComponent, IDisposable
{
    #region Members

    private DockNodeCollector childCollector;

    private DockNodeState node;

    private Orientation orientation;

    private double ratio = 0.5;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        ParentCollector?.AddNode( Node );
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await base.OnAfterRenderAsync( firstRender );

        if ( !firstRender )
            return;

        SynchronizeNode();

        if ( ParentDockLayout is not null )
            await ParentDockLayout.NotifyDefinitionChanged();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
            ParentCollector?.RemoveNode( Node );

        base.Dispose( disposing );
    }

    private void SynchronizeNode()
    {
        DockNodeState first = ChildCollector.Nodes.Count > 0 ? ChildCollector.Nodes[0] : null;
        DockNodeState second = ChildCollector.Nodes.Count > 1 ? ChildCollector.Nodes[1] : null;
        DockNodeState currentNode = Node;

        currentNode.Orientation = Orientation;
        currentNode.Ratio = Ratio;
        currentNode.First = first;
        currentNode.Second = second;
    }

    #endregion

    #region Properties

    internal DockNodeCollector ChildCollector => childCollector ??= new( SynchronizeNode );

    internal DockNodeState Node => node ??= new()
    {
        Kind = DockNodeKind.Split,
        Orientation = orientation,
        Ratio = ratio,
    };

    [CascadingParameter] internal DockNodeCollector ParentCollector { get; set; }

    [CascadingParameter] internal DockLayout ParentDockLayout { get; set; }

    /// <summary>
    /// Defines the initial split orientation.
    /// </summary>
    [Parameter]
    public Orientation Orientation
    {
        get => orientation;
        set
        {
            if ( orientation == value )
                return;

            orientation = value;

            if ( node is not null )
                node.Orientation = value;
        }
    }

    /// <summary>
    /// Defines the initial first child ratio.
    /// </summary>
    [Parameter]
    public double Ratio
    {
        get => ratio;
        set
        {
            if ( ratio == value )
                return;

            ratio = value;

            if ( node is not null )
                node.Ratio = value;
        }
    }

    /// <summary>
    /// Specifies the split child content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}