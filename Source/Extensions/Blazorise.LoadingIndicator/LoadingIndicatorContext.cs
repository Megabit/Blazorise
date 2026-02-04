namespace Blazorise.LoadingIndicator;

/// <summary>
/// Represents the context data passed to <see cref="LoadingIndicator.IndicatorTemplate"/>.
/// </summary>
/// <param name="Text">Optional status text.</param>
/// <param name="Progress">Optional progress value.</param>
public sealed record LoadingIndicatorContext( string Text = null, int? Progress = null )
{
    /// <summary>
    /// Gets an empty context instance.
    /// </summary>
    public static LoadingIndicatorContext Empty { get; } = new LoadingIndicatorContext();
}