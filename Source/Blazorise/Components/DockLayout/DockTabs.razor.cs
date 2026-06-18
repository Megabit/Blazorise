#region Using directives
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

    private string activePanel;

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
    protected override void OnAfterRender( bool firstRender )
    {
        base.OnAfterRender( firstRender );

        BuildNode();

        if ( firstRender )
            ParentDockLayout?.NotifyDefinitionChanged();
    }

    private DockNodeState BuildNode()
    {
        node ??= new()
        {
            Kind = DockNodeKind.Tabs,
        };

        node.Panels.Clear();
        node.ActivePanel = ActivePanel;

        foreach ( DockNodeState childNode in childCollector.Nodes )
        {
            if ( childNode.Kind == DockNodeKind.Panel && !string.IsNullOrWhiteSpace( childNode.PanelName ) )
                node.Panels.Add( childNode.PanelName );
        }

        if ( string.IsNullOrWhiteSpace( node.ActivePanel ) && node.Panels.Count > 0 )
            node.ActivePanel = node.Panels[0];

        return node;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the active panel name.
    /// </summary>
    [Parameter]
    public string ActivePanel
    {
        get => activePanel;
        set => activePanel = value;
    }

    /// <summary>
    /// Specifies the tab child content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    [CascadingParameter] internal DockNodeCollector ParentCollector { get; set; }

    [CascadingParameter] internal DockLayout ParentDockLayout { get; set; }

    internal DockNodeCollector ChildCollector => childCollector;

    internal DockNodeState Node => BuildNode();

    #endregion
}