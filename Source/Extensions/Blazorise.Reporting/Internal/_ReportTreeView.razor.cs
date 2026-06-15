#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a lightweight tree view used by reporting designer panels.
/// </summary>
public partial class _ReportTreeView
{
    #region Members

    private readonly HashSet<string> collapsedNodeKeys = [];

    private readonly HashSet<string> defaultCollapsedNodeKeys = [];

    private ElementReference treeElement;

    private JSReportingModule reportingModule;

    private bool treeDragImageSuppressed;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        bool collapseByDefault = parameters.TryGetValue<bool>( nameof( CollapseByDefault ), out var paramCollapseByDefault )
            ? paramCollapseByDefault
            : CollapseByDefault;

        if ( !collapseByDefault && CollapseByDefault )
        {
            defaultCollapsedNodeKeys.Clear();
            collapsedNodeKeys.Clear();
        }

        if ( collapseByDefault && parameters.TryGetValue<IReadOnlyList<ReportTreeNode>>( nameof( Nodes ), out var paramNodes ) )
            CollapseNodesByDefault( paramNodes );

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync( bool firstRender )
    {
        bool hasDraggableNodes = HasDraggableNodes( Nodes );

        if ( hasDraggableNodes && !treeDragImageSuppressed )
        {
            EnsureReportingModule();
            await reportingModule.SuppressTreeNativeDragImage( treeElement );
            treeDragImageSuppressed = true;
        }
        else if ( !hasDraggableNodes && treeDragImageSuppressed )
        {
            await reportingModule.ClearTreeNativeDragImage( treeElement );
            treeDragImageSuppressed = false;
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        try
        {
            if ( reportingModule is not null )
            {
                await reportingModule.ClearTreeNativeDragImage( treeElement );
                await reportingModule.DisposeAsync();
            }
        }
        catch ( JSDisconnectedException )
        {
        }
    }

    internal static bool HasChildren( ReportTreeNode node )
        => node?.Children?.Count > 0;

    private void CollapseNodesByDefault( IEnumerable<ReportTreeNode> nodes )
    {
        if ( nodes is null )
            return;

        foreach ( ReportTreeNode node in nodes )
        {
            if ( node?.Key is null )
                continue;

            if ( HasChildren( node ) && defaultCollapsedNodeKeys.Add( node.Key ) )
                collapsedNodeKeys.Add( node.Key );

            CollapseNodesByDefault( node.Children );
        }
    }

    private static bool HasDraggableNodes( IEnumerable<ReportTreeNode> nodes )
    {
        if ( nodes is null )
            return false;

        foreach ( ReportTreeNode node in nodes )
        {
            if ( node.Draggable || HasDraggableNodes( node.Children ) )
                return true;
        }

        return false;
    }

    private void EnsureReportingModule()
    {
        reportingModule ??= new( JSRuntime, VersionProvider, BlazoriseOptions );
    }

    internal bool IsExpanded( ReportTreeNode node )
        => node?.Key is not null && !collapsedNodeKeys.Contains( node.Key );

    internal void ToggleNode( ReportTreeNode node )
    {
        if ( node?.Key is null )
            return;

        if ( !collapsedNodeKeys.Add( node.Key ) )
            collapsedNodeKeys.Remove( node.Key );
    }

    #endregion

    #region Properties

    /// <summary>
    /// JavaScript runtime used to create the local Reporting module.
    /// </summary>
    [Inject] private IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Version provider used to create the local Reporting module.
    /// </summary>
    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Blazorise options used to create the local Reporting module.
    /// </summary>
    [Inject] private BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// Root nodes rendered by the tree view.
    /// </summary>
    [Parameter] public IReadOnlyList<ReportTreeNode> Nodes { get; set; }

    /// <summary>
    /// Collapses nodes the first time they are rendered.
    /// </summary>
    [Parameter] public bool CollapseByDefault { get; set; }

    /// <summary>
    /// Raised when a selectable tree node is clicked.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNode> NodeClicked { get; set; }

    /// <summary>
    /// Raised when a selectable tree node opens its context menu.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNodeMouseEventArgs> NodeContextMenu { get; set; }

    /// <summary>
    /// Raised when dragging starts for a draggable tree node.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNodeDragEventArgs> NodeDragStarted { get; set; }

    /// <summary>
    /// Raised when dragging ends for a draggable tree node.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNode> NodeDragEnded { get; set; }

    #endregion
}