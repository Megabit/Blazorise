#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Defines a table cell inside a PDF table row.
/// </summary>
public partial class PdfTableCell : ComponentBase
{
    #region Members

    private PdfTableCellContext cellContext;

    private PdfTableRowContext previousRowContext;

    private PdfTableCellDefinition previousDefinition;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( RowContext is null )
            return;

        if ( previousRowContext is not null && previousDefinition is not null )
            previousRowContext.Cells.Remove( previousDefinition );

        PdfTableCellDefinition definition = new()
        {
            Width = Width,
            ColumnSpan = ColumnSpan,
            RowSpan = RowSpan,
        };

        RowContext.Cells.Add( definition );

        cellContext = new( definition );
        previousDefinition = definition;
        previousRowContext = RowContext;
    }

    #endregion

    #region Parameters

    /// <summary>
    /// Provides the PDF table row that receives this cell definition.
    /// </summary>
    [CascadingParameter] protected PdfTableRowContext RowContext { get; set; }

    /// <summary>
    /// Cell width.
    /// </summary>
    [Parameter] public double Width { get; set; } = 90;

    /// <summary>
    /// Number of columns occupied by this cell.
    /// </summary>
    [Parameter] public int ColumnSpan { get; set; } = 1;

    /// <summary>
    /// Number of rows occupied by this cell.
    /// </summary>
    [Parameter] public int RowSpan { get; set; } = 1;

    /// <summary>
    /// Elements declared inside the cell.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}