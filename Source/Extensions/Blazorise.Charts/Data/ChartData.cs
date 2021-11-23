#region Using directives
using System.Collections.Generic;
using System.Runtime.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Base data object for all charts.
    /// </summary>
    /// <typeparam name="TItem">Type of value in the dataset.</typeparam>
    [DataContract]
    public class ChartData<TItem>
    {
        /// <summary>
        /// List of labels for the chart coordinates.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public List<object> Labels { get; set; }

        /// <summary>
        /// List of datasets to be displayed in the chart.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        public List<ChartDataset<TItem>> Datasets { get; set; }
    }
}
