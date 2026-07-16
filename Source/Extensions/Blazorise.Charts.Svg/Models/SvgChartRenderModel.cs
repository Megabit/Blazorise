#region Using directives
using System;
using System.Collections.Generic;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartRenderModel
{
    #region Properties

    public SvgChartType Type { get; init; }

    public SvgChartOptions Options { get; init; }

    public SvgChartZoomOptions Zoom { get; init; }

    public SvgChartViewport Viewport { get; init; }

    public List<object> Labels { get; init; } = [];

    public int CategorySlotCount { get; init; }

    public List<int> CategoryLabelIndexes { get; init; } = [];

    public double CategoryMin { get; init; }

    public double CategoryMax { get; init; }

    public SvgChartAxisScaleKind CategoryScaleKind { get; init; }

    public SvgChartScale CategoryScale { get; init; }

    public SvgChartAxisOptions CategoryAxis { get; init; }

    public Func<SvgChartAxisTickContext, string> CategoryTickFormatter { get; init; }

    public Func<SvgChartAxisTickContext, string> CategoryValueFormatter { get; init; }

    public List<SvgChartRenderSeries> Series { get; init; } = [];

    public double Min { get; init; }

    public double Max { get; init; }

    public List<double> Ticks { get; init; } = [];

    public List<SvgChartRenderValueAxis> ValueAxes { get; init; } = [];

    public SvgChartRenderValueAxis PrimaryValueAxis { get; init; }

    public SvgChartTooltipOptions Tooltip { get; init; }

    #endregion
}