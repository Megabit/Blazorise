#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes a table element placed on a report band.
/// </summary>
public sealed class ReportTableElementDefinition : ReportElementDefinition
{
    /// <inheritdoc />
    public override ReportElementType Type => ReportElementType.Table;

    /// <summary>
    /// Optional data source override for table content.
    /// </summary>
    public string DataSource { get; set; }

    /// <summary>
    /// Column definitions used by the table.
    /// </summary>
    public List<ReportTableColumnDefinition> Columns { get; set; } = [];

    /// <summary>
    /// Row definitions used by the table.
    /// </summary>
    public List<ReportTableRowDefinition> Rows { get; set; } = [];

    /// <summary>
    /// Cell definitions used by the table.
    /// </summary>
    public List<ReportTableCellDefinition> Cells { get; set; } = [];
}