using System.Text.Json.Serialization;

namespace Blazorise.Splitter;

/// <summary>
/// Gutter alignment between elements
/// </summary>
[JsonConverter( typeof( CamelCaseEnumJsonConverter ) )]
public enum SplitterGutterAlignment
{
    /// <summary>
    /// Shrinks the first element to fit the gutter
    /// </summary>
    Start,

    /// <summary>
    /// Shrinks both elements by the same amount so the gutter sits between
    /// </summary>
    Center,

    /// <summary>
    /// Shrinks the second element to fit the gutter
    /// </summary>
    End
}