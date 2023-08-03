using System.Text.Json.Serialization;
using Blazorise.Utilities;

namespace Blazorise.Video;

/// <summary>
/// Defines the video settings types.
/// </summary>
[JsonConverter( typeof( CamelCaseEnumJsonConverter ) )]
public enum VideoSettingsType
{
    /// <summary>
    /// If defined, the captions settings will be available.
    /// </summary>
    Captions,

    /// <summary>
    /// If defined, the quality settings will be available.
    /// </summary>
    Quality,

    /// <summary>
    /// If defined, the speed settings will be available.
    /// </summary>
    Speed,

    /// <summary>
    /// If defined, the loop settings will be available.
    /// </summary>
    Loop,
}
