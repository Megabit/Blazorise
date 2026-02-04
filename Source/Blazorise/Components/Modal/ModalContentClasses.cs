namespace Blazorise;

/// <summary>
/// Component classes for <see cref="ModalContent"/>.
/// </summary>
public sealed record class ModalContentClasses : ComponentClasses
{
    /// <summary>
    /// Targets the dialog wrapper element.
    /// </summary>
    public string Dialog { get; init; }
}

/// <summary>
/// Component styles for <see cref="ModalContent"/>.
/// </summary>
public sealed record class ModalContentStyles : ComponentStyles
{
    /// <summary>
    /// Targets the dialog wrapper element.
    /// </summary>
    public string Dialog { get; init; }
}