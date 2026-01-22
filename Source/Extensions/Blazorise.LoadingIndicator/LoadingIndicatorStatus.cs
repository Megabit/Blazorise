namespace Blazorise.LoadingIndicator;

/// <summary>
/// Represents the current status shown by a <see cref="LoadingIndicator"/>.
/// </summary>
public sealed record LoadingIndicatorStatus( string Text = null, int? Progress = null )
{
    /// <summary>
    /// Gets an empty status instance.
    /// </summary>
    public static LoadingIndicatorStatus Empty { get; } = new LoadingIndicatorStatus();
}