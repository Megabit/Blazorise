#region Using directives
using Blazorise.Extensions;
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
    protected override bool HasDefinitionChanged( ParameterView parameters )
    {
        return base.HasDefinitionChanged( parameters )
            || parameters.IsParameterChanged( Thickness )
            || parameters.IsParameterChanged( Orientation );
    }

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