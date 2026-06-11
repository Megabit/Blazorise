#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting.Internal;

/// <summary>
/// Renders the report designer toolbox and fields explorer.
/// </summary>
public partial class _ReportDesignerToolboxPanel
{
    #region Members

    private const string FieldsExplorerTab = "FieldsExplorer";

    private const string ToolboxTab = "Toolbox";

    private string selectedTab = ToolboxTab;

    #endregion

    #region Methods

    private Color GetTabColor( string tab )
    {
        return string.Equals( selectedTab, tab, StringComparison.Ordinal ) ? Color.Primary : Color.Light;
    }

    private Task SelectTab( string tab )
    {
        selectedTab = tab;

        return Task.CompletedTask;
    }

    #endregion

    #region Properties

    private bool IsToolboxSelected => string.Equals( selectedTab, ToolboxTab, StringComparison.Ordinal );

    private IReadOnlyList<ReportDesignerDataSourceNode> DataSources
        => ReportDataSourceExplorer.ResolveDataSourceDictionary( Definition, DataSourceName ).ToList();

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

    #endregion
}