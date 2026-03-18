namespace Blazorise.Components;

/// <summary>
/// Component classes for <see cref="OneTimeInput"/>.
/// </summary>
public sealed record class OneTimeInputClasses : ComponentClasses
{
    /// <summary>
    /// Targets the root container element.
    /// </summary>
    public string Container { get; init; }

    /// <summary>
    /// Targets each grouped slot wrapper.
    /// </summary>
    public string Group { get; init; }

    /// <summary>
    /// Targets each slot input element.
    /// </summary>
    public string Slot { get; init; }

    /// <summary>
    /// Targets the separator element between groups.
    /// </summary>
    public string Separator { get; init; }
}

/// <summary>
/// Component styles for <see cref="OneTimeInput"/>.
/// </summary>
public sealed record class OneTimeInputStyles : ComponentStyles
{
    /// <summary>
    /// Targets the root container element.
    /// </summary>
    public string Container { get; init; }

    /// <summary>
    /// Targets each grouped slot wrapper.
    /// </summary>
    public string Group { get; init; }

    /// <summary>
    /// Targets each slot input element.
    /// </summary>
    public string Slot { get; init; }

    /// <summary>
    /// Targets the separator element between groups.
    /// </summary>
    public string Separator { get; init; }
}