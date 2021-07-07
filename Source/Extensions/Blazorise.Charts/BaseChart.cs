#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts
{
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
        /// <returns>A task that represents the asynchronous operation.</returns>
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
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Obsolete( "This method will likely be removed in the future. Please use " + nameof( AddLabels ) + " instead." )]
        public Task AddLabel( params object[] labels )
        {
            return AddLabels( labels );
        }

        /// <summary>
        /// Adds a new label to the chart.
        /// </summary>
        /// <param name="labels">Label name(s).</param>
        public async Task AddLabels( params object[] labels )
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
        /// Removed the dataset at the specified index.
        /// </summary>
        /// <param name="dataSetIndex">Index of the dataset in the data list.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RemoveDataSet( int dataSetIndex )
        {
            dirty = true;

            Datasets.RemoveAt( dataSetIndex );

            if ( initialized )
                await JS.RemoveDataSet( JSRuntime, ElementId, dataSetIndex );
        }

        /// <summary>
        /// Sets the new data point(s) to the specified dataset.
        /// </summary>
        /// <param name="dataSetIndex">Dataset index to which we set the data point(s).</param>
        /// <param name="data">Data point(s) to set.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetData( int dataSetIndex, List<TItem> data )
        {
            dirty = true;

            Datasets[dataSetIndex].Data = data;

            if ( initialized )
                await JS.SetData( JSRuntime, ElementId, dataSetIndex, data );
        }

        /// <summary>
        /// Adds the new data point(s) to the specified dataset.
        /// </summary>
        /// <param name="dataSetIndex">Dataset index to which we add the data point(s).</param>
        /// <param name="data">Data point(s) to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
        /// <returns>A task that represents the asynchronous operation.</returns>
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
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddLabelsDatasetsAndUpdate( IReadOnlyCollection<string> labels, params TDataSet[] datasets )
        {
            dirty = true;

            Labels.AddRange( labels );
            Datasets.AddRange( datasets );

            if ( initialized )
                await JS.AddLabelsDatasetsAndUpdate( JSRuntime, ElementId, labels, datasets );
        }

        /// <summary>
        /// Removes the oldest label.
        /// </summary>
        public async Task ShiftLabel()
        {
            dirty = true;

            if ( initialized )
                await JS.ShiftLabel( JSRuntime, ElementId );
        }

        /// <summary>
        /// Removes the oldest data point from the specified dataset.
        /// </summary>
        /// <param name="dataSetIndex">Dataset index from which the oldest data point is to be removed from.</param>
        /// <returns></returns>
        public async Task ShiftData( int dataSetIndex )
        {
            dirty = true;

            if ( initialized )
                await JS.ShiftData( JSRuntime, ElementId, dataSetIndex );
        }
        /// <summary>
        /// Removes the newest label.
        /// </summary>
        public async Task PopLabel()
        {
            dirty = true;

            if ( initialized )
                await JS.PopLabel( JSRuntime, ElementId );
        }

        /// <summary>
        /// Removes the newest data point from the specified dataset.
        /// </summary>
        /// <param name="dataSetIndex">Dataset index from which the newest data point is to be removed from.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task PopData( int dataSetIndex )
        {
            dirty = true;

            if ( initialized )
                await JS.PopData( JSRuntime, ElementId, dataSetIndex );
        }

        /// <summary>
        /// Sets the chart's options manually. Must call Update after making this call.  If the options changes AspectRatio must also call Resize after Update.
        /// </summary>
        /// <param name="options">New chart options used though OptionsJsonString or OptionsObject will supersede.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetOptions( TOptions options )
        {
            dirty = true;

            Options = options;

            if ( initialized )
                await JS.SetOptions( JSRuntime, ElementId, Converters.ToDictionary( Options ), OptionsJsonString, OptionsObject );
        }

        /// <summary>
        /// Sets the chart's <see cref="OptionsObject"/> manually. Must call <see cref="Update"/> after manging this call.
        /// If the options changes AspectRatio must also call <see cref="Resize"/> after <see cref="Update"/>.
        /// </summary>
        /// <param name="optionsObject">New chart options object used.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SetOptionsObject( object optionsObject )
        {
            dirty = true;

            OptionsObject = optionsObject;

            if ( initialized )
                await JS.SetOptions( JSRuntime, ElementId, default( TOptions ), OptionsJsonString, optionsObject );
        }

        /// <summary>
        ///  Manually resize the canvas element. This is run each time the canvas container is resized,
        ///  but you can call this method manually if you change the size of the canvas nodes container element.
        ///  Should also be called when updating the aspect ratio.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Resize()
        {
            if ( initialized )
                await JS.Resize( JSRuntime, ElementId );
        }

        private ValueTask Initialize()
        {
            DotNetObjectRef ??= JS.CreateDotNetObjectRef( new( this ) );

            var eventOptions = new
            {
                HasClickEvent = Clicked.HasDelegate,
                HasHoverEvent = Hovered.HasDelegate,
            };

            return JS.Initialize( JSRuntime, DotNetObjectRef, eventOptions, ElementId, Type,
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

            var eventArgs = new ChartMouseEventArgs( datasetIndex, index, model );

            if ( eventName == "click" )
                return Clicked.InvokeAsync( eventArgs );
            else if ( eventName == "hover" )
                return Hovered.InvokeAsync( eventArgs );
            else if ( eventName == "mouseout" )
                return MouseOut.InvokeAsync( eventArgs );

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

        protected List<object> Labels
        {
            get
            {
                Data ??= new();

                if ( Data.Labels == null )
                    Data.Labels = new();

                return Data.Labels;
            }
        }

        protected List<ChartDataset<TItem>> Datasets
        {
            get
            {
                Data ??= new();

                if ( Data.Datasets == null )
                    Data.Datasets = new();

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
        [Parameter] public string DataJsonString { get; set; }

        /// <summary>
        /// Defines the chart options that is serialized as json string.
        /// </summary>
        [Parameter] public string OptionsJsonString { get; set; }

        /// <summary>
        /// Defines the chart options that is serialized as json object.
        /// </summary>
        [Parameter] public object OptionsObject { get; set; }

        #endregion
    }
}
