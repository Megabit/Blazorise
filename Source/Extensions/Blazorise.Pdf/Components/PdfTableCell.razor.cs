#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Defines a table cell inside a PDF table row.
/// </summary>
public partial class PdfTableCell : ComponentBase, IDisposable
{
    #region Members

    private PdfTableCellContext cellContext;

    private PdfTableCellDefinition definition;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( definition is null )
        {
            if ( RowContext is null )
                return;

            RowContext.Cells.Add( definition = new() );
            cellContext = new( definition );
        }

        definition.Width = Width;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if ( RowContext is not null && definition is not null )
            RowContext.Cells.Remove( definition );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provides the PDF table row that receives this cell definition.
    /// </summary>
    [CascadingParameter] protected PdfTableRowContext RowContext { get; set; }

    /// <summary>
    /// Cell width.
    /// </summary>
    [Parameter] public double Width { get; set; } = 90;

    /// <summary>
    /// Elements declared inside the cell.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}