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

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ParentCollector?.AddNode( Node );
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        await base.OnAfterRenderAsync( firstRender );

        bool nodeChanged = SynchronizeNode();

        if ( ( nodeChanged || firstRender ) && ParentDockLayout is not null )
            await ParentDockLayout.NotifyDefinitionChanged();
    }

    private bool SynchronizeNode()
    {
        DockNodeState first = childCollector.Nodes.Count > 0 ? childCollector.Nodes[0] : null;
        DockNodeState second = childCollector.Nodes.Count > 1 ? childCollector.Nodes[1] : null;
        DockNodeState currentNode = Node;

        if ( currentNode.Orientation == Orientation
            && currentNode.Ratio == Ratio
            && currentNode.First == first
            && currentNode.Second == second )
            return false;

        currentNode.Orientation = Orientation;
        currentNode.Ratio = Ratio;
        currentNode.First = first;
        currentNode.Second = second;

        return true;
    }

    #endregion

    #region Properties

    internal DockNodeCollector ChildCollector => childCollector;

    internal DockNodeState Node => node ??= new()
    {
        Kind = DockNodeKind.Split,
    };

    [CascadingParameter] internal DockNodeCollector ParentCollector { get; set; }

    [CascadingParameter] internal DockLayout ParentDockLayout { get; set; }

    /// <summary>
    /// Defines the split orientation.
    /// </summary>
    [Parameter] public DockSplitOrientation Orientation { get; set; }

    /// <summary>
    /// Defines the first child ratio.
    /// </summary>
    [Parameter] public double Ratio { get; set; } = 0.5;

    /// <summary>
    /// Specifies the split child content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}