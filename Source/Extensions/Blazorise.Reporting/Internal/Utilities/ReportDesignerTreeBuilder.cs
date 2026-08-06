#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Blazorise.Localization;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDesignerTreeBuilder
{
    #region Members

    internal const string FormulaFieldsNodeKey = "fields:formula";

    internal const string RunningTotalFieldsNodeKey = "fields:running-total";

    #endregion

    #region Methods

    internal static IReadOnlyList<ReportTreeNode> BuildToolboxNodes( IReportElementPluginRegistry pluginRegistry, bool allowSubreport = true, TextLocalizerHandler localizer = null )
    {
        List<ReportTreeNode> reportItems =
        [
            CreateToolboxNode( "toolbox:text", Localize( localizer, "Text" ), ReportElementType.Text, "Text" ),
            CreateToolboxNode( "toolbox:image", Localize( localizer, "Image" ), ReportElementType.Image, null ),
            CreateToolboxNode( "toolbox:line", Localize( localizer, "Line" ), ReportElementType.Line, null ),
            CreateToolboxNode( "toolbox:rectangle", Localize( localizer, "Rectangle" ), ReportElementType.Rectangle, null ),
            CreateToolboxNode( "toolbox:panel", Localize( localizer, "Panel" ), ReportElementType.Panel, null ),
            CreateToolboxNode( "toolbox:table", Localize( localizer, "Table" ), ReportElementType.Table, null ),
        ];

        if ( allowSubreport )
            reportItems.Add( CreateToolboxNode( "toolbox:subreport", Localize( localizer, "Subreport" ), ReportElementType.Subreport, null ) );

        List<ReportTreeNode> groups =
        [
            new()
            {
                Key = "toolbox",
                Text = Localize( localizer, "Report Items" ),
                Kind = ReportTreeNodeKind.Folder,
                Children = reportItems,
            }
        ];

        foreach ( IGrouping<string, IReportElementPlugin> category in ( pluginRegistry?.Plugins ?? [] )
                     .Where( plugin => plugin.Descriptor?.ShowInToolbox == true )
                     .GroupBy(
                         plugin => string.IsNullOrWhiteSpace( plugin.Descriptor.Category ) ? "Custom" : plugin.Descriptor.Category,
                         StringComparer.OrdinalIgnoreCase ) )
        {
            List<ReportTreeNode> customItems = category.Select( CreateToolboxNode ).ToList();

            if ( string.Equals( category.Key, "Report Items", StringComparison.OrdinalIgnoreCase ) )
                reportItems.AddRange( customItems );
            else
            {
                groups.Add( new()
                {
                    Key = $"toolbox:category:{category.Key}",
                    Text = category.Key,
                    Kind = ReportTreeNodeKind.Folder,
                    Children = customItems,
                } );
            }
        }

        return groups;
    }

    internal static IReadOnlyList<ReportTreeNode> BuildFieldsExplorerNodes(
        IEnumerable<ReportDesignerDataSourceNode> dataSources,
        IEnumerable<ReportFormulaFieldDefinition> formulaFields,
        IEnumerable<ReportRunningTotalDefinition> runningTotals = null,
        string selectedFormulaFieldName = null,
        string selectedRunningTotalName = null,
        TextLocalizerHandler localizer = null )
    {
        List<ReportDesignerDataSourceNode> dataSourceList = dataSources?.ToList() ?? [];
        List<ReportFormulaFieldDefinition> formulaFieldList = formulaFields?.ToList() ?? [];
        List<ReportRunningTotalDefinition> runningTotalList = runningTotals?.ToList() ?? [];

        return
        [
            BuildSourceFieldsNode( dataSourceList, localizer ),
            BuildFormulaFieldsNode( formulaFieldList, selectedFormulaFieldName, localizer ),
            BuildRunningTotalFieldsNode( runningTotalList, selectedRunningTotalName, localizer ),
            BuildSpecialFieldsNode( localizer ),
        ];
    }

    internal static IReadOnlyList<ReportTreeNode> BuildReportExplorerNodes(
        ReportDefinition definition,
        bool reportSelected,
        int? selectedSectionIndex,
        string selectedElementKey,
        string selectedCellKey,
        Func<string, bool> isElementSelected,
        IReportElementPluginRegistry pluginRegistry,
        bool allowSubreport = true,
        string searchText = null,
        bool currentPageOnly = false,
        TextLocalizerHandler localizer = null )
    {
        searchText = searchText?.Trim();

        return
        [
            new()
            {
                Key = "report",
                Text = Localize( localizer, "Report" ),
                Kind = ReportTreeNodeKind.Report,
                Selectable = true,
                Selected = reportSelected,
                Children = definition.Pages.Select( ( page, pageIndex ) => new ReportTreeNode
                {
                    Key = CreatePageTreeNodeKey( page.Id ),
                    Text = string.IsNullOrWhiteSpace( page.Name ) ? Localize( localizer, "Page {0}", pageIndex + 1 ) : page.Name,
                    Detail = Localize( localizer, "Page" ),
                    Kind = ReportTreeNodeKind.Page,
                    Selectable = true,
                    Children = ( page.Bands ?? [] ).Select( ( section, sectionIndex ) => new ReportTreeNode
                    {
                        Key = CreateSectionTreeNodeKey( page.Id, sectionIndex ),
                        Text = string.IsNullOrWhiteSpace( section.Name )
                            ? Localize( localizer, ReportDefinitionHelper.GetSectionTypeDisplayName( section.Type ) )
                            : section.Name,
                        Detail = Localize( localizer, ReportDefinitionHelper.GetSectionTypeDisplayName( section.Type ) ),
                        Kind = ReportTreeNodeKind.Band,
                        Selectable = true,
                        Selected = ReferenceEquals( page, definition.Page )
                            && selectedSectionIndex == sectionIndex
                            && string.IsNullOrWhiteSpace( selectedElementKey ),
                        Children = section.Elements
                            .Where( element => allowSubreport || element.Type != ReportElementType.Subreport )
                            .Select( element => BuildReportElementNode( element, selectedCellKey, isElementSelected, pluginRegistry, allowSubreport, localizer ) )
                            .ToList(),
                    } )
                    .ToList(),
                } )
                .Where( node => ( !currentPageOnly || string.Equals( node.Key, CreatePageTreeNodeKey( definition.Page.Id ), StringComparison.Ordinal ) )
                    && FilterReportPageNode( node, searchText ) )
                .ToList(),
            }
        ];
    }

    internal static string CreatePageTreeNodeKey( string pageId )
        => $"report:page:{pageId}";

    internal static string CreateSectionTreeNodeKey( string pageId, int sectionIndex )
        => $"report:page:{pageId}:section:{sectionIndex.ToString( CultureInfo.InvariantCulture )}";

    internal static string CreateElementTreeNodeKey( string elementKey )
        => $"report:element:{elementKey}";

    internal static string CreateTableRowTreeNodeKey( string tableKey, int rowIndex )
        => $"report:table-row:{tableKey}:{rowIndex.ToString( CultureInfo.InvariantCulture )}";

    internal static string CreateTableCellTreeNodeKey( string cellKey )
        => $"report:table-cell:{cellKey}";

    internal static bool TryResolveSectionTreeNode( ReportTreeNode node, out string pageId, out int sectionIndex )
    {
        pageId = null;
        sectionIndex = -1;

        if ( node?.Key is null || !node.Key.StartsWith( "report:page:", StringComparison.Ordinal ) )
            return false;

        int separatorIndex = node.Key.IndexOf( ":section:", "report:page:".Length, StringComparison.Ordinal );

        if ( separatorIndex < 0 )
            return false;

        pageId = node.Key["report:page:".Length..separatorIndex];

        return !string.IsNullOrWhiteSpace( pageId )
            && int.TryParse( node.Key[( separatorIndex + ":section:".Length )..], NumberStyles.Integer, CultureInfo.InvariantCulture, out sectionIndex );
    }

    internal static bool TryResolvePageTreeNode( ReportTreeNode node, out string pageId )
    {
        pageId = null;

        if ( node?.Key is null || !node.Key.StartsWith( "report:page:", StringComparison.Ordinal )
            || node.Key.Contains( ":section:", StringComparison.Ordinal ) )
        {
            return false;
        }

        pageId = node.Key["report:page:".Length..];

        return !string.IsNullOrWhiteSpace( pageId );
    }

    internal static bool TryResolveElementTreeNode( ReportTreeNode node, out string elementKey )
    {
        elementKey = null;

        if ( node?.Key is null || !node.Key.StartsWith( "report:element:", StringComparison.Ordinal ) )
            return false;

        elementKey = node.Key["report:element:".Length..];

        return !string.IsNullOrWhiteSpace( elementKey );
    }

    internal static bool TryResolveTableCellTreeNode( ReportTreeNode node, out string cellKey )
    {
        cellKey = null;

        if ( node?.Key is null || !node.Key.StartsWith( "report:table-cell:", StringComparison.Ordinal ) )
            return false;

        cellKey = node.Key["report:table-cell:".Length..];

        return !string.IsNullOrWhiteSpace( cellKey );
    }

    private static ReportTreeNode BuildReportElementNode( ReportElementDefinition element, string selectedCellKey, Func<string, bool> isElementSelected, IReportElementPluginRegistry pluginRegistry, bool allowSubreport = true, TextLocalizerHandler localizer = null )
    {
        var elementKey = ReportDefinitionHelper.EnsureElementId( element );
        IReportElementPlugin plugin = element is ReportCustomElementDefinition customElement
            ? pluginRegistry?.Find( customElement.TypeName )
            : null;

        return new()
        {
            Key = CreateElementTreeNodeKey( elementKey ),
            Text = element.Name ?? ReportElementDefinitionHelper.GetDisplayText( element ),
            Detail = plugin?.Descriptor.DisplayName
                ?? ( element as ReportCustomElementDefinition )?.TypeName
                ?? Localize( localizer, ReportDefinitionHelper.GetElementTypeDisplayName( element.Type ) ),
            Kind = ReportDefinitionHelper.GetElementTreeNodeKind( element.Type ),
            Icon = plugin?.Descriptor.Icon,
            Selectable = true,
            Selected = isElementSelected?.Invoke( elementKey ) == true,
            Children = element switch
            {
                ReportTableElementDefinition table => BuildTableChildNodes( table, elementKey, selectedCellKey, isElementSelected, pluginRegistry, allowSubreport, localizer ),
                ReportPanelElementDefinition panel => ( panel.Elements ?? [] )
                    .Where( child => allowSubreport || child.Type != ReportElementType.Subreport )
                    .Select( child => BuildReportElementNode( child, selectedCellKey, isElementSelected, pluginRegistry, allowSubreport, localizer ) )
                    .ToList(),
                _ => [],
            },
        };
    }

    private static bool FilterReportPageNode( ReportTreeNode node, string searchText )
    {
        if ( string.IsNullOrWhiteSpace( searchText )
             || node.Text?.Contains( searchText, StringComparison.OrdinalIgnoreCase ) == true )
        {
            return true;
        }

        node.Children = node.Children
            .Where( child => FilterReportSectionNode( child, searchText ) )
            .ToList();

        return node.Children.Count > 0;
    }

    private static bool FilterReportSectionNode( ReportTreeNode node, string searchText )
    {
        if ( string.IsNullOrWhiteSpace( searchText )
             || node.Text?.Contains( searchText, StringComparison.OrdinalIgnoreCase ) == true
             || node.Detail?.Contains( searchText, StringComparison.OrdinalIgnoreCase ) == true )
        {
            return true;
        }

        node.Children = node.Children
            .Where( child => FilterReportElementNode( child, searchText ) )
            .ToList();

        return node.Children.Count > 0;
    }

    private static bool FilterReportElementNode( ReportTreeNode node, string searchText )
    {
        if ( string.IsNullOrWhiteSpace( searchText ) )
            return true;

        if ( node.Key.StartsWith( "report:element:", StringComparison.Ordinal )
            && ( node.Text?.Contains( searchText, StringComparison.OrdinalIgnoreCase ) == true
                || node.Detail?.Contains( searchText, StringComparison.OrdinalIgnoreCase ) == true ) )
        {
            return true;
        }

        node.Children = node.Children
            .Where( child => FilterReportElementNode( child, searchText ) )
            .ToList();

        return node.Children.Count > 0;
    }

    private static List<ReportTreeNode> BuildTableChildNodes( ReportTableElementDefinition table, string tableKey, string selectedCellKey, Func<string, bool> isElementSelected, IReportElementPluginRegistry pluginRegistry, bool allowSubreport, TextLocalizerHandler localizer )
    {
        List<ReportTreeNode> rows = [];
        int rowCount = Math.Max(
            table.Rows?.Count ?? 0,
            table.Cells?.Count > 0 ? table.Cells.Max( cell => cell.RowIndex + Math.Max( 1, cell.RowSpan ) ) : 0 );

        for ( int rowIndex = 0; rowIndex < rowCount; rowIndex++ )
        {
            rows.Add( new()
            {
                Key = CreateTableRowTreeNodeKey( tableKey, rowIndex ),
                Text = Localize( localizer, "Row {0}", rowIndex + 1 ),
                Detail = Localize( localizer, "Row" ),
                Kind = ReportTreeNodeKind.TableRow,
                Children = BuildTableCellNodes( table, rowIndex, selectedCellKey, isElementSelected, pluginRegistry, allowSubreport, localizer ),
            } );
        }

        return rows;
    }

    private static List<ReportTreeNode> BuildTableCellNodes( ReportTableElementDefinition table, int rowIndex, string selectedCellKey, Func<string, bool> isElementSelected, IReportElementPluginRegistry pluginRegistry, bool allowSubreport, TextLocalizerHandler localizer )
    {
        return ( table.Cells ?? [] )
            .Where( cell => cell.RowIndex == rowIndex )
            .OrderBy( cell => cell.ColumnIndex )
            .Select( cell =>
            {
                string cellKey = ReportDefinitionHelper.EnsureTableCellId( cell );

                return new ReportTreeNode
                {
                    Key = CreateTableCellTreeNodeKey( cellKey ),
                    Text = Localize( localizer, "Cell {0}", cell.ColumnIndex + 1 ),
                    Detail = GetCellDetail( cell, localizer ),
                    Kind = ReportTreeNodeKind.TableCell,
                    Selectable = true,
                    Selected = string.Equals( selectedCellKey, cellKey, StringComparison.Ordinal ),
                    Children = ( cell.Elements ?? [] )
                        .Where( element => allowSubreport || element.Type != ReportElementType.Subreport )
                        .Select( element => BuildReportElementNode( element, selectedCellKey, isElementSelected, pluginRegistry, allowSubreport, localizer ) )
                        .ToList(),
                };
            } )
            .ToList();
    }

    private static string GetCellDetail( ReportTableCellDefinition cell, TextLocalizerHandler localizer )
    {
        int rowSpan = Math.Max( 1, cell.RowSpan );
        int columnSpan = Math.Max( 1, cell.ColumnSpan );

        return rowSpan == 1 && columnSpan == 1
            ? Localize( localizer, "Cell" )
            : Localize( localizer, "Span {0}x{1}", columnSpan, rowSpan );
    }

    private static ReportTreeNode CreateToolboxNode( string key, string text, ReportElementType elementType, string elementText )
    {
        return new()
        {
            Key = key,
            Text = text,
            Kind = ReportDefinitionHelper.GetElementTreeNodeKind( elementType ),
            Draggable = true,
            Value = new ReportToolboxTreeNodeValue( elementType, null, elementText ?? text ),
        };
    }

    private static ReportTreeNode CreateToolboxNode( IReportElementPlugin plugin )
    {
        ReportElementDescriptor descriptor = plugin.Descriptor;

        return new()
        {
            Key = $"toolbox:custom:{descriptor.TypeName}",
            Text = descriptor.DisplayName,
            Kind = ReportTreeNodeKind.Custom,
            Icon = descriptor.Icon,
            Draggable = true,
            Value = new ReportToolboxTreeNodeValue( null, descriptor.TypeName, descriptor.DisplayName ),
        };
    }

    private static ReportTreeNode BuildFieldExplorerNode( string dataSourceName, ReportDesignerFieldNode field, TextLocalizerHandler localizer )
    {
        bool hasChildren = field.Children.Count > 0;

        return new()
        {
            Key = $"fields:field:{dataSourceName}:{field.Path}",
            Text = field.Name,
            Detail = hasChildren ? null : Localize( localizer, ReportDefinitionHelper.GetDataTypeDisplayName( field.DataType ) ),
            Kind = hasChildren ? ReportTreeNodeKind.Folder : ReportTreeNodeKind.Field,
            Selectable = !hasChildren,
            Draggable = !hasChildren,
            Value = !hasChildren ? new ReportFieldTreeNodeValue( dataSourceName, field.Path ) : null,
            Children = field.Children.Select( child => BuildFieldExplorerNode( dataSourceName, child, localizer ) ).ToList(),
        };
    }

    private static ReportTreeNode BuildSourceFieldsNode( IReadOnlyList<ReportDesignerDataSourceNode> dataSources, TextLocalizerHandler localizer )
    {
        ReportDesignerDataSourceNode singleDataSource = dataSources.Count == 1 ? dataSources[0] : null;

        return new()
        {
            Key = "fields:source",
            Text = Localize( localizer, "Source Fields" ),
            Kind = ReportTreeNodeKind.SourceFields,
            Selectable = singleDataSource is not null,
            Value = singleDataSource is not null ? new ReportDataSourceTreeNodeValue( singleDataSource.BindingName ) : null,
            Children = dataSources.Count == 1
                ? dataSources[0].Fields.Select( field => BuildFieldExplorerNode( dataSources[0].BindingName, field, localizer ) ).ToList()
                : dataSources.Select( dataSource => new ReportTreeNode
                {
                    Key = $"fields:data-source:{dataSource.Name}",
                    Text = dataSource.Name,
                    Kind = ReportTreeNodeKind.DataSource,
                    Selectable = true,
                    Value = new ReportDataSourceTreeNodeValue( dataSource.BindingName ),
                    Children = dataSource.Fields.Select( field => BuildFieldExplorerNode( dataSource.BindingName, field, localizer ) ).ToList(),
                } ).ToList(),
        };
    }

    private static ReportTreeNode BuildFormulaFieldsNode( IReadOnlyList<ReportFormulaFieldDefinition> formulaFields, string selectedFormulaFieldName, TextLocalizerHandler localizer )
    {
        return new()
        {
            Key = FormulaFieldsNodeKey,
            Text = Localize( localizer, "Formula Fields" ),
            Kind = ReportTreeNodeKind.FormulaFields,
            Selectable = true,
            Children = formulaFields
                .Where( field => !string.IsNullOrWhiteSpace( field.Name ) )
                .OrderBy( field => field.Name )
                .Select( field => new ReportTreeNode
                {
                    Key = $"fields:formula:{field.Id}",
                    Text = field.Name,
                    Detail = Localize( localizer, "Formula" ),
                    Kind = ReportTreeNodeKind.FormulaField,
                    Selectable = true,
                    Selected = string.Equals( field.Name, selectedFormulaFieldName, StringComparison.OrdinalIgnoreCase ),
                    Draggable = true,
                    Value = new ReportFieldTreeNodeValue( ReportFormulaFieldResolver.DataSourceName, field.Name ),
                } )
                .ToList(),
        };
    }

    private static ReportTreeNode BuildRunningTotalFieldsNode( IReadOnlyList<ReportRunningTotalDefinition> runningTotals, string selectedRunningTotalName, TextLocalizerHandler localizer )
    {
        return new()
        {
            Key = RunningTotalFieldsNodeKey,
            Text = Localize( localizer, "Running Total Fields" ),
            Kind = ReportTreeNodeKind.RunningTotalFields,
            Selectable = true,
            Children = runningTotals
                .Where( field => !string.IsNullOrWhiteSpace( field.Name ) )
                .OrderBy( field => field.Name )
                .Select( field => new ReportTreeNode
                {
                    Key = $"fields:running-total:{field.Id}",
                    Text = field.Name,
                    Detail = Localize( localizer, "Running total" ),
                    Kind = ReportTreeNodeKind.RunningTotalField,
                    Selectable = true,
                    Selected = string.Equals( field.Name, selectedRunningTotalName, StringComparison.OrdinalIgnoreCase ),
                    Draggable = true,
                    Value = new ReportFieldTreeNodeValue( ReportRunningTotalResolver.DataSourceName, field.Name ),
                } )
                .ToList(),
        };
    }

    private static ReportTreeNode BuildSpecialFieldsNode( TextLocalizerHandler localizer )
    {
        return new()
        {
            Key = "fields:special",
            Text = Localize( localizer, "Special Fields" ),
            Kind = ReportTreeNodeKind.SpecialFields,
            Children = ReportSpecialFieldResolver.GetFields().Select( field => new ReportTreeNode
            {
                Key = $"fields:special:{field.Name}",
                Text = Localize( localizer, field.DisplayName ),
                Detail = Localize( localizer, ReportDefinitionHelper.GetDataTypeDisplayName( field.DataType ) ),
                Kind = ReportTreeNodeKind.Field,
                Selectable = true,
                Draggable = true,
                Value = new ReportFieldTreeNodeValue( ReportSpecialFieldResolver.DataSourceName, field.Name ),
            } ).ToList(),
        };
    }

    private static string Localize( TextLocalizerHandler localizer, string name, params object[] arguments )
        => localizer?.Invoke( name, arguments )
            ?? ( arguments.Length > 0 ? string.Format( CultureInfo.CurrentCulture, name, arguments ) : name );

    #endregion
}