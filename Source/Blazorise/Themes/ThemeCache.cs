#region Using directives
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Themes
{
    /// <summary>
    /// Default implementation of <see cref="IThemeCache"/>.
    /// </summary>
    public class ThemeCache : IThemeCache
    {
        #region Members

        private readonly int maxCacheSize;

        private readonly List<Theme> cachedThemes = new();

        private readonly Dictionary<Theme, string> variableCache = new();
        private readonly Dictionary<Theme, string> styleCache = new();

        private readonly object mutex = new();

        #endregion

        #region Constructors

        /// <summary>
        /// A default <see cref="ThemeCache"/> constructor.
        /// </summary>
        /// <param name="options">Blazorise global settings.</param>
        public ThemeCache( BlazoriseOptions options )
        {
            maxCacheSize = options.ThemeCacheSize;
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public void CacheVariables( Theme theme, string variables )
        {
            if ( maxCacheSize < 1 )
                return;

            lock ( mutex )
            {
                PrepareCache( theme );

                variableCache[theme] = variables;
            }
        }

        /// <inheritdoc/>
        public void CacheStyles( Theme theme, string styles )
        {
            if ( maxCacheSize < 1 )
                return;

            lock ( mutex )
            {
                PrepareCache( theme );

                styleCache[theme] = styles;
            }
        }

        private void PrepareCache( Theme theme )
        {
            if ( !cachedThemes.Contains( theme ) )
            {
                cachedThemes.Add( theme );

                if ( cachedThemes.Count > maxCacheSize )
                {
                    UncacheTheme();
                }
            }
        }

        private void UncacheTheme()
        {
            var uncachedTheme = cachedThemes.FirstOrDefault();

            if ( uncachedTheme != null )
            {
                cachedThemes.Remove( uncachedTheme );
                variableCache.Remove( uncachedTheme );
                styleCache.Remove( uncachedTheme );
            }
        }

        /// <inheritdoc/>
        public bool TryGetVariablesFromCache( Theme theme, out string variables )
        {
            lock ( mutex )
            {
                MoveLastRecentlyUsedThemeToBackOfList( theme );

                variables = variableCache.GetValueOrDefault( theme );
                return variables != null;
            }
        }

        /// <inheritdoc/>
        public bool TryGetStylesFromCache( Theme theme, out string styles )
        {
            lock ( mutex )
            {
                MoveLastRecentlyUsedThemeToBackOfList( theme );

                styles = styleCache.GetValueOrDefault( theme );
                return styles != null;
            }
        }

        private void MoveLastRecentlyUsedThemeToBackOfList( Theme lastUsedTheme )
        {
            if ( cachedThemes.Contains( lastUsedTheme ) )
            {
                cachedThemes.Remove( lastUsedTheme );
                cachedThemes.Add( lastUsedTheme );
            }
        }

        #endregion
    }
}
