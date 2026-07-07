using System;
using System.Collections.Generic;
using Blazorise;

namespace Blazorise.Reporting;

/// <summary>
/// Describes a column inside a report table element.
/// </summary>
public sealed class ReportTableColumnDefinition
{
    /// <summary>
    /// Stable identifier used by persisted table state.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Header text displayed for the column.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Data field rendered in the column.
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Format applied to column values.
    /// </summary>
    public ReportFormatDefinition Format { get; set; }

    /// <summary>
    /// Column width in points.
    /// </summary>
    public double Width { get; set; } = 90;
}