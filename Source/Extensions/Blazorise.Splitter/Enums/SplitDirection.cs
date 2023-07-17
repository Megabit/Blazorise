using System.Text.Json.Serialization;

namespace Blazorise.Splitter;

/// <summary>
/// Split directions
/// </summary>
[JsonConverter( typeof( CamelCaseEnumJsonConverter ) )]
public enum SplitDirection
{
    /// <summary>
    /// Split horizontally.
    /// </summary>
    Horizontal,

    /// <summary>
    /// Split vertically.
    /// </summary>
    Vertical
}