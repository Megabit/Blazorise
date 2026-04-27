#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.PivotGrid.Utilities;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.PivotGrid;

/// <summary>
/// Represents a read-only pivot table component for multi-dimensional data analysis.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public partial class PivotGrid<TItem> : BaseComponent
{
    #region Members

    private readonly List<BasePivotGridField<TItem>> fields = new();
    private readonly Dictionary<BasePivotGridField<TItem>, int> fieldStateHashes = new();
    private PivotGridResult<TItem> pivotResult = PivotGridResult<TItem>.Empty;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        await base.SetParametersAsync( parameters );

        RebuildPivot();
    }

    /// <inheritdoc />
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( "b-pivot-grid" );

        base.BuildClasses( builder );
    }

    internal void RegisterField( BasePivotGridField<TItem> field )
    {
        if ( field is null )
            return;

        var fieldStateHash = field.GetFieldStateHash();
        var fieldExists = fieldStateHashes.TryGetValue( field, out var previousFieldStateHash );

        if ( !fields.Contains( field ) )
            fields.Add( field );

        if ( !fieldExists || previousFieldStateHash != fieldStateHash )
        {
            fieldStateHashes[field] = fieldStateHash;
            RebuildPivot();

            _ = InvokeAsync( StateHasChanged );
        }
    }

    internal bool RemoveField( BasePivotGridField<TItem> field )
    {
        if ( field is null )
            return false;

        fieldStateHashes.Remove( field );

        var removed = fields.Remove( field );

        if ( removed )
        {
            RebuildPivot();
            _ = InvokeAsync( StateHasChanged );
        }

        return removed;
    }

    internal Task NotifyCellClicked( PivotGridCell<TItem> cell, PivotGridAxisItem<TItem> row )
    {
        if ( !CellClicked.HasDelegate || cell is null || row is null )
            return Task.CompletedTask;

        return CellClicked.InvokeAsync( new PivotGridCellClickedEventArgs<TItem>(
            cell.DataColumn.Aggregate,
            cell.Value,
            row.Values,
            cell.DataColumn.Column.Values,
            cell.Items ) );
    }

    private void RebuildPivot()
    {
        var rowFields = fields.Where( x => x.Visible && x.FieldArea == PivotGridFieldArea.Row ).ToList();
        var columnFields = fields.Where( x => x.Visible && x.FieldArea == PivotGridFieldArea.Column ).ToList();
        var aggregates = fields.OfType<PivotGridAggregate<TItem>>().Where( x => x.Visible && x.FieldArea == PivotGridFieldArea.Aggregate ).ToList();

        if ( aggregates.Count == 0 )
        {
            pivotResult = new( rowFields, columnFields, aggregates, [], [] );
            return;
        }

        var sourceItems = Items?.ToList() ?? [];
        var rowAxisItems = BuildAxisItems( sourceItems, rowFields, ShowRowSubtotals, ShowRowTotals, RowTotalPosition );
        var columnAxisItems = BuildAxisItems( sourceItems, columnFields, ShowColumnSubtotals, ShowColumnTotals, ColumnTotalPosition );
        var dataColumns = columnAxisItems
            .SelectMany( column => aggregates.Select( aggregate => new PivotGridDataColumn<TItem>( column, aggregate ) ) )
            .ToList();

        var rows = rowAxisItems
            .Select( row => new PivotGridResultRow<TItem>( row, BuildCells( row, columnFields, dataColumns ) ) )
            .ToList();

        pivotResult = new( rowFields, columnFields, aggregates, dataColumns, rows );
    }

    private IReadOnlyList<PivotGridCell<TItem>> BuildCells( PivotGridAxisItem<TItem> row, IReadOnlyList<BasePivotGridField<TItem>> columnFields, IReadOnlyList<PivotGridDataColumn<TItem>> dataColumns )
    {
        return dataColumns.Select( dataColumn =>
        {
            var cellItems = GetMatchingItems( row.Items, columnFields, dataColumn.Column.Values );
            var value = dataColumn.Aggregate.Aggregate( cellItems );
            var formattedValue = dataColumn.Aggregate.FormatValue( value );

            return new PivotGridCell<TItem>(
                dataColumn,
                value,
                formattedValue,
                cellItems,
                row.IsTotal || row.IsGrandTotal,
                dataColumn.Column.IsTotal || dataColumn.Column.IsGrandTotal,
                row.IsGrandTotal && dataColumn.Column.IsGrandTotal );
        } ).ToList();
    }

    private IReadOnlyList<TItem> GetMatchingItems( IReadOnlyList<TItem> items, IReadOnlyList<BasePivotGridField<TItem>> columnFields, IReadOnlyList<object> values )
    {
        if ( values.Count == 0 )
            return items;

        return items.Where( item => MatchesPath( item, columnFields, values ) ).ToList();
    }

    private IReadOnlyList<PivotGridAxisItem<TItem>> BuildAxisItems( IReadOnlyList<TItem> items, IReadOnlyList<BasePivotGridField<TItem>> axisFields, bool showSubtotals, bool showGrandTotal, PivotGridTotalPosition totalPosition )
    {
        var result = new List<PivotGridAxisItem<TItem>>();

        if ( axisFields.Count == 0 )
        {
            result.Add( new PivotGridAxisItem<TItem>( [], items, 0, true, true ) );
            return result;
        }

        BuildAxisItems( result, items, axisFields, 0, [] , showSubtotals, totalPosition );

        if ( showGrandTotal )
            result.Add( new PivotGridAxisItem<TItem>( [], items, 0, true, true ) );

        return result;
    }

    private void BuildAxisItems( List<PivotGridAxisItem<TItem>> result, IReadOnlyList<TItem> items, IReadOnlyList<BasePivotGridField<TItem>> axisFields, int level, IReadOnlyList<object> parentValues, bool showSubtotals, PivotGridTotalPosition totalPosition )
    {
        var field = axisFields[level];
        var groups = items
            .GroupBy( field.GetValue, PivotGridObjectEqualityComparer.Instance )
            .OrderBy( group => field.FormatValue( group.Key ), StringComparer.CurrentCultureIgnoreCase )
            .ToList();

        foreach ( var group in groups )
        {
            var values = parentValues.Concat( [group.Key] ).ToList();
            var groupItems = group.ToList();
            var isLeafLevel = level == axisFields.Count - 1;
            var subtotal = new PivotGridAxisItem<TItem>( values, groupItems, level, true, false );

            if ( showSubtotals && !isLeafLevel && totalPosition == PivotGridTotalPosition.Before )
                result.Add( subtotal );

            if ( isLeafLevel )
            {
                result.Add( new PivotGridAxisItem<TItem>( values, groupItems, level, false, false ) );
            }
            else
            {
                BuildAxisItems( result, groupItems, axisFields, level + 1, values, showSubtotals, totalPosition );
            }

            if ( showSubtotals && !isLeafLevel && totalPosition == PivotGridTotalPosition.After )
                result.Add( subtotal );
        }
    }

    private bool MatchesPath( TItem item, IReadOnlyList<BasePivotGridField<TItem>> axisFields, IReadOnlyList<object> values )
    {
        for ( var i = 0; i < values.Count; i++ )
        {
            if ( !object.Equals( axisFields[i].GetValue( item ), values[i] ) )
                return false;
        }

        return true;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the source items to be analyzed.
    /// </summary>
    [Parameter] public IEnumerable<TItem> Items { get; set; }

    /// <summary>
    /// Defines child content. Field components can be declared directly here.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Shows subtotal rows for row dimensions.
    /// </summary>
    [Parameter] public bool ShowRowSubtotals { get; set; } = true;

    /// <summary>
    /// Shows subtotal columns for column dimensions.
    /// </summary>
    [Parameter] public bool ShowColumnSubtotals { get; set; } = true;

    /// <summary>
    /// Shows the row totals.
    /// </summary>
    [Parameter] public bool ShowRowTotals { get; set; } = true;

    /// <summary>
    /// Shows the column totals.
    /// </summary>
    [Parameter] public bool ShowColumnTotals { get; set; } = true;

    /// <summary>
    /// Defines row subtotal position.
    /// </summary>
    [Parameter] public PivotGridTotalPosition RowTotalPosition { get; set; } = PivotGridTotalPosition.After;

    /// <summary>
    /// Defines column subtotal position.
    /// </summary>
    [Parameter] public PivotGridTotalPosition ColumnTotalPosition { get; set; } = PivotGridTotalPosition.After;

    /// <summary>
    /// Defines whether table cells have borders.
    /// </summary>
    [Parameter] public bool Bordered { get; set; } = true;

    /// <summary>
    /// Defines whether rows are striped.
    /// </summary>
    [Parameter] public bool Striped { get; set; }

    /// <summary>
    /// Defines whether rows show hover styling.
    /// </summary>
    [Parameter] public bool Hoverable { get; set; } = true;

    /// <summary>
    /// Defines whether cells use narrow spacing.
    /// </summary>
    [Parameter] public bool Narrow { get; set; }

    /// <summary>
    /// Defines whether table is responsive.
    /// </summary>
    [Parameter] public bool Responsive { get; set; } = true;

    /// <summary>
    /// Defines whether header uses theme contrast.
    /// </summary>
    [Parameter] public ThemeContrast HeaderThemeContrast { get; set; } = ThemeContrast.Light;

    /// <summary>
    /// Text shown for empty values.
    /// </summary>
    [Parameter] public string EmptyText { get; set; } = string.Empty;

    /// <summary>
    /// Text shown when there are no data rows.
    /// </summary>
    [Parameter] public string NoDataText { get; set; } = "No data to display.";

    /// <summary>
    /// Text shown when no value fields are declared.
    /// </summary>
    [Parameter] public string MissingValuesText { get; set; } = "No value fields are declared.";

    /// <summary>
    /// Text used for grand total row and column labels.
    /// </summary>
    [Parameter] public string GrandTotalText { get; set; } = "Grand Total";

    /// <summary>
    /// Text appended to subtotal row and column labels.
    /// </summary>
    [Parameter] public string TotalText { get; set; } = "Total";

    /// <summary>
    /// Text used for the value field header.
    /// </summary>
    [Parameter] public string ValuesText { get; set; } = "Values";

    /// <summary>
    /// Occurs when an aggregate cell is clicked.
    /// </summary>
    [Parameter] public EventCallback<PivotGridCellClickedEventArgs<TItem>> CellClicked { get; set; }

    #endregion
}