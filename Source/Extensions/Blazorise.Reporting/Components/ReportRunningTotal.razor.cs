#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a stateful summary field that accumulates values while detail records are rendered.
/// </summary>
public partial class ReportRunningTotal : ComponentBase
{
    #region Methods

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        ReportContext?.RegisterRunningTotal( new()
        {
            Id = Id,
            Name = Name,
            DataSource = DataSource,
            Field = Field,
            Function = Function,
            EvaluateMode = EvaluateMode,
            EvaluateFormula = EvaluateFormula,
            ResetMode = ResetMode,
            ResetGroupId = ResetGroupId,
        }, ResetGroup );
    }

    #endregion

    #region Properties

    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <summary>
    /// Stable identifier used by persisted report state.
    /// </summary>
    [Parameter] public string Id { get; set; } = System.Guid.NewGuid().ToString( "N" );

    /// <summary>
    /// Running total field name shown in the field explorer and used by expressions.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Data source or path that provides the records being accumulated.
    /// </summary>
    [Parameter] public string DataSource { get; set; }

    /// <summary>
    /// Field path summarized by the running total.
    /// </summary>
    [Parameter] public string Field { get; set; }

    /// <summary>
    /// Summary operation used to update the running total value.
    /// </summary>
    [Parameter] public ReportAggregateFunction Function { get; set; } = ReportAggregateFunction.Sum;

    /// <summary>
    /// Determines whether every record or only formula-matching records are accumulated.
    /// </summary>
    [Parameter] public ReportRunningTotalEvaluateMode EvaluateMode { get; set; } = ReportRunningTotalEvaluateMode.EveryRecord;

    /// <summary>
    /// Boolean formula used when EvaluateMode is set to Formula.
    /// </summary>
    [Parameter] public string EvaluateFormula { get; set; }

    /// <summary>
    /// Determines when the accumulated value resets.
    /// </summary>
    [Parameter] public ReportRunningTotalResetMode ResetMode { get; set; } = ReportRunningTotalResetMode.Never;

    /// <summary>
    /// Stable group section identifier used when ResetMode is set to Group.
    /// </summary>
    [Parameter] public string ResetGroupId { get; set; }

    /// <summary>
    /// Group section name used when ResetMode is set to Group.
    /// </summary>
    [Parameter] public string ResetGroup { get; set; }

    #endregion
}