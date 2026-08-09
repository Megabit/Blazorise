#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Modules;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Supports js chart module behavior in chart components.
/// </summary>
public class JSChartModule : BaseJSModule,
    IJSDestroyableModule
{
    #region Constructors

    /// <summary>
    /// Default module constructor.
    /// </summary>
    /// <param name="jsRuntime">JavaScript runtime instance.</param>
    /// <param name="versionProvider">Version provider.</param>
    /// <param name="options">Blazorise options.</param>
    public JSChartModule( IJSRuntime jsRuntime, IVersionProvider versionProvider, BlazoriseOptions options )
        : base( jsRuntime, versionProvider, options )
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initializes browser integration for the js chart module.
    /// </summary>
    public virtual async ValueTask Initialize<TItem, TOptions>( DotNetObjectReference<ChartAdapter> dotNetObjectReference, object eventOptions, ElementReference canvasRef, string canvasId, ChartType type, ChartData<TItem> data, TOptions options, string dataJsonString, string optionsJsonString, object optionsObject, IReadOnlyList<string> pluginNames )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "initialize",
            dotNetObjectReference,
            eventOptions,
            canvasRef,
            canvasId,
            ToChartTypeString( type ),
            ToChartData( data ),
            (object)options,
            dataJsonString,
            optionsJsonString,
            optionsObject,
            pluginNames );
    }

    /// <summary>
    /// Changes the rendered chart to another visualization type.
    /// </summary>
    public virtual async ValueTask ChangeType( ElementReference canvasRef, string canvasId, ChartType type )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "changeChartType",
            canvasRef,
            canvasId,
            ToChartTypeString( type ) );
    }

    /// <summary>
    /// Releases resources held by the js chart module.
    /// </summary>
    public virtual async ValueTask Destroy( ElementReference canvasRef, string canvasId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "destroy", canvasRef, canvasId );
    }

    /// <summary>
    /// Updates options.
    /// </summary>
    public virtual async ValueTask SetOptions<TOptions>( string canvasId, TOptions options, string optionsJsonString, object optionsObject )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "setOptions", canvasId, options, optionsJsonString, optionsObject );
    }

    /// <summary>
    /// Refreshes the chart with its current data and options.
    /// </summary>
    public virtual async ValueTask Update( string canvasId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "update", canvasId );
    }

    /// <summary>
    /// Clears all data from the rendered chart.
    /// </summary>
    public virtual async ValueTask Clear( string canvasId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "clear", canvasId );
    }

    /// <summary>
    /// Adds label.
    /// </summary>
    public virtual async ValueTask AddLabel( string canvasId, IReadOnlyCollection<object> newLabels )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "addLabel", canvasId, newLabels );
    }

    /// <summary>
    /// Adds data set.
    /// </summary>
    public virtual async ValueTask AddDataSet( string canvasId, IReadOnlyCollection<object> newDataSet )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "addDataset", canvasId, ToChartDataSet( newDataSet ) );
    }

    /// <summary>
    /// Removes data set.
    /// </summary>
    public virtual async ValueTask RemoveDataSet( string canvasId, int dataSetIndex )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "removeDataset", canvasId, dataSetIndex );
    }

    /// <summary>
    /// Adds datasets and update.
    /// </summary>
    public virtual async ValueTask AddDatasetsAndUpdate( string canvasId, IReadOnlyCollection<object> newDataSet )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "addDatasetsAndUpdate", canvasId, ToChartDataSet( newDataSet ) );
    }

    /// <summary>
    /// Adds labels datasets and update.
    /// </summary>
    public virtual async ValueTask AddLabelsDatasetsAndUpdate( string canvasId, IReadOnlyCollection<object> newLabels, IReadOnlyCollection<object> newDataSet )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "addLabelsDatasetsAndUpdate", canvasId, newLabels, ToChartDataSet( newDataSet ) );
    }

    /// <summary>
    /// Updates data.
    /// </summary>
    public virtual async ValueTask SetData<TItem>( string canvasId, int dataSetIndex, IReadOnlyCollection<TItem> newData )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "setData", canvasId, dataSetIndex, newData );
    }

    /// <summary>
    /// Adds data.
    /// </summary>
    public virtual async ValueTask AddData<TItem>( string canvasId, int dataSetIndex, IReadOnlyCollection<TItem> newData )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "addData", canvasId, dataSetIndex, newData );
    }

    /// <summary>
    /// Removes the first label from the chart.
    /// </summary>
    public virtual async ValueTask ShiftLabel( string canvasId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "shiftLabel", canvasId );
    }

    /// <summary>
    /// Removes the first data point from the chart.
    /// </summary>
    public virtual async ValueTask ShiftData( string canvasId, int dataSetIndex )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "shiftData", canvasId, dataSetIndex );
    }

    /// <summary>
    /// Removes the last label from the chart.
    /// </summary>
    public virtual async ValueTask PopLabel( string canvasId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "popLabel", canvasId );
    }

    /// <summary>
    /// Removes the last data point from the chart.
    /// </summary>
    public virtual async ValueTask PopData( string canvasId, int dataSetIndex )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "popData", canvasId, dataSetIndex );
    }

    /// <summary>
    /// Resizes the chart to fit its container.
    /// </summary>
    public virtual async ValueTask Resize( string canvasId )
    {
        var moduleInstance = await Module;

        await moduleInstance.InvokeVoidAsync( "resize", canvasId );
    }

    /// <summary>
    /// Returns the JavaScript identifier for a chart type.
    /// </summary>
    public static string ToChartTypeString( ChartType type )
    {
        return type switch
        {
            ChartType.Bar => "bar",
            ChartType.Pie => "pie",
            ChartType.Doughnut => "doughnut",
            ChartType.Radar => "radar",
            ChartType.PolarArea => "polarArea",
            ChartType.Scatter => "scatter",
            ChartType.Bubble => "bubble",
            _ => "line",
        };
    }

    private static object ToChartData<T>( ChartData<T> data )
    {
        return new
        {
            data?.Labels,
            Datasets = data?.Datasets?.ToList<object>()
        };
    }

    private static object ToChartDataSet( IReadOnlyCollection<object> dataSet )
    {
        return dataSet?.ToList<object>();
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public override string ModuleFileName => $"./_content/Blazorise.Charts/charts.js?v={VersionProvider.Version}";

    #endregion
}