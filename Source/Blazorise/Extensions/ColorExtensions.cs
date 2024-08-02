namespace Blazorise.Extensions;

/// <summary>
/// Helper methods for colors.
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Determines if the supplied color is null or empty, i.e. not containing any color.
    /// </summary>
    /// <param name="color">Color value to check.</param>
    /// <returns>True if color is <c>null</c> or <see cref="Color.Default"/>.</returns>
    public static bool IsNullOrDefault( this Color color )
    {
        return color == null || color == Color.Default;
    }

    /// <summary>
    /// Determines if the supplied color is defined, i.e. not null or empty.
    /// </summary>
    /// <param name="color">Color value to check.</param>
    /// <returns>True if color is not <c>null</c> or <see cref="Color.Default"/>.</returns>
    public static bool IsNotNullOrDefault( this Color color )
    {
        return color != null && color != Color.Default;
    }

    /// <summary>
    /// Determines if the supplied background color is null or empty, i.e. not containing any color.
    /// </summary>
    /// <param name="background">Color value to check.</param>
    /// <returns>True if background color is <c>null</c> or <see cref="Background.Default"/>.</returns>
    public static bool IsNullOrDefault( this Background background )
    {
        return background == null || background == Background.Default;
    }
}
