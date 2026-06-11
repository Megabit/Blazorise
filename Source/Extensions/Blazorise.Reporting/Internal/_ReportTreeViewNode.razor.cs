#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders a cached row for the internal report tree view.
/// </summary>
public partial class _ReportTreeViewNode
{
    private readonly ClassBuilder iconClassBuilder;

    private string IconClass => iconClassBuilder.Class;

    private IconName NodeIconName
        => Node?.Kind switch
        {
            ReportTreeNodeKind.Report => IconName.FileAlt,
            ReportTreeNodeKind.SourceFields => IconName.Database,
            ReportTreeNodeKind.SpecialFields => IconName.Magic,
            ReportTreeNodeKind.DataSource => IconName.Server,
            ReportTreeNodeKind.Field => IconName.Tag,
            ReportTreeNodeKind.Band => IconName.BorderAll,
            ReportTreeNodeKind.Table => IconName.Table,
            ReportTreeNodeKind.Image => IconName.Image,
            ReportTreeNodeKind.Line => IconName.GripLines,
            ReportTreeNodeKind.Rectangle => IconName.Square,
            ReportTreeNodeKind.PageBreak => IconName.File,
            ReportTreeNodeKind.Folder => IconName.Folder,
            _ => IconName.TextHeight,
        };

    private string RowClass => ClassNames;

    private string RowStyle => StyleNames;

    /// <summary>
    /// Initializes a new _ReportTreeViewNode component instance.
    /// </summary>
    public _ReportTreeViewNode()
    {
        iconClassBuilder = new( BuildIconClasses );
    }

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        if ( parameters.TryGetValue<ReportTreeNode>( nameof( Node ), out _ ) )
        {
            iconClassBuilder.Dirty();
            DirtyClasses();
        }

        if ( parameters.TryGetValue<EventCallback<ReportTreeNode>>( nameof( NodeClicked ), out _ ) )
            DirtyClasses();

        if ( parameters.TryGetValue<int>( nameof( Level ), out var paramLevel ) && paramLevel != Level )
            DirtyStyles();

        return base.SetParametersAsync( parameters );
    }

    private void BuildIconClasses( ClassBuilder builder )
    {
        builder.Append( "b-report-treeview-icon" );
        builder.Append( Node is null ? null : $"kind-{Node.Kind.ToString().ToLowerInvariant()}" );
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-report-treeview-row" );
        builder.Append( "selectable", Node.Selectable && NodeClicked.HasDelegate );
        builder.Append( "draggable", Node.Draggable );
        builder.Append( "active", Node.Selected );
    }

    /// <inheritdoc />
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( $"--b-report-treeview-level:{Level}" );
    }

    private async Task OnNodeClicked()
    {
        if ( Node.Selectable && NodeClicked.HasDelegate )
            await NodeClicked.InvokeAsync( Node );
    }

    private async Task OnNodeContextMenu( MouseEventArgs eventArgs )
    {
        if ( Node.Selectable && NodeContextMenu.HasDelegate )
            await NodeContextMenu.InvokeAsync( new ReportTreeNodeMouseEventArgs( Node, eventArgs ) );
    }

    private async Task OnNodeDragStarted( DragEventArgs eventArgs )
    {
        if ( Node.Draggable && NodeDragStarted.HasDelegate )
            await NodeDragStarted.InvokeAsync( new ReportTreeNodeDragEventArgs( Node, eventArgs ) );
    }

    private async Task OnNodeDragEnded()
    {
        if ( Node.Draggable && NodeDragEnded.HasDelegate )
            await NodeDragEnded.InvokeAsync( Node );
    }

    /// <summary>
    /// Tree node rendered by the row component.
    /// </summary>
    [Parameter] public ReportTreeNode Node { get; set; }

    /// <summary>
    /// Tree indentation level.
    /// </summary>
    [Parameter] public int Level { get; set; }

    /// <summary>
    /// Indicates that the node is expanded.
    /// </summary>
    [Parameter] public bool Expanded { get; set; }

    /// <summary>
    /// Indicates that the node has children.
    /// </summary>
    [Parameter] public bool HasChildren { get; set; }

    /// <summary>
    /// Raised when the node expand or collapse toggle is clicked.
    /// </summary>
    [Parameter] public EventCallback ToggleClicked { get; set; }

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
}