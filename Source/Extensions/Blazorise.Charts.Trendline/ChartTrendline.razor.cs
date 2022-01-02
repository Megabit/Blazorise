#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts.Trendline
{
    public interface IChartTrendline
    {
        Task Refresh();
    }

    /// <summary>
    /// Provides the streaming capabilities to the supported chart types.
    /// </summary>
    /// <typeparam name="TItem">Data point type.</typeparam>
    public partial class ChartTrendline<TItem> : BaseComponent, IChartTrendline, IAsyncDisposable
    {
        protected override Task OnInitializedAsync()
        {
            if ( JSModule == null )
            {
                JSModule = new JSChartTrendlineModule( JSRuntime, VersionProvider );
            }

            return base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender && ParentChart != null )
            {
                DotNetObjectRef ??= DotNetObjectReference.Create( new ChartTrendlineAdapter( this ) );

                await JSModule.Initialize( DotNetObjectRef, ParentChart.ElementRef, ParentChart.ElementId, Vertical, Options );
            }

            await base.OnAfterRenderAsync( firstRender );
        }

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
            }

            await base.DisposeAsync( disposing );
        }

        public async Task Refresh()
        {
            if ( !Rendered )
                return;

            //foreach ( var dataset in ParentChart?.Data?.Datasets ?? Enumerable.Empty<ChartDataset<TItem>>() )
            //{
            //    var datasetIndex = ParentChart.Data.Datasets.IndexOf( dataset );

            //    var newData = new ChartTrendlineData<TItem>( dataset.Label, datasetIndex );

            //    await Refreshed.InvokeAsync( newData );

            //}
        }

        /// <inheritdoc/>
        protected override bool ShouldAutoGenerateId => true;

        protected DotNetObjectReference<ChartTrendlineAdapter> DotNetObjectRef { get; set; }

        protected JSChartTrendlineModule JSModule { get; private set; }

        [Inject] private IJSRuntime JSRuntime { get; set; }

        [Inject] private IVersionProvider VersionProvider { get; set; }

        [CascadingParameter] protected BaseChart<TItem> ParentChart { get; set; }

        /// <summary>
        /// If true, chart will be set to vertical mode.
        /// </summary>
        [Parameter] public bool Vertical { get; set; }

        /// <summary>
        /// Stream options.
        /// </summary>
        [Parameter] public List<ChartTrendlineOptions> Options { get; set; } = new() { new ChartTrendlineOptions() };

        /// <summary>
        /// Callback function that will be called at a regular interval. The callback takes one argument, a reference to the dataset object. You can update your datasets here. The chart will be automatically updated after returning.
        /// </summary>
        //[Parameter] public EventCallback<ChartTrendlineData<TItem>> Refreshed { get; set; }

    }
}
