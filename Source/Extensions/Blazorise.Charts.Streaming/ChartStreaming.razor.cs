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
    /// Provides the streaming capabilites to the supported chart types.
    /// </summary>
    /// <typeparam name="TItem">Data point type.</typeparam>
    public partial class ChartStreaming<TItem> : BaseComponent, IChartStreaming
    {
        #region Members

        private DotNetObjectReference<ChartStreamingAdapter> dotNetObjectReference;

        #endregion

        #region Methods

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender && ParentChart != null )
            {
                dotNetObjectReference ??= JS.CreateDotNetObjectRef( new ChartStreamingAdapter( this ) );

                await JS.Initialize( JSRuntime, dotNetObjectReference, ParentChart.ElementId, Vertical, Options );
            }

            await base.OnAfterRenderAsync( firstRender );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                JS.DisposeDotNetObjectRef( dotNetObjectReference );
            }

            base.Dispose( disposing );
        }

        public async Task Refresh()
        {
            foreach ( var dataset in ParentChart?.Data?.Datasets ?? Enumerable.Empty<ChartDataset<TItem>>() )
            {
                var datasetIndex = ParentChart.Data.Datasets.IndexOf( dataset );

                var newData = new ChartStreamingData<TItem>( dataset.Label, datasetIndex );

                await Refreshed.InvokeAsync( newData );

                await JS.AddData( JSRuntime,
                    ParentChart.ElementId,
                    newData.DatasetIndex,
                    newData.Value );
            }
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        [Inject] IJSRuntime JSRuntime { get; set; }

        [CascadingParameter] protected BaseChart<TItem> ParentChart { get; set; }

        /// <summary>
        /// If true, chart will be set to vertical mode.
        /// </summary>
        [Parameter] public bool Vertical { get; set; }

        /// <summary>
        /// Stream options.
        /// </summary>
        [Parameter] public ChartStreamingOptions Options { get; set; } = new ChartStreamingOptions();

        /// <summary>
        /// Callback function that will be called at a regular interval. The callback takes one argument, a reference to the dataset object. You can update your datasets here. The chart will be automatically updated after returning.
        /// </summary>
        [Parameter] public EventCallback<ChartStreamingData<TItem>> Refreshed { get; set; }

        #endregion
    }
}
