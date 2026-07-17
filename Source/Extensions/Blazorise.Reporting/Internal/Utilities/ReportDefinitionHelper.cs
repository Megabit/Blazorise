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

    private const double ToolboxPanelElementWidth = 180;

    private const double ToolboxPanelElementHeight = 90;

    private const int DefaultTableColumnCount = 2;

    private const int DefaultTableRowCount = 2;

    internal const double DefaultTableColumnWidth = 90;

    internal const double DefaultTableRowHeight = 24;

    private const double ToolboxTableElementWidth = 180;

    private const double ToolboxTableElementHeight = 48;

    private const double ToolboxSubreportElementWidth = 240;

    private const double ToolboxSubreportElementHeight = 120;

    #endregion

    #region Methods

    internal static ReportElementDefinition CreateElementFromToolbox( ReportElementType elementType, string text, double x, double y )
    {
        ReportElementDefinition definition = ReportElementDefinitionFactory.Create( elementType );

        definition.Name = elementType.ToString();
        definition.X = x;
        definition.Y = y;
        definition.Width = DefaultToolboxElementWidth;
        definition.Height = DefaultToolboxElementHeight;

        switch ( elementType )
        {
            case ReportElementType.Text:
                definition.Name = text;
                ( (ReportTextElementDefinition)definition ).Text = text;
                break;
            case ReportElementType.Image:
                definition.Height = ToolboxImageElementHeight;
                ( (ReportImageElementDefinition)definition ).Text = "Image";
                break;
            case ReportElementType.Line:
                definition.Height = ToolboxLineElementHeight;
                break;
            case ReportElementType.Rectangle:
                definition.Height = ToolboxRectangleElementHeight;
                break;
            case ReportElementType.Panel:
                definition.Width = ToolboxPanelElementWidth;
                definition.Height = ToolboxPanelElementHeight;
                break;
            case ReportElementType.Table:
                definition.Width = ToolboxTableElementWidth;
                definition.Height = ToolboxTableElementHeight;
                EnsureTableLayout( definition );
                break;
            case ReportElementType.Subreport:
                definition.Width = ToolboxSubreportElementWidth;
                definition.Height = ToolboxSubreportElementHeight;
                definition.Name = string.IsNullOrWhiteSpace( text ) ? "Subreport" : text;
                ( (ReportSubreportElementDefinition)definition ).Report = CreateDefaultSubreportDefinition( definition.Name );
                break;
        }

        return definition;
    }

    internal static ReportDefinition CreateDefaultSubreportDefinition( string name )
    {
        return new()
        {
            Name = string.IsNullOrWhiteSpace( name ) ? "Subreport" : name,
            Bands =
            [
                new()
                {
                    Name = "Report Header",
                    Type = ReportBandType.ReportHeader,
                    Height = 60,
                },
                new()
                {
                    Name = "Detail",
                    Type = ReportBandType.Detail,
                    Height = 120,
                    Default = true,
                },
                new()
                {
                    Name = "Report Footer",
                    Type = ReportBandType.ReportFooter,
                    Height = 60,
                },
            ],
        };
    }

    internal static ReportDefinition EnsureSubreportDefinition( ReportSubreportElementDefinition element )
    {
        if ( element is null )
            return null;

        element.Report ??= CreateDefaultSubreportDefinition( ReportSubreportResolver.GetDisplayName( element ) );

        if ( string.IsNullOrWhiteSpace( element.Report.Name ) )
            element.Report.Name = ReportSubreportResolver.GetDisplayName( element );

        RemoveSubreportElements( element.Report );

        return element.Report;
    }

    internal static void RemoveSubreportElements( ReportDefinition definition )
    {
        if ( definition?.Bands is null )
            return;

        foreach ( ReportBandDefinition section in definition.Bands )
        {
            RemoveSubreportElements( section.Elements );
        }
    }

    private static void RemoveSubreportElements( IList<ReportElementDefinition> elements )
    {
        if ( elements is null )
            return;

        for ( int i = elements.Count - 1; i >= 0; i-- )
        {
            if ( elements[i] is ReportSubreportElementDefinition )
            {
                elements.RemoveAt( i );
                continue;
            }

            foreach ( IList<ReportElementDefinition> childElements in GetChildElementCollections( elements[i] ) )
            {
                RemoveSubreportElements( childElements );
            }
        }
    }

    internal static IEnumerable<ReportSubreportElementDefinition> EnumerateSubreportElements( ReportDefinition definition )
    {
        if ( definition?.Bands is null )
            yield break;

        foreach ( ReportSubreportElementDefinition subreport in EnumerateElements( definition ).OfType<ReportSubreportElementDefinition>() )
            yield return subreport;
    }

    internal static void ApplyRowsLimit( ReportDefinition definition, int? rowsLimit )
    {
        if ( definition is null )
            return;

        definition.RowsLimit = rowsLimit;

        foreach ( ReportSubreportElementDefinition subreport in EnumerateSubreportElements( definition ) )
        {
            if ( subreport.Report is not null )
                subreport.Report.RowsLimit = rowsLimit;
        }
    }

    internal static IEnumerable<ReportElementDefinition> EnumerateElements( ReportDefinition definition )
    {
        if ( definition?.Bands is null )
            yield break;

        foreach ( ReportBandDefinition section in definition.Bands )
        {
            foreach ( ReportElementDefinition element in EnumerateElements( section.Elements ) )
                yield return element;
        }
    }

    internal static IEnumerable<ReportElementDefinition> EnumerateElements( IEnumerable<ReportElementDefinition> elements )
    {
        foreach ( ReportElementDefinition element in elements ?? [] )
        {
            if ( element is null )
                continue;

            yield return element;

            foreach ( IList<ReportElementDefinition> childElements in GetChildElementCollections( element ) )
            {
                foreach ( ReportElementDefinition child in EnumerateElements( childElements ) )
                    yield return child;
            }
        }
    }

    internal static IEnumerable<IList<ReportElementDefinition>> GetChildElementCollections( ReportElementDefinition element )
    {
        if ( element is ReportPanelElementDefinition panel && panel.Elements is not null )
            yield return panel.Elements;

        if ( element is ReportTableElementDefinition table )
        {
            foreach ( ReportTableCellDefinition cell in table.Cells ?? [] )
            {
                if ( cell.Elements is not null )
                    yield return cell.Elements;
            }
        }
    }

    internal static bool ContainsElement( ReportElementDefinition container, ReportElementDefinition element )
    {
        if ( container is null || element is null )
            return false;

        foreach ( IList<ReportElementDefinition> childElements in GetChildElementCollections( container ) )
        {
            foreach ( ReportElementDefinition child in childElements )
            {
                if ( ReferenceEquals( child, element ) || ContainsElement( child, element ) )
                    return true;
            }
        }

        return false;
    }

    internal static void PlaceElementInPanel( ReportPanelElementDefinition panel, ReportElementDefinition element, double x, double y )
    {
        if ( panel is null || element is null )
            return;

        element.Width = Math.Min( element.Width, Math.Max( ReportLayoutGeometry.DefaultMinimumElementSize, panel.Width ) );
        element.Height = Math.Min( element.Height, Math.Max( ReportLayoutGeometry.GetMinimumElementHeight( element ), panel.Height ) );
        element.X = ReportLayoutGeometry.Clamp( x, 0, Math.Max( 0, panel.Width - element.Width ) );
        element.Y = ReportLayoutGeometry.Clamp( y, 0, Math.Max( 0, panel.Height - element.Height ) );
    }

    internal static string CreateUniqueSubreportName( ReportDefinition definition )
    {
        const string baseName = "Subreport";

        if ( definition is null )
            return baseName;

        HashSet<string> names = EnumerateSubreportElements( definition )
            .Select( ReportSubreportResolver.GetDisplayName )
            .Where( name => !string.IsNullOrWhiteSpace( name ) )
            .ToHashSet( StringComparer.OrdinalIgnoreCase );

        if ( names.Add( baseName ) )
            return baseName;

        int index = 2;
        string name;

        do
        {
            name = $"{baseName} {index}";
            index++;
        }
        while ( !names.Add( name ) );

        return name;
    }

    internal static void EnsureTableLayout( ReportElementDefinition element )
    {
        EnsureTableLayout( element, DefaultTableRowCount, DefaultTableColumnCount );
    }

    internal static void EnsureTableLayout( ReportElementDefinition element, int rowCount, int columnCount )
    {
        if ( element is not ReportTableElementDefinition table )
            return;

        rowCount = Math.Max( 1, rowCount );
        columnCount = Math.Max( 1, columnCount );

        table.Columns ??= [];
        table.Rows ??= [];
        table.Cells ??= [];

        while ( table.Columns.Count < columnCount )
        {
            table.Columns.Add( new()
            {
                Width = DefaultTableColumnWidth,
            } );
        }

        if ( table.Columns.Count > columnCount )
            table.Columns.RemoveRange( columnCount, table.Columns.Count - columnCount );

        while ( table.Rows.Count < rowCount )
        {
            table.Rows.Add( new()
            {
                Height = DefaultTableRowHeight,
            } );
        }

        if ( table.Rows.Count > rowCount )
            table.Rows.RemoveRange( rowCount, table.Rows.Count - rowCount );

        table.Cells.RemoveAll( cell => cell.RowIndex >= rowCount || cell.ColumnIndex >= columnCount );

        foreach ( ReportTableCellDefinition cell in table.Cells )
        {
            cell.RowSpan = Math.Clamp( Math.Max( 1, cell.RowSpan ), 1, rowCount - cell.RowIndex );
            cell.ColumnSpan = Math.Clamp( Math.Max( 1, cell.ColumnSpan ), 1, columnCount - cell.ColumnIndex );
        }

        for ( int rowIndex = 0; rowIndex < rowCount; rowIndex++ )
        {
            for ( int columnIndex = 0; columnIndex < columnCount; columnIndex++ )
            {
                if ( table.Cells.Any( cell => CoversTablePosition( cell, rowIndex, columnIndex ) ) )
                    continue;

                table.Cells.Add( new()
                {
                    RowIndex = rowIndex,
                    ColumnIndex = columnIndex,
                } );
            }
        }

        table.Width = Math.Max( table.Width, table.Columns.Sum( column => Math.Max( 1, column.Width ) ) );
        table.Height = Math.Max( table.Height, table.Rows.Sum( row => Math.Max( 1, row.Height ) ) );
    }

    internal static void FitElementToTableCell( ReportTableElementDefinition table, ReportTableCellDefinition cell, ReportElementDefinition element )
    {
        if ( table is null || cell is null || element is null )
            return;

        element.X = 0;
        element.Y = 0;
        element.Width = Math.Max( 1, GetTableCellWidth( table, cell ) );
        element.Height = Math.Max( 1, GetTableCellHeight( table, cell ) );
    }

    internal static void FitElementsToTableCell( ReportTableElementDefinition table, ReportTableCellDefinition cell )
    {
        if ( cell?.Elements is null )
            return;

        foreach ( ReportElementDefinition element in cell.Elements )
        {
            FitElementToTableCell( table, cell, element );
        }
    }

    internal static void ScaleTableLayout( ReportTableElementDefinition table, double originalWidth, double originalHeight )
    {
        if ( table is null )
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

    internal static double GetTableCellWidth( ReportTableElementDefinition table, ReportTableCellDefinition cell )
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

    internal static double GetTableCellHeight( ReportTableElementDefinition table, ReportTableCellDefinition cell )
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

    internal static (string DataSourceName, string FieldName) NormalizeFieldBindingForSection( ReportDefinition definition, ReportBandDefinition section, string dataSourceName, string fieldName )
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

        while ( definition.Bands.Any( section => string.Equals( section.Name, name, StringComparison.OrdinalIgnoreCase ) ) )
        {
            name = $"{baseName} {index}";
            index++;
        }

        return name;
    }

    internal static string GetSectionDisplayName( ReportBandDefinition section )
    {
        return string.IsNullOrWhiteSpace( section.Name ) ? GetSectionTypeDisplayName( section.Type ) : section.Name;
    }

    internal static string GetSectionTypeDisplayName( ReportBandType type )
    {
        return type switch
        {
            ReportBandType.ReportHeader => "Report Header",
            ReportBandType.PageHeader => "Page Header",
            ReportBandType.Detail => "Detail",
            ReportBandType.GroupHeader => "Group Header",
            ReportBandType.GroupFooter => "Group Footer",
            ReportBandType.PageFooter => "Page Footer",
            ReportBandType.ReportFooter => "Report Footer",
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
            ReportElementType.Subreport => ReportTreeNodeKind.Subreport,
            ReportElementType.Panel => ReportTreeNodeKind.Panel,
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

    internal static bool CanDeleteSection( ReportBandDefinition section )
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

        foreach ( var section in definition.Bands )
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

    internal static string EnsureBandId( ReportBandDefinition section )
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

        for ( int sectionIndex = 0; sectionIndex < definition.Bands.Count; sectionIndex++ )
        {
            ReportBandDefinition section = definition.Bands[sectionIndex];

            if ( TryFindElementLocation( section.Elements, key, sectionIndex, 0, 0, null, null, null, out location ) )
                return true;
        }

        return false;
    }

    internal static int RemoveElementsByIds( ReportDefinition definition, ISet<string> elementIds )
    {
        if ( definition is null || elementIds is null || elementIds.Count == 0 )
            return -1;

        int lastSectionIndex = -1;

        for ( int sectionIndex = 0; sectionIndex < definition.Bands.Count; sectionIndex++ )
        {
            ReportBandDefinition section = definition.Bands[sectionIndex];

            if ( RemoveElementsByIds( section.Elements, elementIds ) )
                lastSectionIndex = sectionIndex;
        }

        return lastSectionIndex;
    }

    private static bool TryFindElementLocation(
        IList<ReportElementDefinition> elements,
        string key,
        int sectionIndex,
        double ownerOffsetX,
        double ownerOffsetY,
        ReportPanelElementDefinition parentPanel,
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
                    OwnerOffsetX = ownerOffsetX,
                    OwnerOffsetY = ownerOffsetY,
                    ParentPanel = parentPanel,
                    ParentTable = parentTable,
                    ParentCell = parentCell,
                };

                return true;
            }

            if ( element is ReportPanelElementDefinition panel
                && TryFindElementLocation( panel.Elements, key, sectionIndex, ownerOffsetX + panel.X, ownerOffsetY + panel.Y, panel, null, null, out location ) )
            {
                return true;
            }

            if ( element is ReportTableElementDefinition table )
            {
                foreach ( ReportTableCellDefinition cell in table.Cells ?? [] )
                {
                    double cellOffsetX = table.Columns?.Take( cell.ColumnIndex ).Sum( column => Math.Max( 1, column.Width ) ) ?? 0;
                    double cellOffsetY = table.Rows?.Take( cell.RowIndex ).Sum( row => Math.Max( 1, row.Height ) ) ?? 0;

                    if ( TryFindElementLocation( cell.Elements, key, sectionIndex, ownerOffsetX + table.X + cellOffsetX, ownerOffsetY + table.Y + cellOffsetY, null, element, cell, out location ) )
                        return true;
                }
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

            foreach ( IList<ReportElementDefinition> childElements in GetChildElementCollections( element ) )
            {
                removed = RemoveElementsByIds( childElements, elementIds ) || removed;
            }
        }

        return removed;
    }

    internal static bool TryFindTableCellLocation(
        ReportDefinition definition,
        string cellKey,
        out int sectionIndex,
        out int tableIndex,
        out ReportTableElementDefinition table,
        out ReportTableCellDefinition cell )
    {
        sectionIndex = -1;
        tableIndex = -1;
        table = null;
        cell = null;

        if ( definition is null || string.IsNullOrWhiteSpace( cellKey ) )
            return false;

        for ( int currentSectionIndex = 0; currentSectionIndex < definition.Bands.Count; currentSectionIndex++ )
        {
            ReportBandDefinition section = definition.Bands[currentSectionIndex];

            foreach ( ReportTableElementDefinition tableElement in EnumerateElements( section.Elements ).OfType<ReportTableElementDefinition>() )
            {
                ReportTableCellDefinition foundCell = ( tableElement.Cells ?? [] ).FirstOrDefault( item => item.Id == cellKey );

                if ( foundCell is null )
                    continue;

                sectionIndex = currentSectionIndex;
                tableIndex = section.Elements.IndexOf( tableElement );
                table = tableElement;
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

        if ( element is ReportPanelElementDefinition panel )
        {
            foreach ( ReportElementDefinition childElement in panel.Elements ?? [] )
            {
                EnsureElementIds( childElement, elementIds, columnIds, rowIds, cellIds );
            }
        }

        if ( element is not ReportTableElementDefinition table )
            return;

        foreach ( ReportTableColumnDefinition column in table.Columns ?? Enumerable.Empty<ReportTableColumnDefinition>() )
        {
            column.Id = EnsureUniqueDefinitionId( column.Id, columnIds );
        }

        foreach ( ReportTableRowDefinition row in table.Rows ?? Enumerable.Empty<ReportTableRowDefinition>() )
        {
            row.Id = EnsureUniqueDefinitionId( row.Id, rowIds );
        }

        foreach ( ReportTableCellDefinition cell in table.Cells ?? Enumerable.Empty<ReportTableCellDefinition>() )
        {
            cell.Id = EnsureUniqueDefinitionId( cell.Id, cellIds );
            FitElementsToTableCell( table, cell );

            foreach ( ReportElementDefinition childElement in cell.Elements ?? Enumerable.Empty<ReportElementDefinition>() )
            {
                EnsureElementIds( childElement, elementIds, columnIds, rowIds, cellIds );
            }
        }
    }

    internal static void RegenerateElementIds( ReportElementDefinition element )
    {
        if ( element is null )
            return;

        element.Id = CreateDefinitionId();

        if ( element is ReportTableElementDefinition table )
        {
            foreach ( ReportTableColumnDefinition column in table.Columns ?? [] )
                column.Id = CreateDefinitionId();

            foreach ( ReportTableRowDefinition row in table.Rows ?? [] )
                row.Id = CreateDefinitionId();

            foreach ( ReportTableCellDefinition cell in table.Cells ?? [] )
                cell.Id = CreateDefinitionId();
        }

        foreach ( IList<ReportElementDefinition> childElements in GetChildElementCollections( element ) )
        {
            foreach ( ReportElementDefinition childElement in childElements )
                RegenerateElementIds( childElement );
        }
    }

    #endregion
}