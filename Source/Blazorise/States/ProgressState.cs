namespace Blazorise.States;

/// <summary>
/// Holds the information about the current state of the <see cref="Progress"/> component.
/// </summary>
public record ProgressState
{
    /// <summary>
    /// Defines the progress bar color.
    /// </summary>
    public Color Color { get; init; }

    /// <summary>
    /// Size of the progress bar.
    /// </summary>
    public Size? Size { get; init; }

    /// <summary>
    /// Set to true to make the progress bar stripped.
    /// </summary>
    public bool Striped { get; init; }

    /// <summary>
    /// Set to true to make the progress bar animated.
    /// </summary>
    public bool Animated { get; init; }

    /// <summary>
    /// Set to true to show that an operation is being executed.
    /// </summary>
    public bool Indeterminate { get; init; }

    /// <summary>
    /// Minimum value of the progress bar.
    /// </summary>
    public int Min { get; init; }

    /// <summary>
    /// Maximum value of the progress bar.
    /// </summary>
    public int Max { get; init; }

    /// <summary>
    /// The progress value.
    /// </summary>
    public int? Value { get; init; }
}