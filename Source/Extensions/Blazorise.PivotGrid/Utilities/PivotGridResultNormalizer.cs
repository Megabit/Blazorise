#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.PivotGrid.Utilities;

internal static class PivotGridResultNormalizer
{
    public static PivotGridResult<TItem> Normalize<TItem>( PivotGridResult<TItem> result )
    {
        if ( result is null )
            return null;

        var dataColumns = new List<PivotGridDataColumn<TItem>>();
        var dataColumnIndexes = new List<int>();
        var sourceDataColumns = result.DataColumns ?? [];

        for ( var dataColumnIndex = 0; dataColumnIndex < sourceDataColumns.Count; dataColumnIndex++ )
        {
            var dataColumn = sourceDataColumns[dataColumnIndex];

            if ( dataColumn?.Column is null || dataColumn.Aggregate is null )
                continue;

            dataColumns.Add( new( NormalizeAxisItem( dataColumn.Column ), dataColumn.Aggregate ) );
            dataColumnIndexes.Add( dataColumnIndex );
        }

        var rows = ( result.Rows ?? [] )
            .Where( row => row?.Row is not null )
            .Select( row =>
            {
                var rowAxisItem = NormalizeAxisItem( row.Row );

                return new PivotGridResultRow<TItem>(
                    rowAxisItem,
                    dataColumnIndexes.Select( ( sourceIndex, index ) =>
                        row.Cells is not null && sourceIndex < row.Cells.Count
                            ? CreateCell( row.Cells[sourceIndex], dataColumns[index], rowAxisItem )
                            : CreateEmptyCell( dataColumns[index], rowAxisItem ) ).ToList() );
            } )
            .ToList();

        return new(
            result.RowFields?.Where( field => field is not null ).ToList() ?? [],
            result.ColumnFields?.Where( field => field is not null ).ToList() ?? [],
            result.Aggregates?.Where( aggregate => aggregate is not null ).ToList() ?? [],
            dataColumns,
            rows );
    }

    public static PivotGridResult<TItem> NormalizeVirtualized<TItem>( PivotGridResult<TItem> result, PivotGridResult<TItem> currentResult )
    {
        var normalizedResult = Normalize( result );

        if ( normalizedResult is null || currentResult?.DataColumns is null || currentResult.DataColumns.Count == 0 )
            return normalizedResult ?? PivotGridResult<TItem>.Empty;

        var dataColumnIndexes = currentResult.DataColumns
            .Select( dataColumn => FindDataColumnIndex( normalizedResult.DataColumns, dataColumn ) )
            .ToList();

        var rows = normalizedResult.Rows
            .Select( row => new PivotGridResultRow<TItem>(
                row.Row,
                dataColumnIndexes.Select( ( dataColumnIndex, index ) =>
                    dataColumnIndex >= 0 && dataColumnIndex < row.Cells.Count
                        ? CreateCell( row.Cells[dataColumnIndex], currentResult.DataColumns[index], row.Row )
                        : CreateEmptyCell( currentResult.DataColumns[index], row.Row ) ).ToList() ) )
            .ToList();

        return new( currentResult.RowFields, currentResult.ColumnFields, currentResult.Aggregates, currentResult.DataColumns, rows );
    }

    private static int FindDataColumnIndex<TItem>( IReadOnlyList<PivotGridDataColumn<TItem>> dataColumns, PivotGridDataColumn<TItem> currentDataColumn )
    {
        for ( var i = 0; i < dataColumns.Count; i++ )
        {
            var dataColumn = dataColumns[i];

            if ( PivotGridAxisItemEqualityComparer<TItem>.Instance.Equals( dataColumn.Column, currentDataColumn.Column )
                 && AggregateInfoMatches( dataColumn.Aggregate, currentDataColumn.Aggregate ) )
                return i;
        }

        return -1;
    }

    private static bool AggregateInfoMatches<TItem>( PivotGridAggregateInfo<TItem> aggregate, PivotGridAggregateInfo<TItem> currentAggregate )
        => string.Equals( aggregate.Field, currentAggregate.Field, StringComparison.Ordinal )
            && string.Equals( aggregate.Caption, currentAggregate.Caption, StringComparison.Ordinal )
            && aggregate.Aggregate == currentAggregate.Aggregate;

    private static PivotGridAxisItem<TItem> NormalizeAxisItem<TItem>( PivotGridAxisItem<TItem> axisItem )
        => axisItem.Values is not null && axisItem.Items is not null
            ? axisItem
            : new( axisItem.Values ?? [], axisItem.Items ?? [], axisItem.Level, axisItem.IsTotal, axisItem.IsGrandTotal );

    private static PivotGridCell<TItem> CreateCell<TItem>( PivotGridCell<TItem> cell, PivotGridDataColumn<TItem> dataColumn, PivotGridAxisItem<TItem> row )
        => cell is null
            ? CreateEmptyCell( dataColumn, row )
            : new( dataColumn, cell.Value, cell.FormattedValue ?? string.Empty, cell.Items ?? [], cell.IsRowTotal, cell.IsColumnTotal, cell.IsGrandTotal );

    private static PivotGridCell<TItem> CreateEmptyCell<TItem>( PivotGridDataColumn<TItem> dataColumn, PivotGridAxisItem<TItem> row )
    {
        var isColumnTotalsRow = row.IsTotal || row.IsGrandTotal;
        var isRowTotalsColumn = dataColumn.Column.IsTotal || dataColumn.Column.IsGrandTotal;

        return new( dataColumn, null, dataColumn.Aggregate.EmptyText, [], isRowTotalsColumn, isColumnTotalsRow, row.IsGrandTotal && dataColumn.Column.IsGrandTotal );
    }
}