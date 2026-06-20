#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Defines an initial tab group inside a <see cref="DockLayout"/>.
/// </summary>
public partial class DockTabs : BaseComponent
{
    #region Members

    private string activePane;

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
        node ??= new()
        {
            Kind = DockNodeKind.Tabs,
        };

        node.Panes.Clear();
        node.ActivePane = ActivePane;

        foreach ( DockNodeState childNode in childCollector.Nodes )
        {
            if ( childNode.Kind == DockNodeKind.Pane && !string.IsNullOrWhiteSpace( childNode.PaneName ) )
                node.Panes.Add( childNode.PaneName );
        }

        if ( string.IsNullOrWhiteSpace( node.ActivePane ) && node.Panes.Count > 0 )
            node.ActivePane = node.Panes[0];

        return node;
    }

    #endregion

    #region Properties

    internal DockNodeCollector ChildCollector => childCollector;

    internal DockNodeState Node => BuildNode();

    [CascadingParameter] internal DockNodeCollector ParentCollector { get; set; }

    [CascadingParameter] internal DockLayout ParentDockLayout { get; set; }

    /// <summary>
    /// Defines the active pane name.
    /// </summary>
    [Parameter]
    public string ActivePane
    {
        get => activePane;
        set => activePane = value;
    }

    /// <summary>
    /// Specifies the tab child content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}