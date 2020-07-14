﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Interface is needed to set the value from javascript because calling generic component directly is not supported by Blazor.
    /// </summary>
    public interface IBaseChart
    {
        Task Event( string eventName, int datasetIndex, int index, string model );
    }

    public class BaseChart<TItem> : BaseComponent
    {
        #region Members

        protected DotNetObjectReference<ChartAdapter> dotNetObjectRef;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Chart() );

            base.BuildClasses( builder );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                JS.Destroy( JSRuntime, ElementId );
                JS.DisposeDotNetObjectRef( dotNetObjectRef );
            }

            base.Dispose( disposing );
        }

        #endregion

        #region Properties

        [Inject] protected IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Defines the chart data.
        /// </summary>
        [Parameter] public ChartData<TItem> Data { get; set; }

        [Parameter] public EventCallback<ChartMouseEventArgs> Clicked { get; set; }

        [Parameter] public EventCallback<ChartMouseEventArgs> Hovered { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }

    public class BaseChart<TDataSet, TItem, TOptions, TModel> : BaseChart<TItem>, IBaseChart
    where TDataSet : ChartDataset<TItem>
    where TOptions : ChartOptions
    where TModel : ChartModel
    {
        #region Members

        private bool dirty = true;

        private bool initialized;

        #endregion

        #region Methods

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender && !initialized )
            {
                await Initialize();

                initialized = true;
            }
            else if ( initialized )
            {
                await Update();
            }
        }

        /// <summary>
        /// Clears all the labels and data from the chart.
        /// </summary>
        public async Task Clear()
        {
            dirty = true;

            Labels.Clear();
            Datasets.Clear();

            if ( initialized )
                await JS.Clear( JSRuntime, ElementId );
        }

        /// <summary>
        /// Adds a new label to the chart.
        /// </summary>
        /// <param name="labels">Label name(s).</param>
        public async Task AddLabel( params string[] labels )
        {
            dirty = true;

            Labels.AddRange( labels );

            if ( initialized )
                await JS.AddLabel( JSRuntime, ElementId, labels );
        }

        /// <summary>
        /// Adds a new dataset to the chart.
        /// </summary>
        /// <param name="datasets">Data set(s).</param>
        public async Task AddDataSet( params TDataSet[] datasets )
        {
            dirty = true;

            Datasets.AddRange( datasets );

            if ( initialized )
                await JS.AddDataSet( JSRuntime, ElementId, datasets );
        }

        /// <summary>
        /// Adds the new data point to the specified dataset.
        /// </summary>
        /// <param name="dataSetIndex">Dataset index to which we add the data point.</param>
        /// <param name="data">Data point(s) to add.</param>
        /// <returns></returns>
        public async Task AddData( int dataSetIndex, params TItem[] data )
        {
            dirty = true;

            Datasets[dataSetIndex].Data.AddRange( data );

            if ( initialized )
                await JS.AddData( JSRuntime, ElementId, dataSetIndex, data );
        }

        /// <summary>
        /// Adds new datasets and then update the chart.
        /// </summary>
        /// <param name="datasets">List of datasets.</param>
        /// <returns></returns>
        public async Task AddDatasetsAndUpdate( params TDataSet[] datasets )
        {
            dirty = true;

            Datasets.AddRange( datasets );

            if ( initialized )
                await JS.AddDatasetsAndUpdate( JSRuntime, ElementId, datasets );
        }

        /// <summary>
        /// Adds new set of labels and datasets and then update the chart.
        /// </summary>
        /// <param name="labels">List of labels.</param>
        /// <param name="datasets">List of datasets.</param>
        /// <returns></returns>
        public async Task AddLabelsDatasetsAndUpdate( IReadOnlyCollection<string> labels, params TDataSet[] datasets )
        {
            dirty = true;

            Labels.AddRange( labels );
            Datasets.AddRange( datasets );

            if ( initialized )
                await JS.AddLabelsDatasetsAndUpdate( JSRuntime, ElementId, labels, datasets );
        }

        /// <summary>
        /// Sets the charts options manually. Must call 
        /// </summary>
        /// <param name="options">New chart options.</param>
        /// <returns></returns>
        public async Task SetOptions( TOptions options )
        {
            dirty = true;

            Options = options;

            if ( initialized )
                await JS.SetOptions( JSRuntime, ElementId, Converters.ToDictionary( Options ), OptionsJsonString, OptionsObject );
        }

        private async Task Initialize()
        {
            dotNetObjectRef ??= JS.CreateDotNetObjectRef( new ChartAdapter( this ) );

            await JS.Initialize( JSRuntime, dotNetObjectRef, Clicked.HasDelegate, Hovered.HasDelegate, ElementId, Type,
                Data,
                Converters.ToDictionary( Options ),
                DataJsonString,
                OptionsJsonString,
                OptionsObject );
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

                await JS.Update( JSRuntime, ElementId );
            }
        }

        public Task Event( string eventName, int datasetIndex, int index, string modelJson )
        {
            var model = Serialize( modelJson );

            var chartClickData = new ChartMouseEventArgs( datasetIndex, index, model );

            if ( eventName == "click" )
                return Clicked.InvokeAsync( chartClickData );
            else if ( eventName == "hover" )
                return Hovered.InvokeAsync( chartClickData );

            return Task.CompletedTask;
        }

        // TODO: this is just temporary until System.Text.Json implements serialization of the inheriter fields.
        private object Serialize( string data )
        {
            switch ( Type )
            {
                case ChartType.Line:
                    return System.Text.Json.JsonSerializer.Deserialize<LineChartModel>( data );
                case ChartType.Bar:
                case ChartType.HorizontalBar:
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

        protected List<string> Labels
        {
            get
            {
                if ( Data == null )
                    Data = new ChartData<TItem>();

                if ( Data.Labels == null )
                    Data.Labels = new List<string>();

                return Data.Labels;
            }
        }

        protected List<ChartDataset<TItem>> Datasets
        {
            get
            {
                if ( Data == null )
                    Data = new ChartData<TItem>();

                if ( Data.Datasets == null )
                    Data.Datasets = new List<ChartDataset<TItem>>();

                return Data.Datasets;
            }
        }

        /// <summary>
        /// Defines the chart type.
        /// </summary>
        [Parameter] public ChartType Type { get; set; }

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

        /// <summary>
        /// Defines the chart options that is serialized as json object.
        /// </summary>
        [Obsolete( "This parameter will likely be removed in the future as it's just a temporary feature until Blazor implements better serializer." )]
        [Parameter] public object OptionsObject { get; set; }

        #endregion
    }
}
