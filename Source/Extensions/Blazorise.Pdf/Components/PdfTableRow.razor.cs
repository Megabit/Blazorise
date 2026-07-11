#region Using directives
using System;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Defines a table row inside a PDF table.
/// </summary>
public partial class PdfTableRow : ComponentBase, IDisposable
{
    #region Members

    private PdfTableRowContext rowContext;

    private PdfTableRowDefinition definition;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( definition is null )
        {
            if ( TableContext is null )
                return;

            TableContext.Rows.Add( definition = new() );
            rowContext = new( definition );
        }

        definition.Height = Height;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if ( TableContext is not null && definition is not null )
            TableContext.Rows.Remove( definition );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provides the PDF table that receives this row definition.
    /// </summary>
    [CascadingParameter] protected PdfTableContext TableContext { get; set; }

    /// <summary>
    /// Row height.
    /// </summary>
    [Parameter] public double Height { get; set; } = 24;

    /// <summary>
    /// Cells declared inside the row.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}