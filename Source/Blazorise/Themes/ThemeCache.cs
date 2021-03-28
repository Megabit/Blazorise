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

        private const int cacheSize = 10;

        private readonly Queue<Theme> cachedThemes = new();

        private readonly Dictionary<Theme, string> variableCache = new();
        private readonly Dictionary<Theme, string> styleCache = new();

        private readonly object mutex = new();

        #endregion

        #region Methods

        public void CacheVariables( Theme theme, string variables )
        {
            lock ( mutex )
            {
                PrepareCache( theme );

                variableCache[theme] = variables;
            }
        }

        public void CacheStyles( Theme theme, string styles )
        {
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
