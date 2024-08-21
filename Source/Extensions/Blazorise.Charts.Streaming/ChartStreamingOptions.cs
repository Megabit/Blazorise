#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Streaming;

public class ChartStreamingOptions
{
    /// <summary>
    /// Duration of the chart in milliseconds (how much time of data it will show).
    /// </summary>
    [JsonPropertyName( "duration" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? Duration { get; set; } = 10000;

    /// <summary>
    /// Duration of the data to be kept in milliseconds. If not set, old data will be automatically deleted as it disappears off the chart.
    /// </summary>
    [JsonPropertyName( "ttl" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? Ttl { get; set; }

    /// <summary>
    /// Refresh interval of data in milliseconds. onRefresh callback function will be called at this interval.
    /// </summary>
    [JsonPropertyName( "refresh" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? Refresh { get; set; } = 1000;

    /// <summary>
    /// Delay added to the chart in milliseconds so that upcoming values are known before lines are plotted. This makes the chart look like a continual stream rather than very jumpy on the right hand side. Specify the maximum expected delay.
    /// </summary>
    [JsonPropertyName( "delay" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public long? Delay { get; set; } = 0;

    /// <summary>
    /// Frequency at which the chart is drawn on a display (frames per second). This option can be set at chart level but not at axis level. Decrease this value to save CPU power (https://github.com/nagix/chartjs-plugin-streaming#lowering-cpu-usage).
    /// </summary>
    [JsonPropertyName( "frameRate" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public int? FrameRate { get; set; } = 30;

    /// <summary>
    /// If set to true, scrolling stops. Note that onRefresh callback is called even when this is set to true.
    /// </summary>
    [JsonPropertyName( "pause" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Pause { get; set; } = false;
}