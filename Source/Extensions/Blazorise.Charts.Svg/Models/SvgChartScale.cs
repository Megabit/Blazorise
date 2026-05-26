#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartScale
{
    #region Properties

    public double Min { get; init; }

    public double Max { get; init; }

    public List<double> Ticks { get; init; } = [];

    #endregion
}