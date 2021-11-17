namespace Blazorise
{
    /// <summary>
    /// Represents a cache for a <see cref="IThemeGenerator"/>.
    /// </summary>  
    public interface IThemeCache
    {
        /// <summary>
        /// Caches a theme and it's built CSS variables.
        /// </summary>
        /// <param name="theme">Theme to cache.</param>
        /// <param name="variables">CSS variables to cache.</param>
        void CacheVariables( Theme theme, string variables );

        /// <summary>
        /// Caches a theme and it's built CSS styles.
        /// </summary>
        /// <param name="theme">Theme to cache.</param>
        /// <param name="styles">CSS styles to cache.</param>
        void CacheStyles( Theme theme, string styles );

        /// <summary>
        /// Gets the CSS variables associated with the specified theme.
        /// </summary>
        /// <param name="theme">Cached theme.</param>
        /// <param name="variables">When this method returns, contains the variables associated with the specified key.</param>
        /// <returns>true if the cache contains an element with the specified theme; otherwise, false.</returns>
        bool TryGetVariablesFromCache( Theme theme, out string variables );

        /// <summary>
        /// Gets the CSS styles associated with the specified theme.
        /// </summary>
        /// <param name="theme">Cached theme.</param>
        /// <param name="styles">When this method returns, contains the styles associated with the specified key.</param>
        /// <returns>true if the cache contains an element with the specified theme; otherwise, false.</returns>
        bool TryGetStylesFromCache( Theme theme, out string styles );
    }
}
