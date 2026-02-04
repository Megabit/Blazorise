namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Switch{TValue}"/>.
/// </summary>
public sealed record class SwitchClasses : ComponentClasses
{
    /// <summary>
    /// Targets the switch wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}

/// <summary>
/// Component styles for <see cref="Switch{TValue}"/>.
/// </summary>
public sealed record class SwitchStyles : ComponentStyles
{
    /// <summary>
    /// Targets the switch wrapper element.
    /// </summary>
    public string Wrapper { get; init; }
}