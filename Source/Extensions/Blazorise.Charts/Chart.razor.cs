﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts
{
    public abstract class BaseChart<TDataSet, TItem, TOptions> : BaseComponent
        where TDataSet : ChartDataset<TItem>
        where TOptions : ChartOptions
    {
        #region Members

        private bool dirty = true;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Chart() );

            base.RegisterClasses();
        }

        protected override async Task OnAfterRenderAsync( bool firstRender )
        {
            await Update();
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

        /// <summary>
        /// Update and redraw the chart.
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            if ( dirty )
            {
                dirty = false;

                await JS.SetChartData( JSRuntime, ElementId, Type, Data, Options, DataJsonString, OptionsJsonString );
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

        [Inject] IJSRuntime JSRuntime { get; set; }

        #endregion
    }
}
