#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Interface is needed to set the value from javascript because calling generic component directly is not supported by Blazor.
/// </summary>
public interface IBaseChart
{
    /// <summary>
    /// Handles an event raised by the JavaScript chart implementation.
    /// </summary>
    /// <param name="eventName">The event name.</param>
    /// <param name="datasetIndex">The dataset index.</param>
    /// <param name="index">The data point index.</param>
    /// <param name="model">The serialized event model.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Event( string eventName, int datasetIndex, int index, string model );
}