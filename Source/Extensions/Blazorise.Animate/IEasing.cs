namespace Blazorise.Animate;

/// <summary>
/// Supplies the timing curve used by an <see cref="Animate"/> transition.
/// </summary>
public interface IEasing
{
    /// <summary>
    /// CSS easing identifier associated with the curve.
    /// </summary>
    string Name { get; }
}