using Blazorise.Themes;
using Xunit;

namespace Blazorise.Tests
{
    public class ThemeCacheTest
    {
        private readonly ThemeCache themeCache;

        public ThemeCacheTest()
        {
            themeCache = new( new( null, options => { } ) );
        }

        [Fact]
        public void TryGetVariablesFromCache_ReturnsVariablesCachedBy_CacheVariables()
        {
            // setup
            var theme = new Theme();

            // test
            themeCache.CacheVariables( theme, "xyz" );
            var success = themeCache.TryGetVariablesFromCache( theme, out var variables );

            // validate
            Assert.True( success );
            Assert.Equal( "xyz", variables );
        }

        [Fact]
        public void TryGetStylesFromCache_ReturnsStylesCachedBy_CacheStyles()
        {
            // setup
            var theme = new Theme();

            // test
            themeCache.CacheStyles( theme, "xyz" );
            var success = themeCache.TryGetStylesFromCache( theme, out var styles );

            // validate
            Assert.True( success );
            Assert.Equal( "xyz", styles );
        }

        [Fact]
        public void CacheStyles_CachesUpToAFixedAmountOfThemes()
        {
            // setup
            var maxCachedStyles = 10;

            var firstCachedTheme = new Theme();
            themeCache.CacheStyles( firstCachedTheme, "xyz" );

            // test
            for ( var i = 0; i < maxCachedStyles - 1; i++ )
            {
                themeCache.CacheStyles( new() { Black = i.ToString() }, i.ToString() );
            }
            var success = themeCache.TryGetStylesFromCache( firstCachedTheme, out var styles );

            // validate
            Assert.True( success );
            Assert.Equal( "xyz", styles );
        }

        [Fact]
        public void CacheStyles_CachesNoMoreThanAFixedAmountOfThemes()
        {
            // setup
            var maxCachedStyles = 10;

            var firstCachedTheme = new Theme();
            themeCache.CacheStyles( firstCachedTheme, "xyz" );

            // test
            for ( var i = 0; i < maxCachedStyles; i++ )
            {
                themeCache.CacheStyles( new() { Black = i.ToString() }, i.ToString() );
            }
            var success = themeCache.TryGetStylesFromCache( firstCachedTheme, out var styles );

            // validate
            Assert.False( success );
            Assert.Null( styles );
        }

        [Fact]
        public void CacheStyles_ExpelsLastUsedTheme_IfCacheIsFull()
        {
            // setup
            var maxCachedStyles = 10;

            var firstCachedTheme = new Theme() { Black = "0" };
            var secondCachedTheme = new Theme() { Black = "1" };

            for ( var i = 0; i < maxCachedStyles; i++ )
            {
                themeCache.CacheStyles( new() { Black = i.ToString() }, i.ToString() );
            }

            _ = themeCache.TryGetStylesFromCache( firstCachedTheme, out _ );
            themeCache.CacheStyles( new() { Black = "10" }, "10" );

            // test
            var firstThemeStillInCache = themeCache.TryGetStylesFromCache( firstCachedTheme, out _ );
            var secondThemeStillInCache = themeCache.TryGetStylesFromCache( secondCachedTheme, out _ );

            // validate
            Assert.True( firstThemeStillInCache );
            Assert.False( secondThemeStillInCache );
        }

        [Fact]
        public void EqualThemes_AreEqual()
        {
            // setup
            var theme1 = new Theme();
            var theme2 = new Theme();

            // test
            var areEqual = theme1.Equals( theme2 );

            // validate
            Assert.True( areEqual );
        }

        [Fact]
        public void UnequalThemes_AreNotEqual()
        {
            // setup
            var theme1 = new Theme() { TextColorOptions = new() { Danger = "danger" } };
            var theme2 = new Theme() { TextColorOptions = new() { Danger = "warning" } };

            // test
            var areEqual = theme1.Equals( theme2 );

            // validate
            Assert.False( areEqual );
        }
    }
}
