#region Using directives
using System;
using System.Threading.Tasks;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// An abstract class for chart plugins that require initialization with a parent chart. It includes a property for the
/// plugin's name.
/// </summary>
public abstract class ChartPlugin : BaseComponent, IAsyncDisposable
{
    /// <summary>
    /// An abstract method that is called when the parent chart is initialized. It is intended to be overridden in
    /// derived classes.
    /// </summary>
    /// <returns>Returns a Task representing the asynchronous operation.</returns>
    protected internal abstract Task OnParentChartInitialized();

    /// <summary>
    /// Defines the name of the plugin.
    /// </summary>
    protected internal abstract string Name { get; }
}