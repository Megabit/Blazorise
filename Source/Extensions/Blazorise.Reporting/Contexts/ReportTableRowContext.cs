#region Using directives
using System;
#endregion

namespace Blazorise.Reporting;

internal sealed class ReportTableRowContext( ReportTableElementDefinition tableDefinition, int rowIndex )
{
    #region Methods

    internal ReportTableCellDefinition AddCell( int rowSpan, int columnSpan )
    {
        rowSpan = Math.Max( 1, rowSpan );
        columnSpan = Math.Max( 1, columnSpan );

        int columnIndex = 0;

        while ( IsPositionOccupied( RowIndex, columnIndex ) )
        {
            columnIndex++;
        }

        EnsureColumns( columnIndex + columnSpan );

        ReportTableCellDefinition definition = new()
        {
            RowIndex = RowIndex,
            ColumnIndex = columnIndex,
            RowSpan = rowSpan,
            ColumnSpan = columnSpan,
        };

        TableDefinition.Cells.Add( definition );

        return definition;
    }

    private void EnsureColumns( int columnCount )
    {
        while ( TableDefinition.Columns.Count < columnCount )
        {
            TableDefinition.Columns.Add( new()
            {
                Width = Internal.ReportDefinitionHelper.DefaultTableColumnWidth,
            } );
        }
    }

    private bool IsPositionOccupied( int rowIndex, int columnIndex )
    {
        foreach ( ReportTableCellDefinition cell in TableDefinition.Cells )
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

    #endregion

    #region Properties

    internal ReportTableElementDefinition TableDefinition { get; } = tableDefinition;

    internal int RowIndex { get; } = rowIndex;

    #endregion
}