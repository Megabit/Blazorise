#region Using directives
using System.Collections.Generic;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Base data object for all charts.
    /// </summary>
    /// <typeparam name="TItem">Type of value in the dataset.</typeparam>
    public class ChartData<TItem>
    {
        /// <summary>
        /// List of labels for the chart coordinates.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public List<object> Labels { get; set; }

        /// <summary>
        /// List of datasets to be displayed in the chart.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public List<ChartDataset<TItem>> Datasets { get; set; }
    }
}
