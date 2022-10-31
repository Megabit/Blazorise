using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

[JsonConverter(typeof(RendererConfigJsonConverter))]
public abstract record BaseRendererConfig
{
    public string?                           ClassName                { get; init; }
    public PreserveAspectRatioConfiguration? ImagePreserveAspectRatio { get; init; }
}