#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts
{
    /// <summary>
    /// Tooltip Configuration
    /// </summary>
    public class Tooltips
    {
        /// <summary>
        /// Are on-canvas tooltips enabled.
        /// </summary>
        [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
        public bool? Enabled { get; set; } = true;
    }
}
