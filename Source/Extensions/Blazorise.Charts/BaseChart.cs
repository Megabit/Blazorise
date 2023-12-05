#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Licensing;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts;

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

            NotifyInitialized();
        }
        else if ( initialized )
        {
            await Update();
        }

        await base.OnAfterRenderAsync( firstRender );
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
            await JSModule.Clear( ElementId );
    }

    /// <summary>
    /// Adds a new label to the chart.
    /// </summary>
    /// <param name="labels">Label name(s).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddLabels( params object[] labels )
    {
        dirty = true;

        Labels.AddRange( labels );

        if ( initialized )
            await JSModule.AddLabel( ElementId, labels );
    }

    private void LimitDataSets( params TDataSet[] datasets )
    {
        if ( datasets.IsNullOrEmpty() )
            return;

        var chartsRowLimit = LicenseChecker.GetChartsRowsLimit();

        if ( !chartsRowLimit.HasValue )
            return;

        foreach ( var dataSet in datasets )
        {
            dataSet.Data = LimitData( dataSet.Data );
        }
    }

    private List<TItem> LimitData( List<TItem> data )
    {
        var chartsRowLimit = LicenseChecker.GetChartsRowsLimit();

        if ( !chartsRowLimit.HasValue )
            return data;

        return data.Take( chartsRowLimit.Value ).ToList();
    }

    /// <summary>
    /// Adds a new dataset to the chart.
    /// </summary>
    /// <param name="datasets">Data set(s).</param>
    public async Task AddDataSet( params TDataSet[] datasets )
    {
        dirty = true;

        LimitDataSets( datasets );

        Datasets.AddRange( datasets );

        if ( initialized )
            await JSModule.AddDataSet( ElementId, datasets );
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
            await JSModule.RemoveDataSet( ElementId, dataSetIndex );
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

        var limitedData = LimitData( data );
        Datasets[dataSetIndex].Data = limitedData;

        if ( initialized )
            await JSModule.SetData( ElementId, dataSetIndex, limitedData );
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

        var limitedData = LimitData( data.ToList() );

        Datasets[dataSetIndex].Data.AddRange( limitedData );

        if ( initialized )
            await JSModule.AddData( ElementId, dataSetIndex, limitedData );
    }

    /// <summary>
    /// Adds new datasets and then update the chart.
    /// </summary>
    /// <param name="datasets">List of datasets.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddDatasetsAndUpdate( params TDataSet[] datasets )
    {
        dirty = true;

        LimitDataSets( datasets );

        Datasets.AddRange( datasets );

        if ( initialized )
            await JSModule.AddDatasetsAndUpdate( ElementId, datasets );
    }

    /// <summary>
    /// Adds new set of labels and datasets and then update the chart.
    /// </summary>
    /// <param name="labels">List of labels.</param>
    /// <param name="datasets">List of datasets.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddLabelsDatasetsAndUpdate( IReadOnlyCollection<object> labels, params TDataSet[] datasets )
    {
        dirty = true;

        LimitDataSets( datasets );

        Labels.AddRange( labels );
        Datasets.AddRange( datasets );

        if ( initialized )
            await JSModule.AddLabelsDatasetsAndUpdate( ElementId, labels, datasets );
    }

    /// <summary>
    /// Removes the oldest label.
    /// </summary>
    public async Task ShiftLabel()
    {
        dirty = true;

        if ( initialized )
            await JSModule.ShiftLabel( ElementId );
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
            await JSModule.ShiftData( ElementId, dataSetIndex );
    }
    /// <summary>
    /// Removes the newest label.
    /// </summary>
    public async Task PopLabel()
    {
        dirty = true;

        if ( initialized )
            await JSModule.PopLabel( ElementId );
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
            await JSModule.PopData( ElementId, dataSetIndex );
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
            await JSModule.SetOptions( ElementId, Converters.ToDictionary( Options ), OptionsJsonString, OptionsObject );
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
            await JSModule.SetOptions( ElementId, default( TOptions ), OptionsJsonString, optionsObject );
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
            await JSModule.Resize( ElementId );
    }

    /// <summary>
    /// Destroy the current chart instance and recreates it by using the same data and options.
    /// </summary>
    /// <param name="type">New chart type.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ChangeType( ChartType type )
    {
        if ( initialized )
            await JSModule.ChangeType( ElementRef, ElementId, type );
    }

    /// <summary>
    /// Destroys the chart instance. Calling this method should generally dispose of any chart resources.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Destroy()
    {
        if ( initialized )
            await JSModule.Destroy( ElementRef, ElementId );
    }

    private ValueTask Initialize()
    {
        DotNetObjectRef ??= DotNetObjectReference.Create<ChartAdapter>( new( this ) );

        var eventOptions = new
        {
            HasClickEvent = Clicked.HasDelegate,
            HasHoverEvent = Hovered.HasDelegate,
        };

        return JSModule.Initialize( DotNetObjectRef, eventOptions, ElementRef, ElementId, Type,
            Data,
            Options,
            DataJsonString,
            OptionsJsonString,
            OptionsObject,
            PluginNames );
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

            await JSModule.Update( ElementId );
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
                return System.Text.Json.JsonSerializer.Deserialize<BarChartModel>( data );
            case ChartType.Pie:
                return System.Text.Json.JsonSerializer.Deserialize<PieChartModel>( data );
            case ChartType.Doughnut:
                return System.Text.Json.JsonSerializer.Deserialize<DoughnutChartModel>( data );
            case ChartType.PolarArea:
                return System.Text.Json.JsonSerializer.Deserialize<PolarChartModel>( data );
            case ChartType.Radar:
                return System.Text.Json.JsonSerializer.Deserialize<RadarChartModel>( data );
            case ChartType.Scatter:
                return System.Text.Json.JsonSerializer.Deserialize<ScatterChartModel>( data );
            case ChartType.Bubble:
                return System.Text.Json.JsonSerializer.Deserialize<BubbleChartModel>( data );
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
    /// Gets or sets the license checker for the user session.
    /// </summary>
    [Inject] internal BlazoriseLicenseChecker LicenseChecker { get; set; }

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