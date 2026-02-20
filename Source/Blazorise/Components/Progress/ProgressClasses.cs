namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Progress"/>.
/// </summary>
public sealed record class ProgressClasses : ComponentClasses
{
    /// <summary>
    /// Targets the progress bar element.
    /// </summary>
    public string Bar { get; init; }
}

/// <summary>
/// Component styles for <see cref="Progress"/>.
/// </summary>
public sealed record class ProgressStyles : ComponentStyles
{
    /// <summary>
    /// Targets the progress bar element.
    /// </summary>
    public string Bar { get; init; }
}