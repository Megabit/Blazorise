#region Using directives
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Blazorise.Themes;

/// <summary>
/// Default implementation of <see cref="IThemeCache"/>.
/// </summary>
public class ThemeCache : IThemeCache
{
    #region Members

    private readonly int maxCacheSize;

    private readonly Dictionary<int, ThemeCachedResource> cachedThemes = new();

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
            var cacheHash = theme.GetHashCode();
            PrepareCache( cacheHash );

            cachedThemes[cacheHash].Variables = variables;
        }
    }

    /// <inheritdoc/>
    public void CacheStyles( Theme theme, string styles )
    {
        if ( maxCacheSize < 1 )
            return;

        lock ( mutex )
        {
            var cacheHash = theme.GetHashCode();
            PrepareCache( cacheHash );

            cachedThemes[cacheHash].Styles = styles;
        }
    }

    private void PrepareCache( int themeCacheKey )
    {
        if ( !cachedThemes.ContainsKey( themeCacheKey ) )
        {
            if ( cachedThemes.Count + 1 > maxCacheSize )
                UncacheTheme();

            cachedThemes.Add( themeCacheKey, new() );
        }
    }

    private void UncacheTheme()
    {
        cachedThemes.Remove( cachedThemes.First().Key );
    }

    /// <inheritdoc/>
    public bool TryGetVariablesFromCache( Theme theme, out string variables )
    {
        lock ( mutex )
        {
            variables = cachedThemes.GetValueOrDefault( theme.GetHashCode() )?.Variables;
            return variables is not null;
        }
    }

    /// <inheritdoc/>
    public bool TryGetStylesFromCache( Theme theme, out string styles )
    {
        lock ( mutex )
        {
            styles = cachedThemes.GetValueOrDefault( theme.GetHashCode() )?.Styles;
            return styles is not null;
        }
    }

    #endregion

    #region Classes
    /// <summary>
    /// Holds a cached resource
    /// </summary>
    private sealed record ThemeCachedResource
    {
        public string Variables { get; set; }
        public string Styles { get; set; }
    }

    #endregion
}