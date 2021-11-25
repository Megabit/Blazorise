#region Using directives
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    [DataContract]
    public class ChartOptions
    {
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public Scales Scales { get; set; }

        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public Legend Legend { get; set; }

        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public Tooltips Tooltips { get; set; }

        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public Animation Animation { get; set; }

        /// <summary>
        /// Resizes the chart canvas when its container does.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? Responsive { get; set; } = true;

        /// <summary>
        /// Maintain the original canvas aspect ratio (width / height) when resizing.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? MaintainAspectRatio { get; set; } = true;

        /// <summary>
        /// Duration in milliseconds it takes to animate to new size after a resize event.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? ResponsiveAnimationDuration { get; set; } = 0;

        /// <summary>
        /// Canvas aspect ratio (i.e. width / height, a value of 1 representing a square canvas).
        /// Note that this option is ignored if the height is explicitly defined either as attribute or via the style.
        /// </summary>
        [DataMember( EmitDefaultValue = false )]
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public double? AspectRatio { get; set; } = 2;
    }
}
