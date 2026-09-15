namespace Blazorise.Demo.Models;

public record AppSettings
{
    public AnimationSettings AnimationSettings { get; init; } = new();
}