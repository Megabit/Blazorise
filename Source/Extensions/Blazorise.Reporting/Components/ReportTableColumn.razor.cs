#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a column inside a report table element.
/// </summary>
public partial class ReportTableColumn : ComponentBase
{
    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if ( TableDefinition is null )
            return;

        TableDefinition.Columns.Add( new()
        {
            Title = Title,
            Field = Field,
            Format = Format,
            Width = Width,
        } );
    }

    #endregion

    #region Properties

    [CascadingParameter] internal ReportTableElementDefinition TableDefinition { get; set; }

    /// <summary>
    /// Header text displayed for the table column.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Field name rendered by cells in this column.
    /// </summary>
    [Parameter] public string Field { get; set; }

    /// <summary>
    /// Format applied to column values.
    /// </summary>
    [Parameter] public ReportFormatDefinition Format { get; set; }

    /// <summary>
    /// Column width in points.
    /// </summary>
    [Parameter] public double Width { get; set; } = 90;

    #endregion
}