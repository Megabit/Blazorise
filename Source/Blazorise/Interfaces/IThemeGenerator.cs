namespace Blazorise
{
    /// <summary>
    /// Represents a theme generator for a CSS variables and styles.
    /// </summary>
    public interface IThemeGenerator
    {
        /// <summary>
        /// Generates a CSS variables based on the defined <see cref="Theme"/> options.
        /// </summary>
        /// <param name="theme">Theme options.</param>
        /// <returns>CSS variables.</returns>
        string GenerateVariables( Theme theme );

        /// <summary>
        /// Generates a CSS styles based on the defined <see cref="Theme"/> options.
        /// </summary>
        /// <param name="theme">Theme options.</param>
        /// <returns>CSS styles.</returns>
        string GenerateStyles( Theme theme );
    }
}
