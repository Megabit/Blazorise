#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
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

    private readonly PdfTableCellDefinition definition = new();

    private PdfTableRowContext rowContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        bool widthChanged = parameters.IsParameterChanged( Width );
        Task task = base.SetParametersAsync( parameters );

        if ( widthChanged )
            definition.Width = Width;

        return task;
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        cellContext = new( definition );
    }

    /// <inheritdoc />
    public void Dispose()
    {
        rowContext?.Cells.Remove( definition );
        rowContext = null;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provides the PDF table row that receives this cell definition.
    /// </summary>
    [CascadingParameter]
    protected PdfTableRowContext RowContext
    {
        get => rowContext;
        set
        {
            if ( ReferenceEquals( rowContext, value ) )
                return;

            rowContext?.Cells.Remove( definition );
            rowContext = value;

            if ( rowContext is not null && !rowContext.Cells.Contains( definition ) )
                rowContext.Cells.Add( definition );
        }
    }

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