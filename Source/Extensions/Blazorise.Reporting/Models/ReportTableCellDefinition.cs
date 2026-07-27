#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes a cell inside a report layout table element.
/// </summary>
public sealed class ReportTableCellDefinition
{
    /// <summary>
    /// Stable identifier used by persisted table state.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Zero-based row index occupied by the cell.
    /// </summary>
    public int RowIndex { get; set; }

    /// <summary>
    /// Zero-based column index occupied by the cell.
    /// </summary>
    public int ColumnIndex { get; set; }

    /// <summary>
    /// Number of rows spanned by the cell.
    /// </summary>
    public int RowSpan { get; set; } = 1;

    /// <summary>
    /// Number of columns spanned by the cell.
    /// </summary>
    public int ColumnSpan { get; set; } = 1;

    /// <summary>
    /// Elements placed inside the table cell.
    /// </summary>
    public List<ReportElementDefinition> Elements { get; set; } = [];
}