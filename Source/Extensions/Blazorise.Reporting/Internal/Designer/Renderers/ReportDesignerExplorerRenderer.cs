#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDesignerExplorerRenderer
{
    #region Methods

    internal static void RenderReportExplorer(
        RenderTreeBuilder builder,
        object eventReceiver,
        ReportDefinition definition,
        bool reportSelected,
        int? selectedSectionIndex,
        string selectedElementKey,
        Func<string, bool> isElementSelected,
        Func<ReportTreeNode, Task> nodeClicked,
        Func<ReportTreeNodeMouseEventArgs, Task> nodeContextMenu )
    {
        builder.OpenElement( "h5" );
        builder.Content( "Report Explorer" );
        builder.CloseElement();

        builder.OpenComponent<_ReportTreeView>();
        builder.Attribute( "Nodes", ReportDesignerTreeBuilder.BuildReportExplorerNodes(
            definition,
            reportSelected,
            selectedSectionIndex,
            selectedElementKey,
            isElementSelected ) );
        builder.Attribute( "NodeClicked", EventCallback.Factory.Create<ReportTreeNode>( eventReceiver, nodeClicked ) );
        builder.Attribute( "NodeContextMenu", EventCallback.Factory.Create<ReportTreeNodeMouseEventArgs>( eventReceiver, nodeContextMenu ) );
        builder.CloseComponent();
    }

    internal static void RenderDataDictionary(
        RenderTreeBuilder builder,
        object eventReceiver,
        ReportDefinition definition,
        string dataSourceName,
        Func<ReportTreeNodeDragEventArgs, Task> toolboxNodeDragStarted,
        Func<ReportTreeNodeDragEventArgs, Task> fieldsNodeDragStarted,
        Func<ReportTreeNode, Task> nodeDragEnded )
    {
        var dataSources = ReportDataSourceExplorer.ResolveDataSourceDictionary( definition, dataSourceName ).ToList();

        builder.OpenElement( "h5" );
        builder.Content( "Toolbox" );
        builder.CloseElement();

        builder.OpenComponent<_ReportTreeView>();
        builder.Attribute( "Nodes", ReportDesignerTreeBuilder.BuildToolboxNodes() );
        builder.Attribute( "NodeDragStarted", EventCallback.Factory.Create<ReportTreeNodeDragEventArgs>( eventReceiver, toolboxNodeDragStarted ) );
        builder.Attribute( "NodeDragEnded", EventCallback.Factory.Create<ReportTreeNode>( eventReceiver, nodeDragEnded ) );
        builder.CloseComponent();

        builder.OpenElement( "h5" );
        builder.Class( "b-report-dictionary-title" );
        builder.Content( "Fields explorer" );
        builder.CloseElement();

        if ( dataSources.Count == 0 )
        {
            builder.OpenComponent<Paragraph>();
            builder.Attribute( "TextColor", TextColor.Secondary );
            builder.Attribute( "ChildContent", (RenderFragment)( paragraphBuilder => paragraphBuilder.Content( "No data source fields." ) ) );
            builder.CloseComponent();
            return;
        }

        builder.OpenComponent<_ReportTreeView>();
        builder.Attribute( "Nodes", ReportDesignerTreeBuilder.BuildFieldsExplorerNodes( dataSources ) );
        builder.Attribute( "NodeDragStarted", EventCallback.Factory.Create<ReportTreeNodeDragEventArgs>( eventReceiver, fieldsNodeDragStarted ) );
        builder.Attribute( "NodeDragEnded", EventCallback.Factory.Create<ReportTreeNode>( eventReceiver, nodeDragEnded ) );
        builder.CloseComponent();
    }

    #endregion
}