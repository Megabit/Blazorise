#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// This is needed to set the value from javascript because calling generic component directly is not supported by Blazor.
    /// </summary>
    public interface IBaseChart
    {
        Task ModelClicked( int datasetIndex, int index, string model );
    }

    public abstract class BaseChart<TDataSet, TItem, TOptions, TModel> : BaseComponent, IDisposable, IBaseChart
        where TDataSet : ChartDataset<TItem>
        where TOptions : ChartOptions
    {
        #region Members

        private DotNetObjectReference<ChartAdapter> dotNetObjectRef;

        private bool dirty = true;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Chart() );

            base.BuildClasses( builder );
        }

        public void Dispose()
        {
            JS.Destroy( JSRuntime, ElementId );
            JS.DisposeDotNetObjectRef( dotNetObjectRef );
        }

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                await Initialize();
            }
            else
            {
                await Update();
            }
        }

        /// <summary>
        /// Clears all the data from the chart.
        /// </summary>
        public void Clear()
        {
            dirty = true;

            Data?.Labels?.Clear();
            Data?.Datasets?.Clear();
        }

        /// <summary>
        /// Adds a new label to the chart.
        /// </summary>
        /// <param name="label"></param>
        public void AddLabel( params string[] label )
        {
            dirty = true;

            if ( Data == null )
                Data = new ChartData<TItem>();

            if ( Data.Labels == null )
                Data.Labels = new List<string>();

            Data.Labels.AddRange( label );
        }

        /// <summary>
        /// Adds a new dataset to the chart.
        /// </summary>
        /// <param name="dataSet"></param>
        public void AddDataSet( params TDataSet[] dataSet )
        {
            dirty = true;

            if ( Data == null )
                Data = new ChartData<TItem>();

            if ( Data.Datasets == null )
                Data.Datasets = new List<ChartDataset<TItem>>();

            Data.Datasets.AddRange( dataSet );
        }


        public void SetOptions( TOptions options )
        {
            dirty = true;

            Options = options;
        }

        private async Task Initialize()
        {
            dotNetObjectRef = dotNetObjectRef ?? JS.CreateDotNetObjectRef( new ChartAdapter( this ) );
            await JS.InitializeChart( dotNetObjectRef, JSRuntime, ElementId, Type, Data, Options, DataJsonString, OptionsJsonString );
        }

        /// <summary>
        /// Update and redraw the chart.
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            if ( dirty )
            {
                dirty = false;

                await JS.UpdateChart( JSRuntime, ElementId, Data, Options, DataJsonString, OptionsJsonString );
            }
        }

        public Task ModelClicked( int datasetIndex, int index, string modelJson )
        {
            var model = Serialize( modelJson );

            var chartClickData = new ChartMouseEventArgs( datasetIndex, index, model );

            return Clicked.InvokeAsync( chartClickData );
        }

        // TODO: this is just temporary until System.Text.Json implements serialization of the inheriter fields.
        private object Serialize( string data )
        {
            switch ( this.Type )
            {
                case ChartType.Line:
                    return System.Text.Json.JsonSerializer.Deserialize<LineChartModel>( data );
                case ChartType.Bar:
                    return System.Text.Json.JsonSerializer.Deserialize<BarChartModel>( data );
                case ChartType.Pie:
                    return System.Text.Json.JsonSerializer.Deserialize<PieChartModel>( data );
                case ChartType.Doughnut:
                    return System.Text.Json.JsonSerializer.Deserialize<DoughnutChartModel>( data );
                case ChartType.PolarArea:
                    return System.Text.Json.JsonSerializer.Deserialize<PolarChartModel>( data );
                case ChartType.Radar:
                    return System.Text.Json.JsonSerializer.Deserialize<RadarChartModel>( data );
                default:
                    return null;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the chart type.
        /// </summary>
        [Parameter] public ChartType Type { get; set; }

        /// <summary>
        /// Defines the chart data.
        /// </summary>
        [Parameter] public ChartData<TItem> Data { get; set; }

        /// <summary>
        /// Defines the chart options.
        /// </summary>
        [Parameter] public TOptions Options { get; set; }

        /// <summary>
        /// Defines the chart data that is serialized as json string.
        /// </summary>
        [Obsolete( "This parameter will likely be removed in the future as it's just a temporary feature until Blazor implements better serializer." )]
        [Parameter] public string DataJsonString { get; set; }

        /// <summary>
        /// Defines the chart options that is serialized as json string.
        /// </summary>
        [Obsolete( "This parameter will likely be removed in the future as it's just a temporary feature until Blazor implements better serializer." )]
        [Parameter] public string OptionsJsonString { get; set; }

        [Parameter] public EventCallback<ChartMouseEventArgs> Clicked { get; set; }

        [Inject] IJSRuntime JSRuntime { get; set; }

        #endregion
    }
}
