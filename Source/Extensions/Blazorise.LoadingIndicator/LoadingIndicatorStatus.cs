namespace Blazorise.LoadingIndicator;

/// <summary>
/// Represents the current status shown by a <see cref="LoadingIndicator"/>.
/// </summary>
/// <param name="Text">Optional status text.</param>
/// <param name="Progress">Optional progress value.</param>
public sealed record LoadingIndicatorStatus( string Text = null, int? Progress = null )
{
    /// <summary>
    /// Gets an empty status instance.
    /// </summary>
    public static LoadingIndicatorStatus Empty { get; } = new LoadingIndicatorStatus();
}