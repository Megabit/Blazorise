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
        => System.Math.Max( 1, Result.RowFields.Count );

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

    private string GetRowHeaderCaption( int index )
    {
        var field = GetRowField( index );

        if ( field is not null )
            return field.GetCaption();

        return Result.RowFields.Count == 0 ? ValuesText : string.Empty;
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

    private Background GetRowHeaderBackground( PivotGridAxisItem<TItem> row )
        => row.IsGrandTotal ? Background.Primary.Subtle : row.IsTotal ? Background.Light : Background.Default;

    private TextWeight GetRowHeaderTextWeight( PivotGridAxisItem<TItem> row )
        => row.IsTotal || row.IsGrandTotal ? TextWeight.Bold : TextWeight.Default;

    private string GetRowHeaderText( PivotGridResultRow<TItem> resultRow, int index )
    {
        var row = resultRow.Row;

        if ( row.IsGrandTotal )
            return index == 0 ? GrandTotalText : string.Empty;

        if ( index > row.Level || index >= Result.RowFields.Count )
            return string.Empty;

        var field = Result.RowFields[index];
        var value = index < row.Values.Count
            ? row.Values[index]
            : resultRow.Row.Items.Count > 0
                ? field.GetValue( resultRow.Row.Items[0] )
                : null;

        var text = field.FormatValue( value );

        if ( row.IsTotal && index == row.Level )
            text = $"{text} {TotalText}";

        return text;
    }

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
    /// Text shown when no value fields are declared.
    /// </summary>
    [Parameter] public string EmptyText { get; set; }

    /// <summary>
    /// Text shown when no data rows are available.
    /// </summary>
    [Parameter] public string NoDataText { get; set; }

    /// <summary>
    /// Text used for grand total labels.
    /// </summary>
    [Parameter] public string GrandTotalText { get; set; }

    /// <summary>
    /// Text appended to subtotal labels.
    /// </summary>
    [Parameter] public string TotalText { get; set; }

    /// <summary>
    /// Text shown when there are no column dimensions.
    /// </summary>
    [Parameter] public string ValuesText { get; set; }
}