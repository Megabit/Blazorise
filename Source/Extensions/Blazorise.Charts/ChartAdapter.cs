#region Using directives
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Routes browser chart events to the owning component.
/// </summary>
public class ChartAdapter
{
    private readonly IBaseChart chart;

    /// <summary>
    /// Creates a chart adapter instance.
    /// </summary>
    public ChartAdapter( IBaseChart chart )
    {
        this.chart = chart;
    }

    /// <summary>
    /// Forwards a chart event to its component.
    /// </summary>
    [JSInvokable]
    public Task Event( string eventName, int datasetIndex, int index, string model )
    {
        return chart.Event( eventName, datasetIndex, index, model );
    }
}