#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Declares a line element in a report band.
/// </summary>
public partial class ReportLine : BaseReportElement
{
    #region Methods

    /// <inheritdoc />
    protected override ReportElementDefinition BuildDefinition()
    {
        ReportLineElementDefinition definition = (ReportLineElementDefinition)base.BuildDefinition();
        definition.Thickness = Thickness;
        definition.Orientation = Orientation;

        return definition;
    }

    #endregion

    #region Properties

    /// <inheritdoc />
    protected override ReportElementType ElementType => ReportElementType.Line;

    /// <summary>
    /// Line stroke thickness in points.
    /// </summary>
    [Parameter] public double? Thickness { get; set; }

    /// <summary>
    /// Line orientation within the element bounds.
    /// </summary>
    [Parameter] public Orientation Orientation { get; set; } = Orientation.Horizontal;

    #endregion
}