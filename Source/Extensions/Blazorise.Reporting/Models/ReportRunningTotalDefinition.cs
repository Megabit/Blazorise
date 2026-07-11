#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes a stateful summary field that accumulates while report detail records are rendered.
/// </summary>
public sealed class ReportRunningTotalDefinition
{
    /// <summary>
    /// Stable identifier used by persisted report state.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Running total field name shown in the field explorer and used by expressions.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Data source or path that provides the records being accumulated.
    /// </summary>
    public string DataSource { get; set; }

    /// <summary>
    /// Field path summarized by the running total.
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Summary operation used to update the running total value.
    /// </summary>
    public ReportAggregateFunction AggregateFunction { get; set; } = ReportAggregateFunction.Sum;

    /// <summary>
    /// Determines whether every record or only formula-matching records are accumulated.
    /// </summary>
    public ReportRunningTotalEvaluateMode EvaluateMode { get; set; } = ReportRunningTotalEvaluateMode.EveryRecord;

    /// <summary>
    /// Boolean formula used when EvaluateMode is set to Formula.
    /// </summary>
    public string EvaluateFormula { get; set; }

    /// <summary>
    /// Determines when the accumulated value resets.
    /// </summary>
    public ReportRunningTotalResetMode ResetMode { get; set; } = ReportRunningTotalResetMode.Never;

    /// <summary>
    /// Group section identifier used when ResetMode is set to Group.
    /// </summary>
    public string ResetGroupId { get; set; }
}