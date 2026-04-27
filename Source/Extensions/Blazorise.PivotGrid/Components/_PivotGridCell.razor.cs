#region Using directives
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.PivotGrid.Components;

/// <summary>
/// Internal PivotGrid aggregate cell.
/// </summary>
/// <typeparam name="TItem">Item type.</typeparam>
public partial class _PivotGridCell<TItem>
{
    private PivotGridCellContext<TItem> CellContext
        => new(
            Cell.DataColumn.Aggregate,
            Cell.Value,
            Cell.FormattedValue,
            Row.Values,
            Cell.DataColumn.Column.Values,
            Cell.Items,
            Cell.IsRowTotal,
            Cell.IsColumnTotal,
            Cell.IsGrandTotal );

    private TextWeight CellTextWeight
        => Cell.IsRowTotal || Cell.IsColumnTotal ? TextWeight.Bold : TextWeight.Default;

    private Background CellBackground
        => Cell.IsGrandTotal ? Background.Primary.Subtle : Cell.IsRowTotal || Cell.IsColumnTotal ? Background.Light : Background.Default;

    private Task OnClicked( MouseEventArgs eventArgs )
        => PivotGrid.NotifyCellClicked( Cell, Row );

    /// <summary>
    /// Parent PivotGrid component.
    /// </summary>
    [CascadingParameter] public PivotGrid<TItem> PivotGrid { get; set; }

    /// <summary>
    /// Row associated with the cell.
    /// </summary>
    [Parameter] public PivotGridAxisItem<TItem> Row { get; set; }

    /// <summary>
    /// Cell to render.
    /// </summary>
    [Parameter] public PivotGridCell<TItem> Cell { get; set; }
}