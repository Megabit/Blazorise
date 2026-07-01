using Microsoft.AspNetCore.Components;

namespace Blazorise.Reporting;

/// <summary>
/// Declares a line element in a report band.
/// </summary>
public partial class ReportLine : BaseReportElement
{
    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Line;

    /// <inheritdoc />
    protected override ReportElementDefinition BuildDefinition()
    {
        ReportElementDefinition definition = base.BuildDefinition();
        definition.Thickness = Thickness;

        return definition;
    }

    /// <summary>
    /// Line stroke thickness in points.
    /// </summary>
    [Parameter] public double? Thickness { get; set; }
}