#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartRenderValueAxis
{
    #region Properties

    public string Id { get; init; }

    public SvgChartAxisPosition Position { get; init; }

    public SvgChartGridLinesOptions GridLines { get; init; }

    public double Min { get; init; }

    public double Max { get; init; }

    public List<double> Ticks { get; init; } = [];

    #endregion
}