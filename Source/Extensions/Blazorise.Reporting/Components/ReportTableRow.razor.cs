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

    private ReportTableContext registeredTableContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool definitionChanged = registeredTableContext is null || parameters.IsParameterChanged( Height );

        await base.SetParametersAsync( parameters );

        bool contextChanged = !ReferenceEquals( registeredTableContext, TableContext );

        if ( contextChanged )
        {
            DetachRow();
            registeredTableContext = TableContext;

            if ( registeredTableContext?.Definition is not null )
            {
                registeredTableContext.Definition.Rows.Add( definition );
                rowContext.Attach( registeredTableContext, definition );
            }
        }

        if ( definitionChanged )
        {
            definition.Height = Height;
            registeredTableContext?.NotifyDefinitionChanged();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        DetachRow();
        registeredTableContext = null;
    }

    private void DetachRow()
    {
        ReportTableElementDefinition tableDefinition = registeredTableContext?.Definition;

        if ( tableDefinition is null )
            return;

        int rowIndex = tableDefinition.Rows.IndexOf( definition );
        rowContext.Detach();

        if ( rowIndex < 0 )
            return;

        tableDefinition.Rows.RemoveAt( rowIndex );

        foreach ( ReportTableCellDefinition cell in tableDefinition.Cells )
        {
            if ( cell.RowIndex > rowIndex )
                cell.RowIndex--;
        }

        registeredTableContext.NotifyDefinitionChanged();
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