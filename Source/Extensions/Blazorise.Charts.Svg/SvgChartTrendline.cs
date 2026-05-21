#region Using directives
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Defines a trendline for a native SVG chart.
/// </summary>
public class SvgChartTrendline : SvgChartComponentBase
{
    #region Methods

    protected override void Register()
    {
        Parent?.RegisterTrendline( this );
        SetRegisteredParent();
    }

    protected override void Unregister()
    {
        RegisteredParent?.UnregisterTrendline( this );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines whether the trendline is visible.
    /// </summary>
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>
    /// Defines the source series name used to calculate the trendline.
    /// </summary>
    [Parameter] public string SeriesName { get; set; }

    /// <summary>
    /// Defines the trendline name.
    /// </summary>
    [Parameter] public string Name { get; set; }

    /// <summary>
    /// Defines the trendline calculation type.
    /// </summary>
    [Parameter] public SvgChartTrendlineType Type { get; set; } = SvgChartTrendlineType.Linear;

    /// <summary>
    /// Defines the trendline color. Use a Blazorise theme color, or pass a CSS color value such as <c>#4c6ef5</c>, <c>rgb(76, 110, 245)</c>, <c>hsl(228 88% 60%)</c>, or <c>var(--chart-color)</c>.
    /// </summary>
    [Parameter] public Color Color { get; set; }

    /// <summary>
    /// Defines the trendline stroke width.
    /// </summary>
    [Parameter] public double StrokeWidth { get; set; } = 2;

    /// <summary>
    /// Defines the trendline stroke dash pattern.
    /// </summary>
    [Parameter] public string DashPattern { get; set; } = "6 4";

    /// <summary>
    /// Defines the trendline opacity.
    /// </summary>
    [Parameter] public double Opacity { get; set; } = 0.85;

    /// <summary>
    /// Defines the trendline rendering order among other trendlines. Lower values are rendered first, behind higher values.
    /// </summary>
    [Parameter] public int? Order { get; set; }

    #endregion
}