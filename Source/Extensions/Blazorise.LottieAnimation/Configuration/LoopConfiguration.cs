using System.Text.Json.Serialization;

namespace Blazorise.LottieAnimation;

/// <summary>
/// Describes how an animation should loop.
/// </summary>
[JsonConverter( typeof( LoopConfigurationConverter ) )]
public record LoopConfiguration
{
    /// <summary>
    /// Number of times the animation should loop.
    /// </summary>
    public int? Iterations { get; }

    /// <summary>
    /// Whether or not the animation should loop.
    /// </summary>
    public bool? ShouldLoop { get; }

    /// <summary>
    /// Creates a new <see cref="LoopConfiguration"/> instance that specifies a number of times to loop.
    /// </summary>
    /// <param name="iterations">Number of times to loop.</param>
    public LoopConfiguration( int iterations )
    {
        Iterations = iterations;
    }

    /// <summary>
    /// Creates a new <see cref="LoopConfiguration"/> instance that specifies whether or not to loop.
    /// </summary>
    /// <param name="shouldLoop">Whether or not the animation should loop.</param>
    public LoopConfiguration( bool shouldLoop )
    {
        ShouldLoop = shouldLoop;
    }

    /// <summary>
    /// Implicitly converts a boolean value to a <see cref="LoopConfiguration"/>.
    /// </summary>
    /// <param name="shouldLoop">Whether or not the animation should loop.</param>
    /// <returns>A new <see cref="LoopConfiguration"/>.</returns>
    public static implicit operator LoopConfiguration( bool shouldLoop )
    {
        return new LoopConfiguration( shouldLoop );
    }

    /// <summary>
    /// Implicitly converts an integer to a <see cref="LoopConfiguration"/>.
    /// </summary>
    /// <param name="iterations">Number of times the animation should loop.</param>
    /// <returns>A new <see cref="LoopConfiguration"/>.</returns>
    public static implicit operator LoopConfiguration( int iterations )
    {
        return new LoopConfiguration( iterations );
    }
}