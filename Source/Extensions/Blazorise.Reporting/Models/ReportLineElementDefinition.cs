#region Using directives
using System;
using System.Collections.Generic;
using Blazorise;
#endregion

namespace Blazorise.Reporting;

/// <summary>
/// Describes a line element placed on a report band.
/// </summary>
public sealed class ReportLineElementDefinition : ReportElementDefinition
{
    /// <inheritdoc />
    public override ReportElementType Type => ReportElementType.Line;

    /// <summary>
    /// Line stroke thickness in points.
    /// </summary>
    public double? Thickness { get; set; }
}