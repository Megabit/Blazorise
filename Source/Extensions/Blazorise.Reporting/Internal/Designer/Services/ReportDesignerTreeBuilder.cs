#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDesignerTreeBuilder
{
    #region Methods

    internal static IReadOnlyList<ReportTreeNode> BuildToolboxNodes()
    {
        return
        [
            new()
            {
                Key = "toolbox",
                Text = "Report Items",
                Kind = ReportTreeNodeKind.Folder,
                Children =
                [
                    CreateToolboxNode( "toolbox:text", "Text", ReportElementType.Text, "Text" ),
                    CreateToolboxNode( "toolbox:image", "Image", ReportElementType.Image, null ),
                    CreateToolboxNode( "toolbox:line", "Line", ReportElementType.Line, null ),
                    CreateToolboxNode( "toolbox:rectangle", "Rectangle", ReportElementType.Rectangle, null ),
                ],
            }
        ];
    }

    internal static IReadOnlyList<ReportTreeNode> BuildFieldsExplorerNodes( IEnumerable<ReportDesignerDataSourceNode> dataSources )
    {
        var dataSourceList = dataSources?.ToList() ?? [];

        if ( dataSourceList.Count == 1 )
        {
            var dataSource = dataSourceList[0];

            return dataSource.Fields.Select( field => BuildFieldExplorerNode( dataSource.Name, field ) ).ToList();
        }

        return
        [
            new()
            {
                Key = "fields:data-sources",
                Text = "Data Sources",
                Kind = ReportTreeNodeKind.Folder,
                Children = dataSourceList.Select( dataSource => new ReportTreeNode
                {
                    Key = $"fields:data-source:{dataSource.Name}",
                    Text = dataSource.Name,
                    Kind = ReportTreeNodeKind.DataSource,
                    Children = dataSource.Fields.Select( field => BuildFieldExplorerNode( dataSource.Name, field ) ).ToList(),
                } ).ToList(),
            }
        ];
    }

    internal static IReadOnlyList<ReportTreeNode> BuildReportExplorerNodes(
        ReportDefinition definition,
        bool reportSelected,
        int? selectedSectionIndex,
        string selectedElementKey,
        Func<string, bool> isElementSelected )
    {
        return
        [
            new()
            {
                Key = "report",
                Text = "Report",
                Kind = ReportTreeNodeKind.Report,
                Selectable = true,
                Selected = reportSelected,
                Children = definition.Sections.Select( ( section, sectionIndex ) => new ReportTreeNode
                {
                    Key = CreateSectionTreeNodeKey( sectionIndex ),
                    Text = ReportDefinitionHelper.GetSectionDisplayName( section ),
                    Detail = ReportDefinitionHelper.GetSectionTypeDisplayName( section.Type ),
                    Kind = ReportTreeNodeKind.Band,
                    Selectable = true,
                    Selected = selectedSectionIndex == sectionIndex && string.IsNullOrWhiteSpace( selectedElementKey ),
                    Children = section.Elements.Select( element =>
                    {
                        var elementKey = ReportDefinitionHelper.EnsureElementId( element );

                        return new ReportTreeNode
                        {
                            Key = CreateElementTreeNodeKey( elementKey ),
                            Text = element.Name ?? element.Text ?? element.Field ?? element.Type.ToString(),
                            Detail = element.Type.ToString(),
                            Kind = ReportDefinitionHelper.GetElementTreeNodeKind( element.Type ),
                            Selectable = true,
                            Selected = isElementSelected( elementKey ),
                        };
                    } ).ToList(),
                } ).ToList(),
            }
        ];
    }

    internal static string CreateSectionTreeNodeKey( int sectionIndex )
        => $"report:section:{sectionIndex}";

    internal static string CreateElementTreeNodeKey( string elementKey )
        => $"report:element:{elementKey}";

    internal static bool TryResolveSectionTreeNode( ReportTreeNode node, out int sectionIndex )
    {
        sectionIndex = -1;

        return node?.Key is not null
            && node.Key.StartsWith( "report:section:", StringComparison.Ordinal )
            && int.TryParse( node.Key["report:section:".Length..], NumberStyles.Integer, CultureInfo.InvariantCulture, out sectionIndex );
    }

    internal static bool TryResolveElementTreeNode( ReportTreeNode node, out string elementKey )
    {
        elementKey = null;

        if ( node?.Key is null || !node.Key.StartsWith( "report:element:", StringComparison.Ordinal ) )
            return false;

        elementKey = node.Key["report:element:".Length..];

        return !string.IsNullOrWhiteSpace( elementKey );
    }

    private static ReportTreeNode CreateToolboxNode( string key, string text, ReportElementType elementType, string elementText )
    {
        return new()
        {
            Key = key,
            Text = text,
            Kind = ReportDefinitionHelper.GetElementTreeNodeKind( elementType ),
            Draggable = true,
            Value = new ReportToolboxTreeNodeValue( elementType, elementText ?? text ),
        };
    }

    private static ReportTreeNode BuildFieldExplorerNode( string dataSourceName, ReportDesignerFieldNode field )
    {
        var hasChildren = field.Children.Count > 0;

        return new()
        {
            Key = $"fields:field:{dataSourceName}:{field.Path}",
            Text = field.Name,
            Detail = hasChildren ? null : ReportDefinitionHelper.GetDataTypeDisplayName( field.DataType ),
            Kind = hasChildren ? ReportTreeNodeKind.Folder : ReportTreeNodeKind.Field,
            Draggable = !hasChildren,
            Value = !hasChildren ? new ReportFieldTreeNodeValue( dataSourceName, field.Path ) : null,
            Children = field.Children.Select( child => BuildFieldExplorerNode( dataSourceName, child ) ).ToList(),
        };
    }

    #endregion
}