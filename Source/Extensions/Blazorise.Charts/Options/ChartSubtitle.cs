﻿#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts;

/// <summary>
/// Subtitle Configuration.
/// </summary>
public class ChartSubtitle
{
    /// <summary>
    /// Alignment of the subtitle.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Align { get; set; }

    /// <summary>
    /// Color of the text.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    [JsonConverter( typeof( IndexableOptionsConverter<object> ) )]
    public IndexableOption<object> Color { get; set; }

    /// <summary>
    /// Is the legend subtitle displayed.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? Display { get; set; }

    /// <summary>
    /// Marks that this box should take the full width/height of the canvas. If false, the box is sized and placed above/beside the chart area.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public bool? FullSize { get; set; }

    /// <summary>
    /// Position of subtitle.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Position { get; set; }

    /// <summary>
    /// Font of the text.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartFont Font { get; set; } = new ChartFont { Weight = "bold" };

    /// <summary>
    /// Padding around the subtitle.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public object Padding { get; set; }

    /// <summary>
    /// Subtitle text to display.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string Text { get; set; }
}