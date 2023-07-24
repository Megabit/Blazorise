using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blazorise.Splitter;

/// <summary>
/// Configuration object sent via JS interop to configure the split component
/// </summary>
public class SplitterOptions
{
    /// <summary>
    /// Cursor to display while dragging.
    /// </summary>
    [JsonPropertyName( "cursor" )]
    public string Cursor { get; init; }

    /// <summary>
    /// Direction to split.
    /// </summary>
    [JsonPropertyName( "direction" )]
    public SplitterDirection Direction { get; init; } = SplitterDirection.Horizontal;

    /// <summary>
    /// Drag this number of pixels at a time. Defaults to 1 for smooth dragging, but can be set to a pixel value to give more control over the resulting sizes.
    /// Works particularly well when the gutterSize is set to the same size.
    /// </summary>
    [JsonPropertyName( "dragInterval" )]
    public JavascriptNumber DragInterval { get; init; }

    /// <summary>
    /// When the split is created, if ExpandToMin is true, the minSize for each element overrides the percentage value from the sizes option.
    /// </summary>
    [JsonPropertyName( "expandToMin" )]
    public bool? ExpandToMin { get; init; }

    /// <summary>
    /// Determines how the gutter aligns between the two elements.
    /// </summary>
    [JsonPropertyName( "gutterAlign" )]
    public SplitGutterAlignment GutterAlign { get; init; } = SplitGutterAlignment.Center;

    /// <summary>
    /// Gutter size in pixels.
    /// </summary>
    [JsonPropertyName( "gutterSize" )]
    public JavascriptNumber GutterSize { get; init; }

    /// <summary>
    /// Maximum size of each element.
    /// </summary>
    [JsonPropertyName( "maxSize" )]
    public JavascriptNumberOrArray MaxSize { get; init; }

    /// <summary>
    /// Minimum size of each element.
    /// </summary>
    [JsonPropertyName( "minSize" )]
    public JavascriptNumberOrArray MinSize { get; init; }

    /// <summary>
    /// Initial sizes of each element in percents or CSS values.
    /// </summary>
    [JsonPropertyName( "sizes" )]
    public IEnumerable<JavascriptNumber> Sizes { get; init; }

    /// <summary>
    /// Snap to minimum size offset in pixels.
    /// </summary>
    [JsonPropertyName( "snapOffset" )]
    public JavascriptNumberOrArray SnapOffset { get; init; }
}