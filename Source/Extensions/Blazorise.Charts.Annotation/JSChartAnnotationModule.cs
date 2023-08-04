#region Using directives
using System.Collections.Generic;
using System.Text.Json;
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
    public JSChartAnnotationModule( IJSRuntime jsRuntime, IVersionProvider versionProvider )
        : base( jsRuntime, versionProvider )
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
