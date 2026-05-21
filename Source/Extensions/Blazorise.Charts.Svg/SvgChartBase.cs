#region Using directives
using Blazorise;
#endregion

namespace Blazorise.Charts.Svg;

/// <summary>
/// Base class for all native SVG charts.
/// </summary>
public abstract class SvgChartBase : BaseComponent
{
    #region Methods

    internal abstract void RegisterSeries( object series );

    internal abstract void UnregisterSeries( object series );

    internal abstract void RegisterCategoryAxis( object axis );

    internal abstract void UnregisterCategoryAxis( object axis );

    internal abstract void RegisterValueAxis( SvgChartValueAxis axis );

    internal abstract void UnregisterValueAxis( SvgChartValueAxis axis );

    internal abstract void RegisterTitle( SvgChartTitle title );

    internal abstract void UnregisterTitle( SvgChartTitle title );

    internal abstract void RegisterLegend( SvgChartLegend legend );

    internal abstract void UnregisterLegend( SvgChartLegend legend );

    internal abstract void RegisterTooltip( SvgChartTooltip tooltip );

    internal abstract void UnregisterTooltip( SvgChartTooltip tooltip );

    internal abstract void RegisterDataLabels( SvgChartDataLabels dataLabels );

    internal abstract void UnregisterDataLabels( SvgChartDataLabels dataLabels );

    internal abstract void Refresh();

    #endregion
}