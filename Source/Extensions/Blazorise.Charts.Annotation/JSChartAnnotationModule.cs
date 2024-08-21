#region Using directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Annotation;

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

    public virtual async ValueTask AddAnnotationOptions( string canvasId, Dictionary<string, ChartAnnotationOptions> annotationOptions )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "addAnnotation", canvasId, annotationOptions );
    }

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Charts.Annotation/chart.annotation.js?v={VersionProvider.Version}";
}
