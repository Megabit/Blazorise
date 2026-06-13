#region Using directives
using System;
using System.Collections.Generic;
#endregion

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

/// <summary>
/// Describes a field inside a report data source schema.
/// </summary>
public sealed class ReportDataSourceSchemaField
{
    #region Properties

    /// <summary>
    /// Field name used by report expressions.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// User-facing field name shown by designer surfaces.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Runtime field value type when the provider can determine it.
    /// </summary>
    public Type DataType { get; set; }

    /// <summary>
    /// Indicates that the field resolves to a repeatable sequence.
    /// </summary>
    public bool IsCollection { get; set; }

    /// <summary>
    /// Child fields exposed by nested object or collection values.
    /// </summary>
    public List<ReportDataSourceSchemaField> Fields { get; set; } = [];

    #endregion
}