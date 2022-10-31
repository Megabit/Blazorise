using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

[JsonConverter(typeof(RendererEnumConverter))]
public enum Renderer
{
    SVG,
    Canvas,
    HTML
}