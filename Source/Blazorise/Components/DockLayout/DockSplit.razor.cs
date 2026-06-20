#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Defines an initial split node inside a <see cref="DockLayout"/>.
/// </summary>
public partial class DockSplit : BaseComponent
{
    #region Members

    private DockSplitOrientation orientation;

    private double ratio = 0.5;

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

        BuildNode();

        if ( firstRender && ParentDockLayout is not null )
            await ParentDockLayout.NotifyDefinitionChanged();
    }

    private DockNodeState BuildNode()
    {
        DockNodeState first = childCollector.Nodes.Count > 0 ? childCollector.Nodes[0] : null;
        DockNodeState second = childCollector.Nodes.Count > 1 ? childCollector.Nodes[1] : null;

        node ??= new()
        {
            Kind = DockNodeKind.Split,
        };

        node.Orientation = Orientation;
        node.Ratio = Ratio;
        node.First = first;
        node.Second = second;

        return node;
    }

    #endregion

    #region Properties

    internal DockNodeCollector ChildCollector => childCollector;

    internal DockNodeState Node => BuildNode();

    [CascadingParameter] internal DockNodeCollector ParentCollector { get; set; }

    [CascadingParameter] internal DockLayout ParentDockLayout { get; set; }

    /// <summary>
    /// Defines the split orientation.
    /// </summary>
    [Parameter]
    public DockSplitOrientation Orientation
    {
        get => orientation;
        set => orientation = value;
    }

    /// <summary>
    /// Defines the first child ratio.
    /// </summary>
    [Parameter]
    public double Ratio
    {
        get => ratio;
        set => ratio = value;
    }

    /// <summary>
    /// Specifies the split child content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}