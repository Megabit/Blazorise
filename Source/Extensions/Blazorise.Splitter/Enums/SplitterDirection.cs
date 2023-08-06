using System.Text.Json.Serialization;
using Blazorise.Utilities.JsonConverters;

namespace Blazorise.Splitter;

/// <summary>
/// Split directions
/// </summary>
[JsonConverter( typeof( CamelCaseEnumJsonConverter ) )]
public enum SplitterDirection
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