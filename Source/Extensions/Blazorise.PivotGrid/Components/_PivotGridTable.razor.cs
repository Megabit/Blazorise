#region Using directives
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.PivotGrid.Components;

/// <summary>
/// Internal PivotGrid table renderer.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public partial class _PivotGridTable<TItem>
{
    private int RowHeaderColumnCount
        => UseTreeRowHeader ? 1 : System.Math.Max( 1, Result.RowFields.Count );

    private bool UseTreeRowHeader
        => PivotGrid.ExpandableRows && Result.RowFields.Count > 0;

    private int HeaderRowSpan
        => ShowValueHeaderRow ? 2 : 1;

    private int ValueColumnSpan
        => System.Math.Max( 1, Result.Aggregates.Count );

    private bool ShowValueHeaderRow
        => Result.Aggregates.Count > 1;

    private IReadOnlyList<PivotGridAxisItem<TItem>> ColumnGroups
        => Result.DataColumns
            .Select( x => x.Column )
            .Distinct()
            .ToList();

    private BasePivotGridField<TItem> GetRowField( int index )
        => index < Result.RowFields.Count ? Result.RowFields[index] : null;

    private string GrandTotalText
        => PivotGrid.LocalizedGrandTotalText;

    private string TotalText
        => PivotGrid.LocalizedTotalText;

    private string ValuesText
        => PivotGrid.LocalizedValuesText;

    private string GetRowHeaderCaption( int index )
    {
        if ( UseTreeRowHeader )
            return string.Join( " / ", Result.RowFields.Select( field => field.GetCaption() ) );

        var field = GetRowField( index );

        if ( field is not null )
            return field.GetCaption();

        return Result.RowFields.Count == 0 ? ValuesText : string.Empty;
    }

    private PivotGridHeaderContext<TItem> GetHeaderContext( int index )
    {
        var field = UseTreeRowHeader ? null : GetRowField( index );

        return new(
            field,
            GetRowHeaderCaption( index ),
            index,
            [] );
    }

    private string GetColumnHeaderText( PivotGridAxisItem<TItem> column )
    {
        if ( column.IsGrandTotal || Result.ColumnFields.Count == 0 )
            return Result.ColumnFields.Count == 0 ? ValuesText : GrandTotalText;

        var labels = column.Values.Select( ( value, index ) => Result.ColumnFields[index].FormatValue( value ) ).ToList();

        if ( column.IsTotal && labels.Count > 0 )
            labels[^1] = $"{labels[^1]} {TotalText}";

        return string.Join( " / ", labels );
    }

    private PivotGridColumnHeaderContext<TItem> GetColumnHeaderContext( PivotGridAxisItem<TItem> column )
    {
        if ( column.IsGrandTotal || Result.ColumnFields.Count == 0 || column.Values.Count == 0 )
        {
            var text = GetColumnHeaderText( column );

            return new(
                null,
                null,
                text,
                text,
                column.Level,
                column.Values,
                column,
                column.Items,
                column.IsTotal,
                column.IsGrandTotal );
        }

        var level = System.Math.Min( column.Level, Result.ColumnFields.Count - 1 );
        var field = Result.ColumnFields[level];
        var value = level < column.Values.Count ? column.Values[level] : null;
        var formattedValue = field.FormatValue( value );

        return new(
            field,
            value,
            formattedValue,
            GetColumnHeaderText( column ),
            level,
            column.Values,
            column,
            column.Items,
            column.IsTotal,
            column.IsGrandTotal );
    }

    private static PivotGridFieldValueContext<TItem> GetColumnFieldValueContext( PivotGridColumnHeaderContext<TItem> context )
        => new(
            context.Field,
            context.Value,
            context.FormattedValue,
            context.Level,
            context.Path,
            context.IsTotal,
            context.IsGrandTotal );

    private PivotGridAggregateHeaderContext<TItem> GetAggregateHeaderContext( PivotGridDataColumn<TItem> dataColumn )
        => new(
            dataColumn.Aggregate,
            dataColumn.Aggregate.GetCaption(),
            dataColumn,
            dataColumn.Column.Values,
            dataColumn.Column.IsTotal,
            dataColumn.Column.IsGrandTotal );

    private static PivotGridHeaderContext<TItem> GetAggregateFieldHeaderContext( PivotGridAggregateHeaderContext<TItem> context )
        => new(
            context.Aggregate,
            context.Caption,
            0,
            context.ColumnValues );

    private Background GetRowHeaderBackground( PivotGridAxisItem<TItem> row )
        => row.IsGrandTotal ? Background.Primary.Subtle : row.IsTotal ? Background.Light : Background.Default;

    private TextWeight GetRowHeaderTextWeight( PivotGridAxisItem<TItem> row )
        => row.IsTotal || row.IsGrandTotal ? TextWeight.Bold : TextWeight.Default;

    private IconName GetRowExpansionIcon( PivotGridAxisItem<TItem> row )
        => PivotGrid.IsRowExpanded( row ) ? IconName.ChevronDown : IconName.ChevronRight;

    private IconName GetColumnExpansionIcon( PivotGridAxisItem<TItem> column )
        => PivotGrid.IsColumnExpanded( column ) ? IconName.ChevronDown : IconName.ChevronRight;

    private bool IsRowTreeCell( PivotGridAxisItem<TItem> row, int index )
        => UseTreeRowHeader
            && !row.IsGrandTotal
            && index == 0;

    private PivotGridRowHeaderContext<TItem> GetRowHeaderContext( PivotGridResultRow<TItem> resultRow, int index )
    {
        var row = resultRow.Row;
        var level = UseTreeRowHeader
            ? System.Math.Min( row.Level, Result.RowFields.Count - 1 )
            : index;
        var field = GetRowField( level );

        if ( row.IsGrandTotal )
        {
            var grandTotalText = UseTreeRowHeader || index == 0 ? GrandTotalText : string.Empty;

            return new(
                field,
                null,
                grandTotalText,
                grandTotalText,
                level,
                row.Values,
                row,
                row.Items,
                false,
                true );
        }

        if ( !UseTreeRowHeader && ( index > row.Level || index >= Result.RowFields.Count ) )
        {
            return new(
                field,
                null,
                string.Empty,
                string.Empty,
                index,
                row.Values,
                row,
                row.Items,
                false,
                false );
        }

        var value = level < row.Values.Count
            ? row.Values[level]
            : resultRow.Row.Items.Count > 0
                ? field.GetValue( resultRow.Row.Items[0] )
                : null;

        var formattedValue = field.FormatValue( value );
        var text = formattedValue;

        if ( row.IsTotal && level == row.Level )
            text = $"{text} {TotalText}";

        return new(
            field,
            value,
            formattedValue,
            text,
            level,
            row.Values,
            row,
            row.Items,
            row.IsTotal,
            row.IsGrandTotal );
    }

    private static PivotGridFieldValueContext<TItem> GetRowFieldValueContext( PivotGridRowHeaderContext<TItem> context )
        => new(
            context.Field,
            context.Value,
            context.FormattedValue,
            context.Level,
            context.Path,
            context.IsTotal,
            context.IsGrandTotal );

    /// <summary>
    /// Pivot result to render.
    /// </summary>
    [Parameter] public PivotGridResult<TItem> Result { get; set; }

    /// <summary>
    /// Defines whether table cells have borders.
    /// </summary>
    [Parameter] public bool Bordered { get; set; }

    /// <summary>
    /// Defines whether rows are striped.
    /// </summary>
    [Parameter] public bool Striped { get; set; }

    /// <summary>
    /// Defines whether rows show hover styling.
    /// </summary>
    [Parameter] public bool Hoverable { get; set; }

    /// <summary>
    /// Defines whether cells use narrow spacing.
    /// </summary>
    [Parameter] public bool Narrow { get; set; }

    /// <summary>
    /// Defines whether table is responsive.
    /// </summary>
    [Parameter] public bool Responsive { get; set; }

    /// <summary>
    /// Defines header theme contrast.
    /// </summary>
    [Parameter] public ThemeContrast HeaderThemeContrast { get; set; }

    /// <summary>
    /// Parent PivotGrid component.
    /// </summary>
    [CascadingParameter] public PivotGrid<TItem> PivotGrid { get; set; }
}