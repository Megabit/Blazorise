using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

/// <summary>
/// Available Animation Renderers
/// </summary>
[JsonConverter( typeof( RendererEnumConverter ) )]
public enum Renderer
{
    /// <summary>
    /// SVG-based Animation Renderer
    /// </summary>
    SVG,

    /// <summary>
    /// Canvas-based Animation Renderer
    /// </summary>
    Canvas,

    /// <summary>
    /// HTML-based Animation Renderer
    /// </summary>
    HTML
}