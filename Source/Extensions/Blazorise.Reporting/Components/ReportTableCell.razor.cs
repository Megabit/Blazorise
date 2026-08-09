#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a cell inside a report layout table element.
/// </summary>
public partial class ReportTableCell : ComponentBase, IDisposable
{
    #region Members

    private readonly ReportTableCellContext cellContext = new();

    private ReportTableRowContext registeredRowContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool definitionChanged = registeredRowContext is null
            || parameters.IsParameterChanged( RowSpan )
            || parameters.IsParameterChanged( ColumnSpan );

        await base.SetParametersAsync( parameters );

        bool contextChanged = !ReferenceEquals( registeredRowContext, RowContext );

        if ( contextChanged )
        {
            registeredRowContext?.UnregisterCell( this );
            registeredRowContext = RowContext;
        }

        if ( definitionChanged || contextChanged )
        {
            ReportTableCellDefinition definition = registeredRowContext?.RegisterCell( this, RowSpan, ColumnSpan );

            if ( definition is not null )
                cellContext.Attach( registeredRowContext, definition );
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        registeredRowContext?.UnregisterCell( this );
        registeredRowContext = null;
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