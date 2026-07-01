#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal static class ReportDefinitionHelper
{
    #region Members

    private const double DefaultToolboxElementHeight = 18;

    private const double DefaultToolboxElementWidth = 120;

    private const double ToolboxImageElementHeight = 72;

    private const double ToolboxLineElementHeight = 8;

    private const double ToolboxRectangleElementHeight = 48;

    private const int DefaultTableColumnCount = 2;

    private const int DefaultTableRowCount = 2;

    internal const double DefaultTableColumnWidth = 90;

    internal const double DefaultTableRowHeight = 24;

    private const double ToolboxTableElementWidth = 180;

    private const double ToolboxTableElementHeight = 48;

    #endregion

    #region Methods

    internal static ReportElementDefinition CreateElementFromToolbox( ReportElementType elementType, string text, double x, double y )
    {
        var definition = new ReportElementDefinition
        {
            Name = elementType.ToString(),
            Type = elementType,
            X = x,
            Y = y,
            Width = DefaultToolboxElementWidth,
            Height = DefaultToolboxElementHeight,
        };

        switch ( elementType )
        {
            case ReportElementType.Text:
                definition.Name = text;
                definition.Text = text;
                break;
            case ReportElementType.Image:
                definition.Height = ToolboxImageElementHeight;
                definition.Text = "Image";
                break;
            case ReportElementType.Line:
                definition.Height = ToolboxLineElementHeight;
                break;
            case ReportElementType.Rectangle:
                definition.Height = ToolboxRectangleElementHeight;
                break;
            case ReportElementType.Table:
                definition.Width = ToolboxTableElementWidth;
                definition.Height = ToolboxTableElementHeight;
                EnsureTableLayout( definition );
                break;
        }

        return definition;
    }

    internal static void EnsureTableLayout( ReportElementDefinition element )
    {
        EnsureTableLayout( element, DefaultTableRowCount, DefaultTableColumnCount );
    }

    internal static void EnsureTableLayout( ReportElementDefinition element, int rowCount, int columnCount )
    {
        if ( element is null )
            return;

        rowCount = Math.Max( 1, rowCount );
        columnCount = Math.Max( 1, columnCount );

        element.Columns ??= [];
        element.Rows ??= [];
        element.Cells ??= [];

        while ( element.Columns.Count < columnCount )
        {
            element.Columns.Add( new()
            {
                Width = DefaultTableColumnWidth,
            } );
        }

        if ( element.Columns.Count > columnCount )
            element.Columns.RemoveRange( columnCount, element.Columns.Count - columnCount );

        while ( element.Rows.Count < rowCount )
        {
            element.Rows.Add( new()
            {
                Height = DefaultTableRowHeight,
            } );
        }

        if ( element.Rows.Count > rowCount )
            element.Rows.RemoveRange( rowCount, element.Rows.Count - rowCount );

        element.Cells.RemoveAll( cell => cell.RowIndex >= rowCount || cell.ColumnIndex >= columnCount );

        foreach ( ReportTableCellDefinition cell in element.Cells )
        {
            cell.RowSpan = Math.Clamp( Math.Max( 1, cell.RowSpan ), 1, rowCount - cell.RowIndex );
            cell.ColumnSpan = Math.Clamp( Math.Max( 1, cell.ColumnSpan ), 1, columnCount - cell.ColumnIndex );
        }

        for ( int rowIndex = 0; rowIndex < rowCount; rowIndex++ )
        {
            for ( int columnIndex = 0; columnIndex < columnCount; columnIndex++ )
            {
                if ( element.Cells.Any( cell => CoversTablePosition( cell, rowIndex, columnIndex ) ) )
                    continue;

                element.Cells.Add( new()
                {
                    RowIndex = rowIndex,
                    ColumnIndex = columnIndex,
                } );
            }
        }

        element.Width = Math.Max( element.Width, element.Columns.Sum( column => Math.Max( 1, column.Width ) ) );
        element.Height = Math.Max( element.Height, element.Rows.Sum( row => Math.Max( 1, row.Height ) ) );
    }

    internal static void FitElementToTableCell( ReportElementDefinition table, ReportTableCellDefinition cell, ReportElementDefinition element )
    {
        if ( table is null || cell is null || element is null )
            return;

        element.X = 0;
        element.Y = 0;
        element.Width = Math.Max( 1, GetTableCellWidth( table, cell ) );
        element.Height = Math.Max( 1, GetTableCellHeight( table, cell ) );
    }

    internal static void FitElementsToTableCell( ReportElementDefinition table, ReportTableCellDefinition cell )
    {
        if ( cell?.Elements is null )
            return;

        foreach ( ReportElementDefinition element in cell.Elements )
        {
            FitElementToTableCell( table, cell, element );
        }
    }

    internal static void ScaleTableLayout( ReportElementDefinition table, double originalWidth, double originalHeight )
    {
        if ( table?.Type != ReportElementType.Table )
            return;

        if ( table.Columns?.Count > 0 )
        {
            double widthScale = ResolveTableScale( originalWidth, table.Width );

            if ( Math.Abs( widthScale - 1 ) > .0001 )
            {
                foreach ( ReportTableColumnDefinition column in table.Columns )
                {
                    column.Width = Math.Max( 1, column.Width * widthScale );
                }
            }
        }

        if ( table.Rows?.Count > 0 )
        {
            double heightScale = ResolveTableScale( originalHeight, table.Height );

            if ( Math.Abs( heightScale - 1 ) > .0001 )
            {
                foreach ( ReportTableRowDefinition row in table.Rows )
                {
                    row.Height = Math.Max( 1, row.Height * heightScale );
                }
            }
        }

        foreach ( ReportTableCellDefinition cell in table.Cells ?? Enumerable.Empty<ReportTableCellDefinition>() )
        {
            FitElementsToTableCell( table, cell );
        }
    }

    private static double ResolveTableScale( double originalSize, double targetSize )
    {
        if ( originalSize <= 0 || targetSize <= 0 )
            return 1;

        return targetSize / originalSize;
    }

    internal static double GetTableCellWidth( ReportElementDefinition table, ReportTableCellDefinition cell )
    {
        if ( table?.Columns is null || cell is null )
            return 0;

        int availableColumns = Math.Max( 0, table.Columns.Count - cell.ColumnIndex );

        if ( availableColumns == 0 )
            return 0;

        return table.Columns
            .Skip( cell.ColumnIndex )
            .Take( Math.Clamp( cell.ColumnSpan, 1, availableColumns ) )
            .Sum( column => Math.Max( 1, column.Width ) );
    }

    internal static double GetTableCellHeight( ReportElementDefinition table, ReportTableCellDefinition cell )
    {
        if ( table?.Rows is null || cell is null )
            return 0;

        int availableRows = Math.Max( 0, table.Rows.Count - cell.RowIndex );

        if ( availableRows == 0 )
            return 0;

        return table.Rows
            .Skip( cell.RowIndex )
            .Take( Math.Clamp( cell.RowSpan, 1, availableRows ) )
            .Sum( row => Math.Max( 1, row.Height ) );
    }

    private static bool CoversTablePosition( ReportTableCellDefinition cell, int rowIndex, int columnIndex )
    {
        if ( cell is null )
            return false;

        int rowSpan = Math.Max( 1, cell.RowSpan );
        int columnSpan = Math.Max( 1, cell.ColumnSpan );

        return rowIndex >= cell.RowIndex
            && rowIndex < cell.RowIndex + rowSpan
            && columnIndex >= cell.ColumnIndex
            && columnIndex < cell.ColumnIndex + columnSpan;
    }

    internal static (string DataSourceName, string FieldName) NormalizeFieldBindingForSection( ReportDefinition definition, ReportSectionDefinition section, string dataSourceName, string fieldName )
    {
        if ( string.IsNullOrWhiteSpace( fieldName ) )
            return (dataSourceName, fieldName);

        if ( ReportSpecialFieldResolver.IsSpecialDataSource( dataSourceName )
            || ReportFormulaFieldResolver.IsFormulaDataSource( dataSourceName )
            || ReportRunningTotalResolver.IsRunningTotalDataSource( dataSourceName ) )
            return (dataSourceName, fieldName);

        string fieldPath = ReportExpressionFormatter.FormatFieldPath( dataSourceName, fieldName );

        if ( section is not null && !string.IsNullOrWhiteSpace( section.DataSource ) )
        {
            string sectionDataSource = section.DataSource.Trim();
            string sectionPrefix = $"{sectionDataSource}.";

            if ( fieldPath.StartsWith( sectionPrefix, StringComparison.OrdinalIgnoreCase ) )
                return (null, fieldPath[sectionPrefix.Length..]);

            if ( fieldName.StartsWith( sectionPrefix, StringComparison.OrdinalIgnoreCase ) )
                return (null, fieldName[sectionPrefix.Length..]);

            if ( string.Equals( dataSourceName, sectionDataSource, StringComparison.OrdinalIgnoreCase ) )
                return (null, fieldName);
        }

        string rootDataSourceName = definition?.DataSources?.FirstOrDefault()?.Name;

        if ( !string.IsNullOrWhiteSpace( rootDataSourceName ) && string.Equals( dataSourceName, rootDataSourceName, StringComparison.OrdinalIgnoreCase ) )
            return (null, fieldName);

        return (dataSourceName, fieldName);
    }

    internal static string CreateUniqueSectionName( ReportDefinition definition, string baseName )
    {
        if ( string.IsNullOrWhiteSpace( baseName ) )
            baseName = "Band copy";

        if ( definition is null )
            return baseName;

        var name = baseName;
        var index = 2;

        while ( definition.Sections.Any( section => string.Equals( section.Name, name, StringComparison.OrdinalIgnoreCase ) ) )
        {
            name = $"{baseName} {index}";
            index++;
        }

        return name;
    }

    internal static string GetSectionDisplayName( ReportSectionDefinition section )
    {
        return string.IsNullOrWhiteSpace( section.Name ) ? GetSectionTypeDisplayName( section.Type ) : section.Name;
    }

    internal static string GetSectionTypeDisplayName( ReportSectionType type )
    {
        return type switch
        {
            ReportSectionType.Header => "Report Header",
            ReportSectionType.PageHeader => "Page Header",
            ReportSectionType.Detail => "Detail",
            ReportSectionType.Group => "Group Header",
            ReportSectionType.GroupHeader => "Group Header",
            ReportSectionType.GroupFooter => "Group Footer",
            ReportSectionType.PageFooter => "Page Footer",
            ReportSectionType.Footer => "Report Footer",
            _ => type.ToString(),
        };
    }

    internal static ReportTreeNodeKind GetElementTreeNodeKind( ReportElementType type )
    {
        return type switch
        {
            ReportElementType.Field => ReportTreeNodeKind.Field,
            ReportElementType.Table => ReportTreeNodeKind.Table,
            ReportElementType.Image => ReportTreeNodeKind.Image,
            ReportElementType.Line => ReportTreeNodeKind.Line,
            ReportElementType.Rectangle => ReportTreeNodeKind.Rectangle,
            ReportElementType.PageBreak => ReportTreeNodeKind.PageBreak,
            _ => ReportTreeNodeKind.Text,
        };
    }

    internal static string GetDataTypeDisplayName( Type dataType )
    {
        if ( dataType is null )
            return null;

        var nullableType = Nullable.GetUnderlyingType( dataType );

        return nullableType is null
            ? dataType.Name
            : $"{nullableType.Name}?";
    }

    internal static bool CanDeleteSection( ReportSectionDefinition section )
    {
        return section is not null && !section.Default;
    }

    internal static ReportDefinition EnsureDefinitionIds( ReportDefinition definition )
    {
        if ( definition is null )
            return null;

        var definitionIds = new HashSet<string>( StringComparer.Ordinal );
        var dataSourceIds = new HashSet<string>( StringComparer.Ordinal );
        var formulaFieldIds = new HashSet<string>( StringComparer.Ordinal );
        var runningTotalIds = new HashSet<string>( StringComparer.Ordinal );
        var sectionIds = new HashSet<string>( StringComparer.Ordinal );
        var elementIds = new HashSet<string>( StringComparer.Ordinal );
        var columnIds = new HashSet<string>( StringComparer.Ordinal );
        var rowIds = new HashSet<string>( StringComparer.Ordinal );
        var cellIds = new HashSet<string>( StringComparer.Ordinal );

        definition.Id = EnsureUniqueDefinitionId( definition.Id, definitionIds );

        foreach ( var dataSource in definition.DataSources )
        {
            dataSource.Id = EnsureUniqueDefinitionId( dataSource.Id, dataSourceIds );
        }

        foreach ( var formulaField in definition.FormulaFields ?? [] )
        {
            formulaField.Id = EnsureUniqueDefinitionId( formulaField.Id, formulaFieldIds );
        }

        foreach ( var runningTotal in definition.RunningTotals ?? [] )
        {
            runningTotal.Id = EnsureUniqueDefinitionId( runningTotal.Id, runningTotalIds );
        }

        foreach ( var section in definition.Sections )
        {
            section.Id = EnsureUniqueDefinitionId( section.Id, sectionIds );

            foreach ( var element in section.Elements )
            {
                EnsureElementIds( element, elementIds, columnIds, rowIds, cellIds );
            }
        }

        return definition;
    }

    internal static string EnsureElementId( ReportElementDefinition element )
    {
        if ( element is null )
            return null;

        if ( string.IsNullOrWhiteSpace( element.Id ) )
            element.Id = CreateDefinitionId();

        return element.Id;
    }

    internal static string EnsureSectionId( ReportSectionDefinition section )
    {
        if ( section is null )
            return null;

        if ( string.IsNullOrWhiteSpace( section.Id ) )
            section.Id = CreateDefinitionId();

        return section.Id;
    }

    internal static string EnsureTableCellId( ReportTableCellDefinition cell )
    {
        if ( cell is null )
            return null;

        if ( string.IsNullOrWhiteSpace( cell.Id ) )
            cell.Id = CreateDefinitionId();

        return cell.Id;
    }

    internal static bool TryFindElementLocation( ReportDefinition definition, string key, out int sectionIndex, out int elementIndex, out ReportElementDefinition element )
    {
        sectionIndex = -1;
        elementIndex = -1;
        element = null;

        if ( !TryFindElementLocation( definition, key, out ReportElementLocation location ) )
            return false;

        sectionIndex = location.SectionIndex;
        elementIndex = location.ElementIndex;
        element = location.Element;

        return true;
    }

    internal static bool TryFindElementLocation( ReportDefinition definition, string key, out ReportElementLocation location )
    {
        location = null;

        if ( definition is null || string.IsNullOrWhiteSpace( key ) )
            return false;

        for ( int sectionIndex = 0; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            ReportSectionDefinition section = definition.Sections[sectionIndex];

            if ( TryFindElementLocation( section.Elements, key, sectionIndex, null, null, out location ) )
                return true;
        }

        return false;
    }

    internal static int RemoveElementsByIds( ReportDefinition definition, ISet<string> elementIds )
    {
        if ( definition is null || elementIds is null || elementIds.Count == 0 )
            return -1;

        int lastSectionIndex = -1;

        for ( int sectionIndex = 0; sectionIndex < definition.Sections.Count; sectionIndex++ )
        {
            ReportSectionDefinition section = definition.Sections[sectionIndex];

            if ( RemoveElementsByIds( section.Elements, elementIds ) )
                lastSectionIndex = sectionIndex;
        }

        return lastSectionIndex;
    }

    private static bool TryFindElementLocation(
        IList<ReportElementDefinition> elements,
        string key,
        int sectionIndex,
        ReportElementDefinition parentTable,
        ReportTableCellDefinition parentCell,
        out ReportElementLocation location )
    {
        location = null;

        if ( elements is null )
            return false;

        for ( int elementIndex = 0; elementIndex < elements.Count; elementIndex++ )
        {
            ReportElementDefinition element = elements[elementIndex];

            if ( EnsureElementId( element ) == key )
            {
                location = new()
                {
                    SectionIndex = sectionIndex,
                    ElementIndex = elementIndex,
                    Element = element,
                    OwnerElements = elements,
                    ParentTable = parentTable,
                    ParentCell = parentCell,
                };

                return true;
            }

            if ( element.Type != ReportElementType.Table || element.Cells is null )
                continue;

            foreach ( ReportTableCellDefinition cell in element.Cells )
            {
                if ( TryFindElementLocation( cell.Elements, key, sectionIndex, element, cell, out location ) )
                    return true;
            }
        }

        return false;
    }

    private static bool RemoveElementsByIds( IList<ReportElementDefinition> elements, ISet<string> elementIds )
    {
        if ( elements is null )
            return false;

        bool removed = false;

        for ( int elementIndex = elements.Count - 1; elementIndex >= 0; elementIndex-- )
        {
            ReportElementDefinition element = elements[elementIndex];

            if ( elementIds.Contains( EnsureElementId( element ) ) )
            {
                elements.RemoveAt( elementIndex );
                removed = true;
                continue;
            }

            if ( element.Type != ReportElementType.Table || element.Cells is null )
                continue;

            foreach ( ReportTableCellDefinition cell in element.Cells )
            {
                removed = RemoveElementsByIds( cell.Elements, elementIds ) || removed;
            }
        }

        return removed;
    }

    internal static bool TryFindTableCellLocation(
        ReportDefinition definition,
        string cellKey,
        out int sectionIndex,
        out int tableIndex,
        out ReportElementDefinition table,
        out ReportTableCellDefinition cell )
    {
        sectionIndex = -1;
        tableIndex = -1;
        table = null;
        cell = null;

        if ( definition is null || string.IsNullOrWhiteSpace( cellKey ) )
            return false;

        for ( int currentSectionIndex = 0; currentSectionIndex < definition.Sections.Count; currentSectionIndex++ )
        {
            ReportSectionDefinition section = definition.Sections[currentSectionIndex];

            for ( int currentElementIndex = 0; currentElementIndex < section.Elements.Count; currentElementIndex++ )
            {
                ReportElementDefinition element = section.Elements[currentElementIndex];

                if ( element.Type != ReportElementType.Table || element.Cells is null )
                    continue;

                ReportTableCellDefinition foundCell = element.Cells.FirstOrDefault( item => item.Id == cellKey );

                if ( foundCell is null )
                    continue;

                sectionIndex = currentSectionIndex;
                tableIndex = currentElementIndex;
                table = element;
                cell = foundCell;

                return true;
            }
        }

        return false;
    }

    internal static string CreateDefinitionId()
    {
        return Guid.NewGuid().ToString( "N" );
    }

    private static string EnsureUniqueDefinitionId( string id, HashSet<string> usedIds )
    {
        if ( string.IsNullOrWhiteSpace( id ) || !usedIds.Add( id ) )
        {
            do
            {
                id = CreateDefinitionId();
            }
            while ( !usedIds.Add( id ) );
        }

        return id;
    }

    private static void EnsureElementIds(
        ReportElementDefinition element,
        HashSet<string> elementIds,
        HashSet<string> columnIds,
        HashSet<string> rowIds,
        HashSet<string> cellIds )
    {
        if ( element is null )
            return;

        element.Id = EnsureUniqueDefinitionId( element.Id, elementIds );

        foreach ( ReportTableColumnDefinition column in element.Columns ?? Enumerable.Empty<ReportTableColumnDefinition>() )
        {
            column.Id = EnsureUniqueDefinitionId( column.Id, columnIds );
        }

        foreach ( ReportTableRowDefinition row in element.Rows ?? Enumerable.Empty<ReportTableRowDefinition>() )
        {
            row.Id = EnsureUniqueDefinitionId( row.Id, rowIds );
        }

        foreach ( ReportTableCellDefinition cell in element.Cells ?? Enumerable.Empty<ReportTableCellDefinition>() )
        {
            cell.Id = EnsureUniqueDefinitionId( cell.Id, cellIds );
            FitElementsToTableCell( element, cell );

            foreach ( ReportElementDefinition childElement in cell.Elements ?? Enumerable.Empty<ReportElementDefinition>() )
            {
                EnsureElementIds( childElement, elementIds, columnIds, rowIds, cellIds );
            }
        }
    }

    #endregion
}