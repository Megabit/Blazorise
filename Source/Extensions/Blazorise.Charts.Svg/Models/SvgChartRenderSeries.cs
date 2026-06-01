#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartRenderSeries
{
    #region Properties

    public string Name { get; init; }

    public SvgChartType Type { get; init; }

    public List<double?> Values { get; init; } = [];

    public List<double?> XValues { get; init; } = [];

    public List<double?> YValues { get; init; } = [];

    public List<double?> RadiusValues { get; init; } = [];

    public Color Color { get; init; }

    public string RenderColor { get; init; }

    public List<string> PointColors { get; init; } = [];

    public bool Hidden { get; init; }

    public int? Order { get; init; }

    public string CategoryAxisId { get; init; }

    public string ValueAxisId { get; init; }

    public string Stack { get; init; }

    public List<double?> StackBaseValues { get; init; } = [];

    public List<double?> StackEndValues { get; init; } = [];

    public double BorderRadius { get; init; }

    public double StrokeWidth { get; init; }

    public double MarkerRadius { get; init; }

    public double FillOpacity { get; init; }

    public SvgChartInterpolationMode Interpolation { get; init; }

    public double Tension { get; init; } = 0.4;

    #endregion
}