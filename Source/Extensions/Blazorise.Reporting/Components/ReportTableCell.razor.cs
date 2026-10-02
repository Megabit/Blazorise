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

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var definitionChanged = RowContext is not null
            && ( parameters.IsParameterChanged( RowSpan ) || parameters.IsParameterChanged( ColumnSpan ) );

        await base.SetParametersAsync( parameters );

        if ( definitionChanged )
        {
            UpdateDefinition();
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();

        UpdateDefinition();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        RowContext?.UnregisterCell( this );
        RowContext = null;
    }

    private void UpdateDefinition()
    {
        var definition = RowContext?.RegisterCell( this, RowSpan, ColumnSpan );

        if ( definition is not null )
        {
            cellContext.Attach( RowContext, definition );
        }
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