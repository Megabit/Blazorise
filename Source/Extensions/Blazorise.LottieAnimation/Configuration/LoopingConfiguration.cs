using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

[JsonConverter(typeof(LoopConfigurationJsonConverter))]
public record LoopingConfiguration
{
    public int? Iterations { get; }

    public bool? ShouldLoop { get; }

    public LoopingConfiguration(int iterations)
    {
        Iterations = iterations;
    }

    public LoopingConfiguration(bool shouldLoop)
    {
        ShouldLoop = shouldLoop;
    }

    public static implicit operator LoopingConfiguration(bool shouldLoop)
    {
        return new LoopingConfiguration(shouldLoop);
    }

    public static implicit operator LoopingConfiguration(int iterations)
    {
        return new LoopingConfiguration(iterations);
    }
}