using System;

namespace Blazorise.Reporting;

internal sealed class ReportTableRowContext
{
    private readonly ReportRegistrationCollection<ReportTableCellDefinition> cells = new();

    internal void Attach( ReportTableContext tableContext, ReportTableRowDefinition rowDefinition )
    {
        RemoveCells();
        TableContext = tableContext;
        RowDefinition = rowDefinition;
        RebuildCells();
    }

    internal void Detach()
    {
        RemoveCells();
        TableContext = null;
        RowDefinition = null;
    }

    internal ReportTableCellDefinition RegisterCell( object owner, int rowSpan, int columnSpan )
    {
        if ( !cells.TryGetValue( owner, out ReportTableCellDefinition definition ) )
            definition = new();

        definition.RowSpan = Math.Max( 1, rowSpan );
        definition.ColumnSpan = Math.Max( 1, columnSpan );
        cells.Set( owner, definition );
        RebuildCells();

        return definition;
    }

    internal void UnregisterCell( object owner )
    {
        if ( cells.Remove( owner ) )
            RebuildCells();
    }

    internal void NotifyDefinitionChanged()
    {
        TableContext?.NotifyDefinitionChanged();
    }

    private void RebuildCells()
    {
        ReportTableElementDefinition tableDefinition = TableDefinition;

        if ( tableDefinition is null || RowDefinition is null )
            return;

        foreach ( ReportTableCellDefinition cell in cells.Values )
            tableDefinition.Cells.Remove( cell );

        int rowIndex = tableDefinition.Rows.IndexOf( RowDefinition );

        if ( rowIndex < 0 )
            return;

        foreach ( ReportTableCellDefinition cell in cells.Values )
        {
            int columnIndex = 0;

            while ( IsPositionOccupied( tableDefinition, rowIndex, columnIndex ) )
                columnIndex++;

            EnsureColumns( tableDefinition, columnIndex + cell.ColumnSpan );
            cell.RowIndex = rowIndex;
            cell.ColumnIndex = columnIndex;
            tableDefinition.Cells.Add( cell );
        }

        NotifyDefinitionChanged();
    }

    private void RemoveCells()
    {
        if ( TableDefinition is not ReportTableElementDefinition tableDefinition )
            return;

        foreach ( ReportTableCellDefinition cell in cells.Values )
            tableDefinition.Cells.Remove( cell );
    }

    private static void EnsureColumns( ReportTableElementDefinition tableDefinition, int columnCount )
    {
        while ( tableDefinition.Columns.Count < columnCount )
        {
            tableDefinition.Columns.Add( new()
            {
                Width = Internal.ReportDefinitionHelper.DefaultTableColumnWidth,
            } );
        }
    }

    private static bool IsPositionOccupied( ReportTableElementDefinition tableDefinition, int rowIndex, int columnIndex )
    {
        foreach ( ReportTableCellDefinition cell in tableDefinition.Cells )
        {
            int rowSpan = Math.Max( 1, cell.RowSpan );
            int columnSpan = Math.Max( 1, cell.ColumnSpan );

            if ( rowIndex >= cell.RowIndex
                && rowIndex < cell.RowIndex + rowSpan
                && columnIndex >= cell.ColumnIndex
                && columnIndex < cell.ColumnIndex + columnSpan )
            {
                return true;
            }
        }

        return false;
    }

    internal ReportTableContext TableContext { get; private set; }

    internal ReportTableElementDefinition TableDefinition => TableContext?.Definition;

    internal ReportTableRowDefinition RowDefinition { get; private set; }
}