using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

/// <summary>
/// Available animation renderers types.
/// </summary>
[JsonConverter( typeof( LottieAnimationRendererConverter ) )]
public enum LottieAnimationRenderer
{
    /// <summary>
    /// Svg-based animation renderers.
    /// </summary>
    Svg,

    /// <summary>
    /// Canvas-based animation renderers.
    /// </summary>
    Canvas,

    /// <summary>
    /// Html-based animation renderers.
    /// </summary>
    Html
}