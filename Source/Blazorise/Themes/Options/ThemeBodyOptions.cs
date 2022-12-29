namespace Blazorise;

/// <summary>
/// Defines the body theme options.
/// </summary>
public record ThemeBodyOptions
{
    /// <summary>
    /// Gets or sets the body background color.
    /// </summary>
    /// <remarks>
    /// If defined it will also override any <see cref="Background.Body"/>.
    /// </remarks>
    public string BackgroundColor { get; set; }

    /// <summary>
    /// Gets or sets the body text color.
    /// </summary>
    /// <remarks>
    /// If defined it will also override any <see cref="TextColor.Body"/>.
    /// </remarks>
    public string TextColor { get; set; }
}