#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Defines a table row inside a PDF table.
/// </summary>
public partial class PdfTableRow : ComponentBase
{
    #region Members

    private PdfTableRowContext rowContext;

    private PdfTableContext previousTableContext;

    private PdfTableRowDefinition previousDefinition;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( TableContext is null )
            return;

        if ( previousTableContext is not null && previousDefinition is not null )
            previousTableContext.Rows.Remove( previousDefinition );

        PdfTableRowDefinition definition = new()
        {
            Height = Height,
        };

        TableContext.Rows.Add( definition );

        rowContext = new( definition );
        previousDefinition = definition;
        previousTableContext = TableContext;
    }

    #endregion

    #region Parameters

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