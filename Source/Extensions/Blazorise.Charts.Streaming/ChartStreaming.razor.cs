#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Streaming
{
    public interface IChartStreaming
    {
        Task Refresh();
    }

    /// <summary>
    /// Provides the streaming capabilities to the supported chart types.
    /// </summary>
    /// <typeparam name="TItem">Data point type.</typeparam>
    public partial class ChartStreaming<TItem> : BaseComponent, IChartStreaming
    {
        #region Methods

        protected override Task OnInitializedAsync()
        {
            if ( JSModule == null )
            {
                JSModule = new JSChartStreamingModule( JSRuntime );
            }

            return base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender && ParentChart != null )
            {
                DotNetObjectRef ??= DotNetObjectReference.Create( new ChartStreamingAdapter( this ) );

                await JSModule.Initialize( DotNetObjectRef, ParentChart.ElementRef, ParentChart.ElementId, Vertical, Options );
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        protected override async ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing && Rendered )
            {
                var jsModuleDestroyTask = JSModule.Destroy( ParentChart.ElementRef, ParentChart.ElementId );

                try
                {
                    await jsModuleDestroyTask;
                }
                catch when ( jsModuleDestroyTask.IsCanceled )
                {
                }
#if NET6_0_OR_GREATER
                catch ( Microsoft.JSInterop.JSDisconnectedException )
                {
                }
#endif

                var jsModuleDisposeTask = JSModule.DisposeAsync();

                try
                {
                    await jsModuleDisposeTask;
                }
                catch when ( jsModuleDisposeTask.IsCanceled )
                {
                }
#if NET6_0_OR_GREATER
                catch ( Microsoft.JSInterop.JSDisconnectedException )
                {
                }
#endif

                if ( DotNetObjectRef != null )
                {
                    DotNetObjectRef.Dispose();
                    DotNetObjectRef = null;
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

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        protected DotNetObjectReference<ChartStreamingAdapter> DotNetObjectRef { get; set; }

        protected JSChartStreamingModule JSModule { get; private set; }

        [Inject] IJSRuntime JSRuntime { get; set; }

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
}
