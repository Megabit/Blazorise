#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Base;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Charts.Base
{
    public abstract class BaseChart<TDataSet, TItem, TOptions> : BaseComponent
        where TDataSet : ChartDataset<TItem>
        where TOptions : ChartOptions
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Chart() );

            base.RegisterClasses();
        }

        protected override async Task OnAfterRenderAsync()
        {
            await Update();
        }

        /// <summary>
        /// Clears all the data from the chart.
        /// </summary>
        public void Clear()
        {
            Data?.Labels?.Clear();
            Data?.Datasets?.Clear();
        }

        /// <summary>
        /// Adds a new label to the chart.
        /// </summary>
        /// <param name="label"></param>
        public void AddLabel( params string[] label )
        {
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
            if ( Data == null )
                Data = new ChartData<TItem>();

            if ( Data.Datasets == null )
                Data.Datasets = new List<ChartDataset<TItem>>();

            Data.Datasets.AddRange( dataSet );
        }

        /// <summary>
        /// Update and redraw the chart.
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            await JS.SetChartData( ElementId, Type, DataJsonString ?? (object)Data, OptionsJsonString ?? (object)Options );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the chart type.
        /// </summary>
        [Parameter] protected virtual ChartType Type { get; set; }

        /// <summary>
        /// Defines the chart data.
        /// </summary>
        [Parameter] protected ChartData<TItem> Data { get; set; }

        /// <summary>
        /// Defines the chart options.
        /// </summary>
        [Parameter] protected TOptions Options { get; set; }

        /// <summary>
        /// Defines the chart data that is serialized as json string.
        /// </summary>
        [Obsolete( "This parameter will likely be removed in the future as it's just a temporary feature until Blazor implements better serializer." )]
        [Parameter] protected string DataJsonString { get; set; }

        /// <summary>
        /// Defines the chart options that is serialized as json string.
        /// </summary>
        [Obsolete( "This parameter will likely be removed in the future as it's just a temporary feature until Blazor implements better serializer." )]
        [Parameter] protected string OptionsJsonString { get; set; }

        #endregion
    }
}
