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
public partial class ChartStreaming<TItem> : BaseComponent, IChartStreaming, IAsyncDisposable
{
    #region Members

    private const string PluginName = "Streaming";

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if ( ParentChart is not null )
        {
            ParentChart.Initialized += OnParentChartInitialized;

            ParentChart.NotifyPluginInitialized( PluginName );
        }

        return base.OnInitializedAsync();
    }

    private async void OnParentChartInitialized( object sender, EventArgs e )
    {
        if ( JSModule == null )
        {
            JSModule = new JSChartStreamingModule( JSRuntime, VersionProvider, BlazoriseOptions );

            ExecuteAfterRender( async () =>
            {
                DotNetObjectRef ??= DotNetObjectReference.Create( new ChartStreamingAdapter( this ) );

                await JSModule.Initialize( DotNetObjectRef, ParentChart.ElementRef, ParentChart.ElementId, Vertical, Options );
            } );

            await InvokeAsync( StateHasChanged );
        }
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ParentChart.ElementRef, ParentChart.ElementId );

            await JSModule.SafeDisposeAsync();

            if ( DotNetObjectRef != null )
            {
                DotNetObjectRef.Dispose();
                DotNetObjectRef = null;
            }

            if ( ParentChart is not null )
            {
                ParentChart.Initialized -= OnParentChartInitialized;

                ParentChart.NotifyPluginRemoved( PluginName );
            }
        }

        await base.DisposeAsync( disposing );
    }

    public async Task Refresh()
    {
        if ( !Rendered )
            return;

        foreach ( var dataset in ParentChart?.Data?.Datasets ?? Enumerable.Empty<ChartDataset<TItem>>() )
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

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    protected DotNetObjectReference<ChartStreamingAdapter> DotNetObjectRef { get; set; }

    protected JSChartStreamingModule JSModule { get; private set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    [CascadingParameter] protected BaseChart<TItem> ParentChart { get; set; }

    /// <summary>
    /// If true, chart will be set to vertical mode.
    /// </summary>
    [Parameter] public bool Vertical { get; set; }

    /// <summary>
    /// Stream options.
    /// </summary>
    [Parameter] public ChartStreamingOptions Options { get; set; } = new();

    /// <summary>
    /// Callback function that will be called at a regular interval. The callback takes one argument, a reference to the dataset object. You can update your datasets here. The chart will be automatically updated after returning.
    /// </summary>
    [Parameter] public EventCallback<ChartStreamingData<TItem>> Refreshed { get; set; }

    #endregion
}