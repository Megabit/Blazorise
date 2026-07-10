#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDesignerTreeBuilder
{
    #region Members

    internal const string FormulaFieldsNodeKey = "fields:formula";

    internal const string RunningTotalFieldsNodeKey = "fields:running-total";

    #endregion

    #region Methods

    internal static IReadOnlyList<ReportTreeNode> BuildToolboxNodes( bool allowSubreport = true )
    {
        List<ReportTreeNode> reportItems =
        [
            CreateToolboxNode( "toolbox:text", "Text", ReportElementType.Text, "Text" ),
            CreateToolboxNode( "toolbox:image", "Image", ReportElementType.Image, null ),
            CreateToolboxNode( "toolbox:line", "Line", ReportElementType.Line, null ),
            CreateToolboxNode( "toolbox:rectangle", "Rectangle", ReportElementType.Rectangle, null ),
            CreateToolboxNode( "toolbox:table", "Table", ReportElementType.Table, null ),
        ];

        if ( allowSubreport )
            reportItems.Add( CreateToolboxNode( "toolbox:subreport", "Subreport", ReportElementType.Subreport, null ) );

        return
        [
            new()
            {
                Key = "toolbox",
                Text = "Report Items",
                Kind = ReportTreeNodeKind.Folder,
                Children = reportItems,
            }
        ];
    }

    internal static IReadOnlyList<ReportTreeNode> BuildFieldsExplorerNodes(
        IEnumerable<ReportDesignerDataSourceNode> dataSources,
        IEnumerable<ReportFormulaFieldDefinition> formulaFields,
        IEnumerable<ReportRunningTotalDefinition> runningTotals = null,
        string selectedFormulaFieldName = null,
        string selectedRunningTotalName = null )
    {
        List<ReportDesignerDataSourceNode> dataSourceList = dataSources?.ToList() ?? [];
        List<ReportFormulaFieldDefinition> formulaFieldList = formulaFields?.ToList() ?? [];
        List<ReportRunningTotalDefinition> runningTotalList = runningTotals?.ToList() ?? [];

        return
        [
            BuildSourceFieldsNode( dataSourceList ),
            BuildFormulaFieldsNode( formulaFieldList, selectedFormulaFieldName ),
            BuildRunningTotalFieldsNode( runningTotalList, selectedRunningTotalName ),
            BuildSpecialFieldsNode(),
        ];
    }

    internal static IReadOnlyList<ReportTreeNode> BuildReportExplorerNodes(
        ReportDefinition definition,
        bool reportSelected,
        int? selectedSectionIndex,
        string selectedElementKey,
        string selectedCellKey,
        Func<string, bool> isElementSelected,
        bool allowSubreport = true )
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
                Children = definition.Bands.Select( ( section, sectionIndex ) => new ReportTreeNode
                {
                    Key = CreateSectionTreeNodeKey( sectionIndex ),
                    Text = ReportDefinitionHelper.GetSectionDisplayName( section ),
                    Detail = ReportDefinitionHelper.GetSectionTypeDisplayName( section.Type ),
                    Kind = ReportTreeNodeKind.Band,
                    Selectable = true,
                    Selected = selectedSectionIndex == sectionIndex && string.IsNullOrWhiteSpace( selectedElementKey ),
                    Children = section.Elements
                        .Where( element => allowSubreport || element.Type != ReportElementType.Subreport )
                        .Select( element => BuildReportElementNode( element, selectedCellKey, isElementSelected, allowSubreport ) )
                        .ToList(),
                } ).ToList(),
            }
        ];
    }

    internal static string CreateSectionTreeNodeKey( int sectionIndex )
        => $"report:section:{sectionIndex}";

    internal static string CreateElementTreeNodeKey( string elementKey )
        => $"report:element:{elementKey}";

    internal static string CreateTableRowTreeNodeKey( string tableKey, int rowIndex )
        => $"report:table-row:{tableKey}:{rowIndex.ToString( CultureInfo.InvariantCulture )}";

    internal static string CreateTableCellTreeNodeKey( string cellKey )
        => $"report:table-cell:{cellKey}";

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

    internal static bool TryResolveTableCellTreeNode( ReportTreeNode node, out string cellKey )
    {
        cellKey = null;

        if ( node?.Key is null || !node.Key.StartsWith( "report:table-cell:", StringComparison.Ordinal ) )
            return false;

        cellKey = node.Key["report:table-cell:".Length..];

        return !string.IsNullOrWhiteSpace( cellKey );
    }

    private static ReportTreeNode BuildReportElementNode( ReportElementDefinition element, string selectedCellKey, Func<string, bool> isElementSelected, bool allowSubreport = true )
    {
        var elementKey = ReportDefinitionHelper.EnsureElementId( element );

        return new()
        {
            Key = CreateElementTreeNodeKey( elementKey ),
            Text = element.Name ?? ReportElementDefinitionHelper.GetDisplayText( element ),
            Detail = element.Type.ToString(),
            Kind = ReportDefinitionHelper.GetElementTreeNodeKind( element.Type ),
            Selectable = true,
            Selected = isElementSelected?.Invoke( elementKey ) == true,
            Children = element is ReportTableElementDefinition table
                ? BuildTableChildNodes( table, elementKey, selectedCellKey, isElementSelected, allowSubreport )
                : [],
        };
    }

    private static List<ReportTreeNode> BuildTableChildNodes( ReportTableElementDefinition table, string tableKey, string selectedCellKey, Func<string, bool> isElementSelected, bool allowSubreport )
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
                Text = $"Row {( rowIndex + 1 ).ToString( CultureInfo.InvariantCulture )}",
                Detail = "Row",
                Kind = ReportTreeNodeKind.TableRow,
                Children = BuildTableCellNodes( table, rowIndex, selectedCellKey, isElementSelected, allowSubreport ),
            } );
        }

        return rows;
    }

    private static List<ReportTreeNode> BuildTableCellNodes( ReportTableElementDefinition table, int rowIndex, string selectedCellKey, Func<string, bool> isElementSelected, bool allowSubreport )
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
                    Text = $"Cell {( cell.ColumnIndex + 1 ).ToString( CultureInfo.InvariantCulture )}",
                    Detail = GetCellDetail( cell ),
                    Kind = ReportTreeNodeKind.TableCell,
                    Selectable = true,
                    Selected = string.Equals( selectedCellKey, cellKey, StringComparison.Ordinal ),
                    Children = ( cell.Elements ?? [] )
                        .Where( element => allowSubreport || element.Type != ReportElementType.Subreport )
                        .Select( element => BuildReportElementNode( element, selectedCellKey, isElementSelected, allowSubreport ) )
                        .ToList(),
                };
            } )
            .ToList();
    }

    private static string GetCellDetail( ReportTableCellDefinition cell )
    {
        int rowSpan = Math.Max( 1, cell.RowSpan );
        int columnSpan = Math.Max( 1, cell.ColumnSpan );

        return rowSpan == 1 && columnSpan == 1
            ? "Cell"
            : $"Span {columnSpan.ToString( CultureInfo.InvariantCulture )}x{rowSpan.ToString( CultureInfo.InvariantCulture )}";
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
        bool hasChildren = field.Children.Count > 0;

        return new()
        {
            Key = $"fields:field:{dataSourceName}:{field.Path}",
            Text = field.Name,
            Detail = hasChildren ? null : ReportDefinitionHelper.GetDataTypeDisplayName( field.DataType ),
            Kind = hasChildren ? ReportTreeNodeKind.Folder : ReportTreeNodeKind.Field,
            Selectable = !hasChildren,
            Draggable = !hasChildren,
            Value = !hasChildren ? new ReportFieldTreeNodeValue( dataSourceName, field.Path ) : null,
            Children = field.Children.Select( child => BuildFieldExplorerNode( dataSourceName, child ) ).ToList(),
        };
    }

    private static ReportTreeNode BuildSourceFieldsNode( IReadOnlyList<ReportDesignerDataSourceNode> dataSources )
    {
        ReportDesignerDataSourceNode singleDataSource = dataSources.Count == 1 ? dataSources[0] : null;

        return new()
        {
            Key = "fields:source",
            Text = "Source Fields",
            Kind = ReportTreeNodeKind.SourceFields,
            Selectable = singleDataSource is not null,
            Value = singleDataSource is not null ? new ReportDataSourceTreeNodeValue( singleDataSource.BindingName ) : null,
            Children = dataSources.Count == 1
                ? dataSources[0].Fields.Select( field => BuildFieldExplorerNode( dataSources[0].BindingName, field ) ).ToList()
                : dataSources.Select( dataSource => new ReportTreeNode
                {
                    Key = $"fields:data-source:{dataSource.Name}",
                    Text = dataSource.Name,
                    Kind = ReportTreeNodeKind.DataSource,
                    Selectable = true,
                    Value = new ReportDataSourceTreeNodeValue( dataSource.BindingName ),
                    Children = dataSource.Fields.Select( field => BuildFieldExplorerNode( dataSource.BindingName, field ) ).ToList(),
                } ).ToList(),
        };
    }

    private static ReportTreeNode BuildFormulaFieldsNode( IReadOnlyList<ReportFormulaFieldDefinition> formulaFields, string selectedFormulaFieldName )
    {
        return new()
        {
            Key = FormulaFieldsNodeKey,
            Text = "Formula Fields",
            Kind = ReportTreeNodeKind.FormulaFields,
            Selectable = true,
            Children = formulaFields
                .Where( field => !string.IsNullOrWhiteSpace( field.Name ) )
                .OrderBy( field => field.Name )
                .Select( field => new ReportTreeNode
                {
                    Key = $"fields:formula:{field.Id}",
                    Text = field.Name,
                    Detail = "Formula",
                    Kind = ReportTreeNodeKind.FormulaField,
                    Selectable = true,
                    Selected = string.Equals( field.Name, selectedFormulaFieldName, StringComparison.OrdinalIgnoreCase ),
                    Draggable = true,
                    Value = new ReportFieldTreeNodeValue( ReportFormulaFieldResolver.DataSourceName, field.Name ),
                } )
                .ToList(),
        };
    }

    private static ReportTreeNode BuildRunningTotalFieldsNode( IReadOnlyList<ReportRunningTotalDefinition> runningTotals, string selectedRunningTotalName )
    {
        return new()
        {
            Key = RunningTotalFieldsNodeKey,
            Text = "Running Total Fields",
            Kind = ReportTreeNodeKind.RunningTotalFields,
            Selectable = true,
            Children = runningTotals
                .Where( field => !string.IsNullOrWhiteSpace( field.Name ) )
                .OrderBy( field => field.Name )
                .Select( field => new ReportTreeNode
                {
                    Key = $"fields:running-total:{field.Id}",
                    Text = field.Name,
                    Detail = "Running total",
                    Kind = ReportTreeNodeKind.RunningTotalField,
                    Selectable = true,
                    Selected = string.Equals( field.Name, selectedRunningTotalName, StringComparison.OrdinalIgnoreCase ),
                    Draggable = true,
                    Value = new ReportFieldTreeNodeValue( ReportRunningTotalResolver.DataSourceName, field.Name ),
                } )
                .ToList(),
        };
    }

    private static ReportTreeNode BuildSpecialFieldsNode()
    {
        return new()
        {
            Key = "fields:special",
            Text = "Special Fields",
            Kind = ReportTreeNodeKind.SpecialFields,
            Children = ReportSpecialFieldResolver.GetFields().Select( field => new ReportTreeNode
            {
                Key = $"fields:special:{field.Name}",
                Text = field.DisplayName,
                Detail = ReportDefinitionHelper.GetDataTypeDisplayName( field.DataType ),
                Kind = ReportTreeNodeKind.Field,
                Selectable = true,
                Draggable = true,
                Value = new ReportFieldTreeNodeValue( ReportSpecialFieldResolver.DataSourceName, field.Name ),
            } ).ToList(),
        };
    }

    #endregion
}