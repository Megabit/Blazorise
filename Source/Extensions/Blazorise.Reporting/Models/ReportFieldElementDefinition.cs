using System;
using System.Collections.Generic;
using Blazorise;

namespace Blazorise.Reporting;

/// <summary>
/// Describes a data-bound field element placed on a report band.
/// </summary>
public sealed class ReportFieldElementDefinition : ReportElementDefinition
{
    /// <inheritdoc />
    public override ReportElementType Type => ReportElementType.Field;

    /// <summary>
    /// Data field expression rendered by the element.
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Format applied to resolved field values.
    /// </summary>
    public ReportFormatDefinition Format { get; set; }

    /// <summary>
    /// Optional data source override for field content.
    /// </summary>
    public string DataSource { get; set; }

    /// <summary>
    /// Aggregate function applied when the field is rendered as a summary value.
    /// </summary>
    public ReportAggregateDefinition Aggregate { get; set; }
}