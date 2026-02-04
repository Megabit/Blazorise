namespace Blazorise;

/// <summary>
/// Defines the theme options for the SpinKit component.
/// </summary>
public record ThemeSpinKitOptions
{
    /// <summary>
    /// SpinKit color variant.
    /// </summary>
    public Color Color { get; set; } = Blazorise.Color.Default;

    /// <summary>
    /// SpinKit size.
    /// </summary>
    public string Size { get; set; }
}