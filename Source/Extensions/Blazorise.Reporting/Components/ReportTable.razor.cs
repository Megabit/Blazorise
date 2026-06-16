#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a table element used to organize report fields and text.
/// </summary>
public partial class ReportTable
{
    #region Methods

    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Table;

    /// <inheritdoc />
    protected override ReportElementDefinition BuildDefinition()
    {
        ReportElementDefinition definition = base.BuildDefinition();

        if ( ChildContent is null )
            Internal.ReportDefinitionHelper.EnsureTableLayout( definition, RowCount, ColumnCount );

        return definition;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Number of rows created when no explicit table rows or cells are declared.
    /// </summary>
    [Parameter] public int RowCount { get; set; } = 2;

    /// <summary>
    /// Number of columns created when no explicit table columns or cells are declared.
    /// </summary>
    [Parameter] public int ColumnCount { get; set; } = 2;

    /// <summary>
    /// Rows, cells, and nested report elements declared inside the report table.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}