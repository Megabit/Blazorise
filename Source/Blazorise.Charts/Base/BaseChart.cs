#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Base;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Charts.Base
{
    public abstract class BaseChart : BaseComponent
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
                Data = new ChartData();

            if ( Data.Labels == null )
                Data.Labels = new List<string>();

            Data.Labels.AddRange( label );
        }

        /// <summary>
        /// Adds a new dataset to the chart.
        /// </summary>
        /// <param name="dataSet"></param>
        public void AddDataSet( params ChartDataset[] dataSet )
        {
            if ( Data == null )
                Data = new ChartData();

            if ( Data.Datasets == null )
                Data.Datasets = new List<ChartDataset>();

            Data.Datasets.AddRange( dataSet );
        }

        /// <summary>
        /// Update and redraw the chart.
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            await JS.SetChartData( elementId, Type, Data, Options );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines the chart type.
        /// </summary>
        [Parameter] protected ChartType Type { get; set; } = ChartType.Line;

        /// <summary>
        /// Defines the chart data.
        /// </summary>
        [Parameter] protected ChartData Data { get; set; }

        /// <summary>
        /// Defines the chart options.
        /// </summary>
        [Parameter] protected ChartOptions Options { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
