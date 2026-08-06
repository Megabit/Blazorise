#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
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

    private readonly PdfTableRowDefinition definition = new();

    private PdfTableContext tableContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override Task SetParametersAsync( ParameterView parameters )
    {
        bool heightChanged = parameters.IsParameterChanged( Height );
        Task task = base.SetParametersAsync( parameters );

        if ( heightChanged )
            definition.Height = Height;

        return task;
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        rowContext = new( definition );
    }

    /// <inheritdoc />
    public void Dispose()
    {
        tableContext?.Rows.Remove( definition );
        tableContext = null;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Provides the PDF table that receives this row definition.
    /// </summary>
    [CascadingParameter]
    protected PdfTableContext TableContext
    {
        get => tableContext;
        set
        {
            if ( ReferenceEquals( tableContext, value ) )
                return;

            tableContext?.Rows.Remove( definition );
            tableContext = value;

            if ( tableContext is not null && !tableContext.Rows.Contains( definition ) )
                tableContext.Rows.Add( definition );
        }
    }

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