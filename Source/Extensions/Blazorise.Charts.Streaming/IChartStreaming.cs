#region Using directives
using System.Threading.Tasks;
#endregion

namespace Blazorise.Charts.Streaming;

/// <summary>
/// Provides the streaming capabilities to the supported chart types.
/// </summary>
public interface IChartStreaming
{
    /// <summary>
    /// Refreshes the current state or data asynchronously.
    /// </summary>
    /// <returns>Returns a Task representing the asynchronous operation.</returns>
    Task Refresh();
}
