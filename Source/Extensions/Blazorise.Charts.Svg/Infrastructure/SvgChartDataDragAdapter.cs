#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Svg;

internal sealed class SvgChartDataDragAdapter<TItem>
{
    #region Members

    private readonly SvgChart<TItem> chart;

    #endregion

    #region Constructors

    public SvgChartDataDragAdapter( SvgChart<TItem> chart )
    {
        this.chart = chart;
    }

    #endregion

    #region Methods

    [JSInvokable]
    public Task<bool> Start( int seriesIndex, int pointIndex )
    {
        return chart.HandleDataDragStart( seriesIndex, pointIndex );
    }

    [JSInvokable]
    public Task Move( double x, double y )
    {
        return chart.HandleDataDragMove( x, y );
    }

    [JSInvokable]
    public Task End( bool canceled )
    {
        return chart.HandleDataDragEnd( canceled );
    }

    [JSInvokable]
    public Task KeyDown( int seriesIndex, int pointIndex, string key, bool shiftKey )
    {
        return chart.HandleDataDragKeyDown( seriesIndex, pointIndex, key, shiftKey );
    }

    #endregion
}