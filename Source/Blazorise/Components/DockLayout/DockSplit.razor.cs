#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Defines an initial split node inside a <see cref="DockLayout"/>.
/// </summary>
public partial class DockSplit : BaseComponent
{
    #region Members

    private DockNodeCollector childCollector = new();

    private DockNodeState node;

    private DockSplitOrientation orientation;

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

    private void SynchronizeNode()
    {
        DockNodeState first = childCollector.Nodes.Count > 0 ? childCollector.Nodes[0] : null;
        DockNodeState second = childCollector.Nodes.Count > 1 ? childCollector.Nodes[1] : null;
        DockNodeState currentNode = Node;

        currentNode.Orientation = Orientation;
        currentNode.Ratio = Ratio;
        currentNode.First = first;
        currentNode.Second = second;
    }

    #endregion

    #region Properties

    internal DockNodeCollector ChildCollector => childCollector;

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
    public DockSplitOrientation Orientation
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