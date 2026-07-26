#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a row inside a report layout table element.
/// </summary>
public partial class ReportTableRow : ComponentBase
{
    #region Members

    private ReportTableRowContext rowContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        rowContext = null;

        if ( TableDefinition is null )
            return;

        int rowIndex = TableDefinition.Rows.Count;

        TableDefinition.Rows.Add( new()
        {
            Height = Height,
        } );

        rowContext = new( TableDefinition, rowIndex );
    }

    #endregion

    #region Properties

    [CascadingParameter] internal ReportTableElementDefinition TableDefinition { get; set; }

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