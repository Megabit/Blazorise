#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a data-bound field element in a report band.
/// </summary>
public partial class ReportField : BaseReportTextElement
{
    #region Methods

    /// <inheritdoc />
    protected override ReportElementDefinition BuildDefinition()
    {
        ReportFieldElementDefinition definition = (ReportFieldElementDefinition)base.BuildDefinition();
        definition.Field = Field;
        definition.Format = Format;
        definition.Aggregate = AggregateFunction is null
            ? null
            : new()
            {
                Function = AggregateFunction.Value,
            };

        return definition;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Field;

    /// <summary>
    /// Field name or expression resolved against the current data item.
    /// </summary>
    [Parameter] public string Field { get; set; }

    /// <summary>
    /// Format applied to the resolved field value.
    /// </summary>
    [Parameter] public ReportFormatDefinition Format { get; set; }

    /// <summary>
    /// Aggregate function applied when this field is rendered as a summary.
    /// </summary>
    [Parameter] public ReportAggregateFunction? AggregateFunction { get; set; }

    #endregion
}