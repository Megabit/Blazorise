namespace Blazorise.Extensions;

/// <summary>
/// Helper methods for intents.
/// </summary>
public static class IntentExtensions
{
    /// <summary>
    /// Determines if the supplied intent is null or empty, i.e. not containing any intent.
    /// </summary>
    /// <param name="intent">Intent value to check.</param>
    /// <returns>True if intent is <c>null</c> or <see cref="Intent.Default"/>.</returns>
    public static bool IsNullOrDefault( this Intent intent )
    {
        return intent is null || intent == Intent.Default;
    }

    /// <summary>
    /// Determines if the supplied intent is defined, i.e. not null or empty.
    /// </summary>
    /// <param name="intent">Intent value to check.</param>
    /// <returns>True if intent is not <c>null</c> or <see cref="Intent.Default"/>.</returns>
    public static bool IsNotNullOrDefault( this Intent intent )
    {
        return intent is not null && intent != Intent.Default;
    }

    /// <summary>
    /// Converts the supplied intent into a color representation.
    /// </summary>
    /// <param name="intent">Intent value to convert.</param>
    /// <returns>Color instance representing the supplied intent.</returns>
    public static Color ToColor( this Intent intent )
    {
        return intent.IsNullOrDefault() ? Color.Default : new Color( intent.Name );
    }

    /// <summary>
    /// Converts the supplied color into an intent representation.
    /// </summary>
    /// <param name="color">Color value to convert.</param>
    /// <returns>Intent instance representing the supplied color.</returns>
    public static Intent ToIntent( this Color color )
    {
        return color is null || color == Color.Default ? Intent.Default : new Intent( color.Name );
    }
}