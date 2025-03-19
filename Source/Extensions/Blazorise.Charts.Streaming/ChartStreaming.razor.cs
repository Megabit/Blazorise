#region Using directives
using System;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Streaming;

public interface IChartStreaming
{
    Task Refresh();
}

/// <summary>
/// Provides the streaming capabilities to the supported chart types.
/// </summary>
/// <typeparam name="TItem">Data point type.</typeparam>
public partial class ChartStreaming<TItem> : ChartPlugin<TItem, JSChartStreamingModule>, IChartStreaming
{
    #region Members

    private DotNetObjectReference<ChartStreamingAdapter> dotNetObjectRef;

    #endregion

    #region Methods

    protected override JSChartStreamingModule GetNewJsModule()
    {
        return new JSChartStreamingModule( JSRuntime, VersionProvider, BlazoriseOptions );
    }

    protected override async Task InitializePluginByJsModule()
    {
        dotNetObjectRef ??= DotNetObjectReference.Create( new ChartStreamingAdapter( this ) );

        await JSModule.Initialize( dotNetObjectRef, ParentChart.ElementRef, ParentChart.ElementId, Vertical, Options );
    }

    protected override bool InitPluginInParameterSet( ParameterView parameterView )
    {
        //this plugin probably doesn't need re-initialization on param changes
        return false; 
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ParentChart.ElementRef, ParentChart.ElementId );

            await base.DisposeAsync( true );

            if ( dotNetObjectRef != null )
            {
                dotNetObjectRef.Dispose();
                dotNetObjectRef = null;
            }
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Refreshes the chart with new data.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public async Task Refresh()
    {
        if ( !Rendered || ParentChart?.Data?.Datasets is null)
            return;

        foreach ( var dataset in ParentChart.Data.Datasets )
        {
            var datasetIndex = ParentChart.Data.Datasets.IndexOf( dataset );

            var newData = new ChartStreamingData<TItem>( dataset.Label, datasetIndex );

            await Refreshed.InvokeAsync( newData );

            await JSModule.AddData( ParentChart.ElementId, newData.DatasetIndex, newData.Value );
        }
    }

    /// <summary>
    /// Pauses the current chart streaming.
    /// </summary>
    /// <param name="animate">If true the chart interpolate and animate from the last known data points.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Pause( bool animate = true )
    {
        if ( !Rendered )
            return;

        if ( Options is not null )
        {
            Options.Pause = true;
        }

        await JSModule.Pause( ParentChart.ElementId, animate );
    }

    /// <summary>
    /// Plays the current chart streaming.
    /// </summary>
    /// <param name="animate">If true the chart interpolate and animate from the last known data points.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Play( bool animate = true )
    {
        if ( !Rendered )
            return;

        if ( Options is not null )
        {
            Options.Pause = false;
        }

        await JSModule.Play( ParentChart.ElementId, animate );
    }

    #endregion

    #region Properties

    protected override JSChartStreamingModule JSModule { get;  set; }

  
    /// <summary>
    /// If true, chart will be set to vertical mode.
    /// </summary>
    [Parameter] public bool Vertical { get; set; }

    /// <summary>
    /// Defines the stream options.
    /// </summary>
    [Parameter] public ChartStreamingOptions Options { get; set; } = new();

    /// <summary>
    /// Callback function that will be called at a regular interval. The callback takes one argument, a reference to the dataset object. You can update your datasets here. The chart will be automatically updated after returning.
    /// </summary>
    [Parameter] public EventCallback<ChartStreamingData<TItem>> Refreshed { get; set; }

    #endregion

    protected override string Name => "Streaming";
}