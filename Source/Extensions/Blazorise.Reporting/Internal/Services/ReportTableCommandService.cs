#region Using directives
using System;
using System.Linq;
#endregion

namespace Blazorise.Reporting.Internal;

internal sealed class ReportTableCommandService
{
    #region Members

    private readonly ReportTableEditor tableEditor;

    #endregion

    #region Constructors

    internal ReportTableCommandService( ReportTableEditor tableEditor )
    {
        this.tableEditor = tableEditor;
    }

    #endregion

    #region Methods

    internal ReportTableCommandResult MergeCell( ReportDefinition definition, string cellKey, int columnSpanDelta, int rowSpanDelta )
    {
        if ( !TryFindCell( definition, cellKey, out ReportTableElementDefinition table, out ReportTableCellDefinition cell ) )
            return new();

        if ( columnSpanDelta > 0 && !tableEditor.CanMergeCellRight( table, cell ) )
            return new();

        if ( rowSpanDelta > 0 && !tableEditor.CanMergeCellDown( table, cell ) )
            return new();

        tableEditor.MergeCell( table, cell, columnSpanDelta, rowSpanDelta );
        ReportDefinitionHelper.FitElementsToTableCell( table, cell );

        return Changed( cell.Id );
    }

    internal ReportTableCommandResult UnmergeCell( ReportDefinition definition, string cellKey )
    {
        if ( !TryFindCell( definition, cellKey, out ReportTableElementDefinition table, out ReportTableCellDefinition cell ) )
            return new();

        int oldRowSpan = Math.Max( 1, cell.RowSpan );
        int oldColumnSpan = Math.Max( 1, cell.ColumnSpan );

        if ( oldRowSpan == 1 && oldColumnSpan == 1 )
            return new();

        cell.RowSpan = 1;
        cell.ColumnSpan = 1;
        ReportDefinitionHelper.FitElementsToTableCell( table, cell );

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

        return Changed( cell.Id );
    }

    internal ReportTableCommandResult InsertRow( ReportDefinition definition, string cellKey, bool insertBelow )
    {
        return UpdateCell( definition, cellKey, ( table, cell ) =>
        {
            int rowIndex = insertBelow
                ? cell.RowIndex + Math.Max( 1, cell.RowSpan )
                : cell.RowIndex;

            tableEditor.InsertRow( table, Math.Clamp( rowIndex, 0, table.Rows.Count ) );

            return FindCellKeyAt( table, Math.Min( rowIndex, table.Rows.Count - 1 ), cell.ColumnIndex );
        } );
    }

    internal ReportTableCommandResult InsertColumn( ReportDefinition definition, string cellKey, bool insertRight )
    {
        return UpdateCell( definition, cellKey, ( table, cell ) =>
        {
            int columnIndex = insertRight
                ? cell.ColumnIndex + Math.Max( 1, cell.ColumnSpan )
                : cell.ColumnIndex;

            tableEditor.InsertColumn( table, Math.Clamp( columnIndex, 0, table.Columns.Count ) );

            return FindCellKeyAt( table, cell.RowIndex, Math.Min( columnIndex, table.Columns.Count - 1 ) );
        } );
    }

    internal ReportTableCommandResult InsertCell( ReportDefinition definition, string cellKey )
    {
        return UpdateCell( definition, cellKey, ( table, cell ) =>
        {
            if ( !tableEditor.CanInsertCell( cell ) )
                return null;

            tableEditor.SplitCell( table, cell );

            return cell.Id;
        } );
    }

    internal ReportTableCommandResult DeleteRow( ReportDefinition definition, string cellKey )
    {
        return UpdateCell( definition, cellKey, ( table, cell ) =>
        {
            if ( !tableEditor.CanDeleteRow( table ) )
                return null;

            int deletedRowIndex = cell.RowIndex;
            tableEditor.DeleteRow( table, deletedRowIndex );

            return FindCellKeyAt( table, Math.Min( deletedRowIndex, table.Rows.Count - 1 ), Math.Min( cell.ColumnIndex, table.Columns.Count - 1 ) );
        } );
    }

    internal ReportTableCommandResult DeleteColumn( ReportDefinition definition, string cellKey )
    {
        return UpdateCell( definition, cellKey, ( table, cell ) =>
        {
            if ( !tableEditor.CanDeleteColumn( table ) )
                return null;

            int deletedColumnIndex = cell.ColumnIndex;
            tableEditor.DeleteColumn( table, deletedColumnIndex );

            return FindCellKeyAt( table, Math.Min( cell.RowIndex, table.Rows.Count - 1 ), Math.Min( deletedColumnIndex, table.Columns.Count - 1 ) );
        } );
    }

    internal ReportTableCommandResult DeleteCell( ReportDefinition definition, string cellKey )
    {
        return UpdateCell( definition, cellKey, ( table, cell ) =>
        {
            if ( !tableEditor.DeleteCell( table, cell, out ReportTableCellDefinition selectedCell ) )
                return null;

            return selectedCell.Id;
        } );
    }

    private ReportTableCommandResult UpdateCell( ReportDefinition definition, string cellKey, Func<ReportTableElementDefinition, ReportTableCellDefinition, string> update )
    {
        if ( !TryFindCell( definition, cellKey, out ReportTableElementDefinition table, out ReportTableCellDefinition cell ) )
            return new();

        tableEditor.EnsureGrid( table );

        string selectedCellKey = update( table, cell );

        tableEditor.NormalizeGrid( table );

        return string.IsNullOrWhiteSpace( selectedCellKey )
            ? new()
            : Changed( selectedCellKey );
    }

    private bool TryFindCell( ReportDefinition definition, string cellKey, out ReportTableElementDefinition table, out ReportTableCellDefinition cell )
    {
        return ReportDefinitionHelper.TryFindTableCellLocation( definition, cellKey, out _, out _, out table, out cell );
    }

    private string FindCellKeyAt( ReportTableElementDefinition table, int rowIndex, int columnIndex )
    {
        return tableEditor.FindCellAt( table, rowIndex, columnIndex )?.Id;
    }

    private static ReportTableCommandResult Changed( string selectedCellKey )
    {
        return new()
        {
            Changed = true,
            SelectedCellKey = selectedCellKey,
        };
    }

    #endregion
}