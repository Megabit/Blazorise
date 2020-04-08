#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Blazorise.Charts.Streaming
{
    public class ChartStreamingData<TItem>
    {
        /// <summary>
        /// Gets or sets the data point.
        /// </summary>
        public TItem Value { get; set; }
    }
}
