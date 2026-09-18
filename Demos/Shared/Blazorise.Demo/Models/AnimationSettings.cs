namespace Blazorise.Demo.Models;

public record AnimationSettings
{
    public bool Animated { get; init; } = true;

    public int? AnimationDuration { get; init; }
}