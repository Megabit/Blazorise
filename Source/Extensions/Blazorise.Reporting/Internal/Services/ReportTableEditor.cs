#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportTableEditor
{
    #region Methods

    internal bool TryFindCellAt( ReportBandDefinition section, double x, double y, out ReportTableCellDropTarget target )
    {
        target = null;

        return section?.Elements is not null
            && TryFindCellAt( section.Elements, x, y, 0, 0, out target );
    }

    private bool TryFindCellAt( IList<ReportElementDefinition> elements, double x, double y, double offsetX, double offsetY, out ReportTableCellDropTarget target )
    {
        target = null;

        for ( int elementIndex = elements.Count - 1; elementIndex >= 0; elementIndex-- )
        {
            ReportElementDefinition element = elements[elementIndex];

            if ( element is ReportPanelElementDefinition panel
                && panel.Suppress?.Value != true
                && x >= offsetX + panel.X
                && x <= offsetX + panel.X + panel.Width
                && y >= offsetY + panel.Y
                && y <= offsetY + panel.Y + panel.Height
                && panel.Elements is not null
                && TryFindCellAt( panel.Elements, x, y, offsetX + panel.X, offsetY + panel.Y, out target ) )
            {
                return true;
            }

            if ( element is not ReportTableElementDefinition table
                || table.Suppress?.Value == true
                || x < offsetX + table.X
                || x > offsetX + table.X + table.Width
                || y < offsetY + table.Y
                || y > offsetY + table.Y + table.Height )
            {
                continue;
            }

            EnsureGrid( table );

            double localX = x - offsetX - table.X;
            double localY = y - offsetY - table.Y;

            foreach ( ReportTableCellDefinition cell in table.Cells.OrderBy( cell => cell.RowIndex ).ThenBy( cell => cell.ColumnIndex ) )
            {
                double cellX = GetColumnOffset( table, cell.ColumnIndex );
                double cellY = GetRowOffset( table, cell.RowIndex );
                double cellWidth = ReportDefinitionHelper.GetTableCellWidth( table, cell );
                double cellHeight = ReportDefinitionHelper.GetTableCellHeight( table, cell );

                if ( localX >= cellX
                    && localX <= cellX + cellWidth
                    && localY >= cellY
                    && localY <= cellY + cellHeight )
                {
                    target = new()
                    {
                        Table = table,
                        Cell = cell,
                        X = Math.Max( 0, localX - cellX ),
                        Y = Math.Max( 0, localY - cellY ),
                    };

                    return true;
                }
            }
        }

        return false;
    }

    internal bool CanInsertCell( ReportTableCellDefinition cell )
    {
        return cell is not null
            && ( cell.RowSpan > 1 || cell.ColumnSpan > 1 );
    }

    internal bool CanDeleteRow( ReportTableElementDefinition table )
    {
        return table?.Rows?.Count > 1;
    }

    internal bool CanDeleteColumn( ReportTableElementDefinition table )
    {
        return table?.Columns?.Count > 1;
    }

    internal bool CanDeleteCell( ReportTableElementDefinition table, ReportTableCellDefinition cell )
    {
        if ( table is null || cell is null )
            return false;

        return FindCellLeftOf( table, cell ) is { } leftCell && CanMergeCellRight( table, leftCell )
            || FindCellAbove( table, cell ) is { } aboveCell && CanMergeCellDown( table, aboveCell )
            || CanMergeCellRight( table, cell )
            || CanMergeCellDown( table, cell );
    }

    internal void InsertRow( ReportTableElementDefinition table, int rowIndex )
    {
        EnsureGrid( table );

        double rowHeight = ResolveRowHeight( table, rowIndex );
        table.Rows.Insert( rowIndex, new()
        {
            Height = rowHeight,
        } );

        foreach ( ReportTableCellDefinition cell in table.Cells )
        {
            if ( cell.RowIndex >= rowIndex )
            {
                cell.RowIndex++;
            }
            else if ( cell.RowIndex + Math.Max( 1, cell.RowSpan ) > rowIndex )
            {
                cell.RowSpan++;
            }
        }

        table.Height += rowHeight;
    }

    internal void InsertColumn( ReportTableElementDefinition table, int columnIndex )
    {
        EnsureGrid( table );

        double columnWidth = ResolveColumnWidth( table, columnIndex );
        table.Columns.Insert( columnIndex, new()
        {
            Width = columnWidth,
        } );

        foreach ( ReportTableCellDefinition cell in table.Cells )
        {
            if ( cell.ColumnIndex >= columnIndex )
            {
                cell.ColumnIndex++;
            }
            else if ( cell.ColumnIndex + Math.Max( 1, cell.ColumnSpan ) > columnIndex )
            {
                cell.ColumnSpan++;
            }
        }

        table.Width += columnWidth;
    }

    internal void SplitCell( ReportTableElementDefinition table, ReportTableCellDefinition cell )
    {
        int oldRowSpan = Math.Max( 1, cell.RowSpan );
        int oldColumnSpan = Math.Max( 1, cell.ColumnSpan );

        cell.RowSpan = 1;
        cell.ColumnSpan = 1;

        for ( int rowIndex = cell.RowIndex; rowIndex < cell.RowIndex + oldRowSpan; rowIndex++ )
        {
            for ( int columnIndex = cell.ColumnIndex; columnIndex < cell.ColumnIndex + oldColumnSpan; columnIndex++ )
            {
                if ( rowIndex == cell.RowIndex && columnIndex == cell.ColumnIndex )
                    continue;

                if ( table.Cells.Any( item => item.RowIndex == rowIndex && item.ColumnIndex == columnIndex ) )
                    continue;

                table.Cells.Add( new()
                {
                    RowIndex = rowIndex,
                    ColumnIndex = columnIndex,
                } );
            }
        }
    }

    internal void DeleteRow( ReportTableElementDefinition table, int rowIndex )
    {
        EnsureGrid( table );

        double rowHeight = table.Rows[rowIndex].Height;
        table.Rows.RemoveAt( rowIndex );

        foreach ( ReportTableCellDefinition cell in table.Cells.ToList() )
        {
            if ( cell.RowIndex == rowIndex )
            {
                if ( cell.RowSpan > 1 )
                    cell.RowSpan--;
                else
                    table.Cells.Remove( cell );
            }
            else if ( cell.RowIndex < rowIndex && cell.RowIndex + Math.Max( 1, cell.RowSpan ) > rowIndex )
            {
                cell.RowSpan--;
            }
            else if ( cell.RowIndex > rowIndex )
            {
                cell.RowIndex--;
            }
        }

        table.Height = Math.Max( 1, table.Height - rowHeight );
    }

    internal void DeleteColumn( ReportTableElementDefinition table, int columnIndex )
    {
        EnsureGrid( table );

        double columnWidth = table.Columns[columnIndex].Width;
        table.Columns.RemoveAt( columnIndex );

        foreach ( ReportTableCellDefinition cell in table.Cells.ToList() )
        {
            if ( cell.ColumnIndex == columnIndex )
            {
                if ( cell.ColumnSpan > 1 )
                    cell.ColumnSpan--;
                else
                    table.Cells.Remove( cell );
            }
            else if ( cell.ColumnIndex < columnIndex && cell.ColumnIndex + Math.Max( 1, cell.ColumnSpan ) > columnIndex )
            {
                cell.ColumnSpan--;
            }
            else if ( cell.ColumnIndex > columnIndex )
            {
                cell.ColumnIndex--;
            }
        }

        table.Width = Math.Max( 1, table.Width - columnWidth );
    }

    internal bool DeleteCell( ReportTableElementDefinition table, ReportTableCellDefinition cell, out ReportTableCellDefinition selectedCell )
    {
        selectedCell = null;

        if ( table is null || cell is null )
            return false;

        if ( FindCellLeftOf( table, cell ) is { } leftCell && CanMergeCellRight( table, leftCell ) )
        {
            MergeCellRight( table, leftCell );
            selectedCell = leftCell;
            return true;
        }

        if ( FindCellAbove( table, cell ) is { } aboveCell && CanMergeCellDown( table, aboveCell ) )
        {
            MergeCellDown( table, aboveCell );
            selectedCell = aboveCell;
            return true;
        }

        if ( CanMergeCellRight( table, cell ) )
        {
            MergeCellRight( table, cell );
            selectedCell = cell;
            return true;
        }

        if ( CanMergeCellDown( table, cell ) )
        {
            MergeCellDown( table, cell );
            selectedCell = cell;
            return true;
        }

        return false;
    }

    internal void MergeCellRight( ReportTableElementDefinition table, ReportTableCellDefinition cell )
    {
        MergeCell( table, cell, columnSpanDelta: 1, rowSpanDelta: 0 );
    }

    internal void MergeCellDown( ReportTableElementDefinition table, ReportTableCellDefinition cell )
    {
        MergeCell( table, cell, columnSpanDelta: 0, rowSpanDelta: 1 );
    }

    internal void MergeCell( ReportTableElementDefinition table, ReportTableCellDefinition cell, int columnSpanDelta, int rowSpanDelta )
    {
        int oldRowSpan = Math.Max( 1, cell.RowSpan );
        int oldColumnSpan = Math.Max( 1, cell.ColumnSpan );
        int newRowSpan = oldRowSpan + rowSpanDelta;
        int newColumnSpan = oldColumnSpan + columnSpanDelta;

        var coveredCells = table.Cells
            .Where( item => item != cell
                && item.RowIndex >= cell.RowIndex
                && item.RowIndex < cell.RowIndex + newRowSpan
                && item.ColumnIndex >= cell.ColumnIndex
                && item.ColumnIndex < cell.ColumnIndex + newColumnSpan )
            .ToList();

        foreach ( ReportTableCellDefinition coveredCell in coveredCells )
        {
            if ( coveredCell.Elements?.Count > 0 )
                cell.Elements.AddRange( coveredCell.Elements );

            table.Cells.Remove( coveredCell );
        }

        cell.RowSpan = newRowSpan;
        cell.ColumnSpan = newColumnSpan;
    }

    internal ReportTableCellDefinition FindCellAt( ReportTableElementDefinition table, int rowIndex, int columnIndex )
    {
        return table?.Cells
            ?.Where( item => item.RowIndex <= rowIndex
                && item.RowIndex + Math.Max( 1, item.RowSpan ) > rowIndex
                && item.ColumnIndex <= columnIndex
                && item.ColumnIndex + Math.Max( 1, item.ColumnSpan ) > columnIndex )
            .OrderByDescending( item => item.RowSpan * item.ColumnSpan )
            .FirstOrDefault();
    }

    internal void NormalizeGrid( ReportTableElementDefinition table )
    {
        if ( table is null )
            return;

        ReportDefinitionHelper.EnsureTableLayout( table, table.Rows.Count, table.Columns.Count );

        foreach ( ReportTableCellDefinition cell in table.Cells )
        {
            ReportDefinitionHelper.FitElementsToTableCell( table, cell );
        }
    }

    internal void EnsureGrid( ReportTableElementDefinition table )
    {
        ReportDefinitionHelper.EnsureTableLayout(
            table,
            table.Rows?.Count > 0 ? table.Rows.Count : 1,
            table.Columns?.Count > 0 ? table.Columns.Count : 1 );
    }

    internal double ResolveRowHeight( ReportTableElementDefinition table, int rowIndex )
    {
        if ( table?.Rows is null || table.Rows.Count == 0 )
            return ReportDefinitionHelper.DefaultTableRowHeight;

        if ( rowIndex > 0 && rowIndex - 1 < table.Rows.Count )
            return table.Rows[rowIndex - 1].Height;

        return table.Rows[Math.Min( rowIndex, table.Rows.Count - 1 )].Height;
    }

    internal double ResolveColumnWidth( ReportTableElementDefinition table, int columnIndex )
    {
        if ( table?.Columns is null || table.Columns.Count == 0 )
            return ReportDefinitionHelper.DefaultTableColumnWidth;

        if ( columnIndex > 0 && columnIndex - 1 < table.Columns.Count )
            return table.Columns[columnIndex - 1].Width;

        return table.Columns[Math.Min( columnIndex, table.Columns.Count - 1 )].Width;
    }

    internal bool CanMergeCellRight( ReportTableElementDefinition table, ReportTableCellDefinition cell )
    {
        if ( table?.Columns is null || cell is null )
            return false;

        int targetColumnIndex = cell.ColumnIndex + Math.Max( 1, cell.ColumnSpan );

        if ( targetColumnIndex >= table.Columns.Count )
            return false;

        int rowSpan = Math.Max( 1, cell.RowSpan );

        for ( int rowIndex = cell.RowIndex; rowIndex < cell.RowIndex + rowSpan; rowIndex++ )
        {
            ReportTableCellDefinition targetCell = table.Cells?.FirstOrDefault( item => item.RowIndex == rowIndex && item.ColumnIndex == targetColumnIndex );

            if ( targetCell is null || targetCell.RowSpan != 1 || targetCell.ColumnSpan != 1 )
                return false;
        }

        return true;
    }

    internal bool CanMergeCellDown( ReportTableElementDefinition table, ReportTableCellDefinition cell )
    {
        if ( table?.Rows is null || cell is null )
            return false;

        int targetRowIndex = cell.RowIndex + Math.Max( 1, cell.RowSpan );

        if ( targetRowIndex >= table.Rows.Count )
            return false;

        int columnSpan = Math.Max( 1, cell.ColumnSpan );

        for ( int columnIndex = cell.ColumnIndex; columnIndex < cell.ColumnIndex + columnSpan; columnIndex++ )
        {
            ReportTableCellDefinition targetCell = table.Cells?.FirstOrDefault( item => item.RowIndex == targetRowIndex && item.ColumnIndex == columnIndex );

            if ( targetCell is null || targetCell.RowSpan != 1 || targetCell.ColumnSpan != 1 )
                return false;
        }

        return true;
    }

    internal double GetColumnOffset( ReportTableElementDefinition table, int columnIndex )
    {
        return table.Columns.Take( Math.Max( 0, columnIndex ) ).Sum( column => Math.Max( 1, column.Width ) );
    }

    internal double GetRowOffset( ReportTableElementDefinition table, int rowIndex )
    {
        return table.Rows.Take( Math.Max( 0, rowIndex ) ).Sum( row => Math.Max( 1, row.Height ) );
    }

    internal void ReplaceCellElement( ReportTableElementDefinition table, ReportTableCellDefinition cell, ReportElementDefinition element )
    {
        if ( table is null || cell is null || element is null )
            return;

        cell.Elements.Clear();
        ReportDefinitionHelper.FitElementToTableCell( table, cell, element );
        cell.Elements.Add( element );
    }

    private ReportTableCellDefinition FindCellLeftOf( ReportTableElementDefinition table, ReportTableCellDefinition cell )
    {
        if ( cell.ColumnIndex == 0 )
            return null;

        return table.Cells?.FirstOrDefault( item =>
            item.RowIndex == cell.RowIndex
            && item.ColumnIndex + Math.Max( 1, item.ColumnSpan ) == cell.ColumnIndex
            && Math.Max( 1, item.RowSpan ) == Math.Max( 1, cell.RowSpan ) );
    }

    private ReportTableCellDefinition FindCellAbove( ReportTableElementDefinition table, ReportTableCellDefinition cell )
    {
        if ( cell.RowIndex == 0 )
            return null;

        return table.Cells?.FirstOrDefault( item =>
            item.ColumnIndex == cell.ColumnIndex
            && item.RowIndex + Math.Max( 1, item.RowSpan ) == cell.RowIndex
            && Math.Max( 1, item.ColumnSpan ) == Math.Max( 1, cell.ColumnSpan ) );
    }

    #endregion
}