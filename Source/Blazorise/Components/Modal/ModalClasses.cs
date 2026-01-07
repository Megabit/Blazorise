namespace Blazorise;

/// <summary>
/// Component classes for <see cref="Modal"/>.
/// </summary>
public sealed record class ModalClasses : ComponentClasses
{
    /// <summary>
    /// Targets the modal backdrop element.
    /// </summary>
    public string Backdrop { get; init; }
}

/// <summary>
/// Component styles for <see cref="Modal"/>.
/// </summary>
public sealed record class ModalStyles : ComponentStyles
{
    /// <summary>
    /// Targets the modal backdrop element.
    /// </summary>
    public string Backdrop { get; init; }
}