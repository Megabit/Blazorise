#region Using directives
using System.Text.Json.Serialization;
using Lambda2Js;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// The option context is used to give contextual information when resolving options. It mainly applies to scriptable
    /// options but also to some function options such as formatter.
    /// </summary>
    public class ScriptableOptionsContext
    {
        /// <summary>
        /// Whether the associated element is hovered.
        /// </summary>
        [JsonPropertyName( "active" )]
        [JavascriptMemberAttribute( MemberName = "active" )]
        public bool Active { get; set; }

        /// <summary>
        /// The associated chart.
        /// </summary>
        [JsonPropertyName( "chart" )]
        [JavascriptMemberAttribute( MemberName = "chart" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public dynamic Chart { get; set; }

        /// <summary>
        /// The index of the associated data.
        /// </summary>
        [JsonPropertyName( "dataIndex" )]
        [JavascriptMemberAttribute( MemberName = "dataIndex" )]
        public int DataIndex { get; set; }

        /// <summary>
        /// The dataset at index datasetIndex.
        /// </summary>
        [JsonPropertyName( "dataset" )]
        [JavascriptMemberAttribute( MemberName = "dataset" )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public dynamic Dataset { get; set; }

        /// <summary>
        /// The index of the associated dataset.
        /// </summary>
        [JsonPropertyName( "datasetIndex" )]
        [JavascriptMemberAttribute( MemberName = "datasetIndex" )]
        public int DatasetIndex { get; set; }
    }
}
