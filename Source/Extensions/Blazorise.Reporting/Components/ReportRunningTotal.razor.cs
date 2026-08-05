#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a stateful summary field that accumulates values while detail records are rendered.
/// </summary>
public partial class ReportRunningTotal : ComponentBase, IDisposable
{
    #region Members

    private ReportContext registeredReportContext;

    #endregion

    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        bool definitionChanged = registeredReportContext is null
            || parameters.IsParameterChanged( Id )
            || parameters.IsParameterChanged( Name )
            || parameters.IsParameterChanged( DataSource )
            || parameters.IsParameterChanged( Field )
            || parameters.IsParameterChanged( AggregateFunction )
            || parameters.IsParameterChanged( EvaluateMode )
            || parameters.IsParameterChanged( EvaluateFormula )
            || parameters.IsParameterChanged( ResetMode )
            || parameters.IsParameterChanged( ResetGroupId );

        await base.SetParametersAsync( parameters );

        bool contextChanged = !ReferenceEquals( registeredReportContext, ReportContext );

        if ( contextChanged )
        {
            registeredReportContext?.UnregisterRunningTotal( this );
            registeredReportContext = ReportContext;
        }

        if ( definitionChanged || contextChanged )
        {
            registeredReportContext?.RegisterRunningTotal( this, new()
            {
                Id = Id,
                Name = Name,
                DataSource = DataSource,
                Field = Field,
                AggregateFunction = AggregateFunction,
                EvaluateMode = EvaluateMode,
                EvaluateFormula = EvaluateFormula,
                ResetMode = ResetMode,
                ResetGroupId = ResetGroupId,
            } );
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        registeredReportContext?.UnregisterRunningTotal( this );
        registeredReportContext = null;
    }

    #endregion

    #region Properties

    [CascadingParameter] internal ReportContext ReportContext { get; set; }

    /// <summary>
    /// Stable identifier used by persisted report definitions.
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
    [Parameter] public ReportAggregateFunction AggregateFunction { get; set; } = ReportAggregateFunction.Sum;

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

    #endregion
}