#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Annotation;

/// <summary>
/// Transfers annotation collections to the Chart.js annotation plugin.
/// </summary>
public class JSChartAnnotationModule : BaseJSModule
{
    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSChartAnnotationModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    /// <summary>
    /// Replaces the named annotations configured for a chart canvas.
    /// </summary>
    public virtual async ValueTask AddAnnotationOptions( string canvasId, Dictionary<string, ChartAnnotationOptions> annotationOptions )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "addAnnotation", canvasId, annotationOptions );
    }

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Charts.Annotation/chart.annotation.js?v={VersionProvider.Version}";
}