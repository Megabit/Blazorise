#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a cell inside a report layout table element.
/// </summary>
public partial class ReportTableCell : ComponentBase
{
    #region Members

    private ReportTableCellContext cellContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        cellContext = null;

        if ( RowContext is null )
            return;

        ReportTableCellDefinition definition = RowContext.AddCell( RowSpan, ColumnSpan );
        cellContext = new( RowContext.TableDefinition, definition );
    }

    #endregion

    #region Properties

    [CascadingParameter] internal ReportTableRowContext RowContext { get; set; }

    /// <summary>
    /// Number of rows spanned by the cell.
    /// </summary>
    [Parameter] public int RowSpan { get; set; } = 1;

    /// <summary>
    /// Number of columns spanned by the cell.
    /// </summary>
    [Parameter] public int ColumnSpan { get; set; } = 1;

    /// <summary>
    /// Elements declared inside the table cell.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}