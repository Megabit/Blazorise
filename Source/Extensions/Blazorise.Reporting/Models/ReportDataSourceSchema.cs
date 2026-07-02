using System;
using System.Collections.Generic;

namespace Blazorise.Reporting;

/// <summary>
/// Describes the fields exposed by a report data source.
/// </summary>
public sealed class ReportDataSourceSchema
{
    #region Properties

    /// <summary>
    /// Indicates that the data source resolves to a repeatable sequence.
    /// </summary>
    public bool IsCollection { get; set; }

    /// <summary>
    /// Fields exposed by the data source.
    /// </summary>
    public List<ReportDataSourceSchemaField> Fields { get; set; } = [];

    #endregion
}