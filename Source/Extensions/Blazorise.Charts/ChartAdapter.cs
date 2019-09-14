#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Charts
{
    public class ChartAdapter
    {
        private readonly IBaseChart chart;

        public ChartAdapter( IBaseChart chart )
        {
            this.chart = chart;
        }

        [JSInvokable]
        public Task ModelClicked( int datasetIndex, int index, string model )
        {
            return chart.ModelClicked( datasetIndex, index, model );
        }
    }
}
