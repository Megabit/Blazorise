#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.DataLabels;

/// <summary>
/// Sends dataset-label configuration to the Chart.js data-labels plugin.
/// </summary>
public class JSChartDataLabelsModule : BaseJSModule
{
    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSChartDataLabelsModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    /// <summary>
    /// Updates label settings for the chart rendered on a canvas.
    /// </summary>
    public virtual ValueTask SetDataLabels( string canvasId, object datasets, ChartDataLabelsOptions options )
        => InvokeSafeVoidAsync( "setDataLabels", canvasId, datasets, options );

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Charts.DataLabels/chart.datalabels.js?v={VersionProvider.Version}";
}