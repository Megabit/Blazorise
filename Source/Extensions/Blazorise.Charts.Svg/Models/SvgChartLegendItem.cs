#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartLegendItem
{
    #region Properties

    public string Label { get; init; }

    public string Color { get; init; }

    public bool Hidden { get; init; }

    public Func<Task> Toggle { get; init; }

    #endregion
}