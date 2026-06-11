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
/// Renders the report designer toolbox and fields explorer.
/// </summary>
public partial class _ReportDesignerToolboxPanel
{
    /// <summary>
    /// Report definition used to discover toolbox and data source fields.
    /// </summary>
    [Parameter] public ReportDefinition Definition { get; set; }

    /// <summary>
    /// Default report data source name shown in the field tree.
    /// </summary>
    [Parameter] public string DataSourceName { get; set; }

    /// <summary>
    /// Raised when a toolbox item starts dragging.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNodeDragEventArgs> ToolboxNodeDragStarted { get; set; }

    /// <summary>
    /// Raised when a field tree item starts dragging.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNodeDragEventArgs> FieldsNodeDragStarted { get; set; }

    /// <summary>
    /// Raised when a tree node drag operation ends.
    /// </summary>
    [Parameter] public EventCallback<ReportTreeNode> NodeDragEnded { get; set; }
}