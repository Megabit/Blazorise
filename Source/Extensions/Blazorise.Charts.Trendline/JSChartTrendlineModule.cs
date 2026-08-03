#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Trendline;

/// <summary>
/// Connects trendline configuration to the Chart.js browser plugin.
/// </summary>
public class JSChartTrendlineModule : BaseJSModule
{
    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSChartTrendlineModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    /// <summary>
    /// Applies trendline settings to the chart rendered on the specified canvas.
    /// </summary>
    /// <param name="canvasId">Identifier of the chart canvas.</param>
    /// <param name="trendlineData">Per-dataset trendline settings.</param>
    /// <returns>Whether the browser plugin accepted the settings.</returns>
    public virtual async ValueTask<bool> AddTrendlineOptions( string canvasId, List<ChartTrendlineData> trendlineData )
    {
        var moduleInstance = await Module;

        return await moduleInstance.InvokeAsync<bool>( "addTrendlines", canvasId, trendlineData );
    }

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Charts.Trendline/charts.trendline.js?v={VersionProvider.Version}";
}