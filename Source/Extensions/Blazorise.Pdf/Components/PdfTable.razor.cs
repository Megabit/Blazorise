#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Pdf;

/// <summary>
/// Defines a table layout in a PDF document.
/// </summary>
public partial class PdfTable : BasePdfElement
{
    #region Members

    private PdfTableContext tableContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if ( tableContext is null && Definition is not null )
            tableContext = new( Definition );
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override PdfElementType ElementType => PdfElementType.Table;

    /// <summary>
    /// Rows declared inside the table.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}