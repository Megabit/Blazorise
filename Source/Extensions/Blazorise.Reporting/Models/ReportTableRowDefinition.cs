using System;
using System.Collections.Generic;
using Blazorise;

namespace Blazorise.Reporting;

/// <summary>
/// Describes a row inside a report layout table element.
/// </summary>
public sealed class ReportTableRowDefinition
{
    /// <summary>
    /// Stable identifier used by persisted table state.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Row height in points.
    /// </summary>
    public double Height { get; set; } = 24;
}