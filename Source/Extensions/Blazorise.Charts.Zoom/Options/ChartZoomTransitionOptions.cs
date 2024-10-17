﻿#region Using directives
using System.Text.Json.Serialization;
#endregion

namespace Blazorise.Charts.Zoom;

/// <summary>
/// Defines the chart transition options for Zoom.
/// </summary>
public class ChartZoomTransitionOptions
{
    /// <summary>
    /// Defines the zoom animation options.
    /// </summary>
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public ChartAnimation Animation { get; set; }
}
