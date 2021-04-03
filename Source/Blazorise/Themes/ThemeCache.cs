using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blazorise.Interfaces;

namespace Blazorise.Themes
{
    public class ThemeCache : IThemeCache
    {
        #region Fields

        private readonly int cacheSize;

        private readonly Queue<Theme> cachedThemes = new();

        private readonly Dictionary<Theme, string> variableCache = new();
        private readonly Dictionary<Theme, string> styleCache = new();

        private readonly object mutex = new();

        #endregion

        #region Constructors

        public ThemeCache(BlazoriseOptions options )
        {
            cacheSize = options.ThemeCacheSize;
        }

        #endregion

        #region Methods

        public void CacheVariables( Theme theme, string variables )
        {
            if ( cacheSize < 1 )
                return;

            lock ( mutex )
            {
                PrepareCache( theme );

                variableCache[theme] = variables;
            }
        }

        public void CacheStyles( Theme theme, string styles )
        {
            if ( cacheSize < 1 )
                return;

            lock ( mutex )
            {
                PrepareCache( theme );

                styleCache[theme] = styles;
            }
        }

        private void PrepareCache(Theme theme)
        {
            if ( !cachedThemes.Contains( theme ) )
            {
                cachedThemes.Enqueue( theme );

                if ( cachedThemes.Count > cacheSize )
                {
                    if ( cachedThemes.TryDequeue( out var uncachedTheme ) )
                    {
                        variableCache.Remove( uncachedTheme );
                        styleCache.Remove( uncachedTheme );
                    }
                }
            }
        }

        public bool TryGetVariablesFromCache( Theme theme, out string variables )
        {
            lock ( mutex )
            {
                variables = variableCache.GetValueOrDefault( theme );
                return variables != null;
            }
        }

        public bool TryGetStylesFromCache( Theme theme, out string styles )
        {
            lock ( mutex )
            {
                styles = styleCache.GetValueOrDefault( theme );
                return styles != null;
            }
        }

        #endregion
    }
}
