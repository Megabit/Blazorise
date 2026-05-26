#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Renders a native SVG polar area chart.
/// </summary>
/// <typeparam name="TItem">The chart item type.</typeparam>
[CascadingTypeParameter( nameof( TItem ) )]
public class SvgPolarAreaChart<TItem> : SvgChart<TItem>
{
    #region Constructors

    public SvgPolarAreaChart()
    {
        Type = SvgChartType.PolarArea;
    }

    #endregion
}