#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Trendline;

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

    public virtual async ValueTask<bool> AddTrendlineOptions( string canvasId, List<ChartTrendlineData> trendlineData )
    {
        var moduleInstance = await Module;

        return await moduleInstance.InvokeAsync<bool>( "addTrendlines", canvasId, trendlineData );
    }

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Charts.Trendline/charts.trendline.js?v={VersionProvider.Version}";
}