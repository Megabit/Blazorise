namespace Blazorise
{
    public interface IThemeCache
    {
        void CacheVariables( Theme theme, string variables );
        void CacheStyles( Theme theme, string styles );

        bool TryGetStylesFromCache( Theme theme, out string styles );
        bool TryGetVariablesFromCache( Theme theme, out string variables );
    }
}
