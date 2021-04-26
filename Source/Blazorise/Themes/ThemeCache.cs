#region Using directives
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Themes
{
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

        public ThemeCache( BlazoriseOptions options )
        {
            maxCacheSize = options.ThemeCacheSize;
        }

        #endregion

        #region Methods

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

        public bool TryGetVariablesFromCache( Theme theme, out string variables )
        {
            lock ( mutex )
            {
                MoveLastRecentlyUsedThemeToBackOfList( theme );

                variables = variableCache.GetValueOrDefault( theme );
                return variables != null;
            }
        }

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
