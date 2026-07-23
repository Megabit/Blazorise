#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Defines an initial tab group inside a <see cref="DockLayout"/>.
/// </summary>
public partial class DockTabs : BaseComponent
{
    #region Members

    private DockNodeCollector childCollector = new();

    private DockNodeState node;

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
        DockNodeState currentNode = Node;
        List<string> paneNames = new();

        foreach ( DockNodeState childNode in childCollector.Nodes )
        {
            if ( childNode.Kind == DockNodeKind.Pane && !string.IsNullOrWhiteSpace( childNode.PaneName ) )
                paneNames.Add( childNode.PaneName );
        }

        string activePane = ActivePane;

        if ( string.IsNullOrWhiteSpace( activePane ) && paneNames.Count > 0 )
            activePane = paneNames[0];

        currentNode.Panes.Clear();
        currentNode.Panes.AddRange( paneNames );
        currentNode.ActivePane = activePane;
    }

    #endregion

    #region Properties

    internal DockNodeCollector ChildCollector => childCollector;

    internal DockNodeState Node => node ??= new()
    {
        Kind = DockNodeKind.Tabs,
    };

    [CascadingParameter] internal DockNodeCollector ParentCollector { get; set; }

    [CascadingParameter] internal DockLayout ParentDockLayout { get; set; }

    /// <summary>
    /// Defines the active pane name.
    /// </summary>
    [Parameter] public string ActivePane { get; set; }

    /// <summary>
    /// Specifies the tab child content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}