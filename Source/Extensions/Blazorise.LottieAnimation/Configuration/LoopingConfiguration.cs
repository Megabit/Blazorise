using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

/// <summary>
/// Describes how an animation should loop.
/// </summary>
[JsonConverter( typeof(LoopConfigurationJsonConverter) )]
public record LoopingConfiguration
{
    /// <summary>
    /// Number of times the animation should loop
    /// </summary>
    public int? Iterations { get; }

    /// <summary>
    /// Whether or not the animation should loop
    /// </summary>
    public bool? ShouldLoop { get; }

    /// <summary>
    /// Creates a new <see cref="LoopingConfiguration"/> instance that specifies a number of times to loop
    /// </summary>
    /// <param name="iterations">Number of times to loop</param>
    public LoopingConfiguration( int iterations )
    {
        Iterations = iterations;
    }

    /// <summary>
    /// Creates a new <see cref="LoopingConfiguration"/> instance that specifies whether or not to loop
    /// </summary>
    /// <param name="shouldLoop"></param>
    public LoopingConfiguration( bool shouldLoop )
    {
        ShouldLoop = shouldLoop;
    }

    /// <summary>
    /// Implicitly converts a boolean value to a <see cref="LoopingConfiguration"/>
    /// </summary>
    /// <param name="shouldLoop">Whether or not the animation should loop</param>
    /// <returns>A new <see cref="LoopingConfiguration"/></returns>
    public static implicit operator LoopingConfiguration( bool shouldLoop )
    {
        return new LoopingConfiguration( shouldLoop );
    }

    /// <summary>
    /// Implicitly converts an integer to a <see cref="LoopingConfiguration"/>
    /// </summary>
    /// <param name="iterations">Number of times the animation should loop</param>
    /// <returns>A new <see cref="LoopingConfiguration"/></returns>
    public static implicit operator LoopingConfiguration( int iterations )
    {
        return new LoopingConfiguration( iterations );
    }
}