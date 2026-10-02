#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a row inside a report layout table element.
/// </summary>
public partial class ReportTableRow : ComponentBase, IDisposable
{
    #region Members

    private readonly ReportTableRowDefinition definition = new();

    private readonly ReportTableRowContext rowContext = new();

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var definitionChanged = TableContext is not null && parameters.IsParameterChanged( Height );

        await base.SetParametersAsync( parameters );

        if ( definitionChanged )
        {
            definition.Height = Height;
            TableContext?.NotifyDefinitionChanged();
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        base.OnInitialized();

        if ( TableContext?.Definition is not null )
        {
            TableContext.Definition.Rows.Add( definition );
            rowContext.Attach( TableContext, definition );
        }

        definition.Height = Height;
        TableContext?.NotifyDefinitionChanged();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        DetachRow();
        TableContext = null;
    }

    private void DetachRow()
    {
        var tableDefinition = TableContext?.Definition;

        if ( tableDefinition is null )
        {
            return;
        }

        var rowIndex = tableDefinition.Rows.IndexOf( definition );
        rowContext.Detach();

        if ( rowIndex < 0 )
        {
            return;
        }

        tableDefinition.Rows.RemoveAt( rowIndex );

        foreach ( var cell in tableDefinition.Cells )
        {
            if ( cell.RowIndex > rowIndex )
            {
                cell.RowIndex--;
            }
        }

        TableContext.NotifyDefinitionChanged();
    }

    #endregion

    #region Properties

    [CascadingParameter] internal ReportTableContext TableContext { get; set; }

    /// <summary>
    /// Row height in points.
    /// </summary>
    [Parameter] public double Height { get; set; } = 24;

    /// <summary>
    /// Cells declared inside the table row.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}