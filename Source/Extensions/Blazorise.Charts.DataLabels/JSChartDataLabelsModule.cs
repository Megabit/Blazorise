#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Trendline;

public class JSChartDataLabelsModule : BaseJSModule
{
    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    public JSChartDataLabelsModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
    {
    }

    public virtual ValueTask SetDataLabels( string canvasId, object datasets, object options )
        => InvokeSafeVoidAsync( "setDataLabels", canvasId, datasets, options );

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Charts.DataLabels/chart.datalabels.js?v={VersionProvider.Version}";
}