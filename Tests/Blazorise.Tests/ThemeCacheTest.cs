using System;
using System.Collections.Generic;
using System.Linq;
using Blazorise.DataGrid.Utils;
using Blazorise.Themes;
using Xunit;

namespace Blazorise.Tests;

public class ThemeCacheTest
{
    private readonly ThemeCache themeCache;

    public ThemeCacheTest()
    {
        themeCache = new( new( null, options => { } ) );
    }


    private Theme InitFullyInstantiatedTheme()
        => RecursiveObjectActivator.CreateInstance<Theme>();

    [Fact]
    public void TryGetVariablesFromCache_ReturnsVariablesCachedBy_CacheVariables()
    {
        // setup
        var theme = InitFullyInstantiatedTheme();

        // test
        themeCache.CacheVariables( theme, "xyz" );
        var success = themeCache.TryGetVariablesFromCache( theme, out var variables );

        // validate
        Assert.True( success );
        Assert.Equal( "xyz", variables );
    }

    [Fact]
    public void TryGetVariablesFromCache_ReturnsVariablesCachedBy_AnEqualTheme_CacheVariables()
    {
        // setup
        var theme = InitFullyInstantiatedTheme();
        var theme2 = InitFullyInstantiatedTheme();

        // test
        themeCache.CacheVariables( theme, "xyz" );
        var success = themeCache.TryGetVariablesFromCache( theme2, out var variables );

        // validate
        Assert.True( success );
        Assert.Equal( "xyz", variables );
    }

    [Fact]
    public void TryGetStylesFromCache_ReturnsStylesCachedBy_CacheStyles()
    {
        // setup
        var theme = InitFullyInstantiatedTheme();

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

        var firstCachedTheme = InitFullyInstantiatedTheme();
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

        var firstCachedTheme = InitFullyInstantiatedTheme();
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
        Assert.False( firstThemeStillInCache );
        Assert.True( secondThemeStillInCache );
    }

    [Fact]
    public void DictionaryCache_RemovesSuccessfully()
    {
        // setup
        Dictionary<Theme, string> cache = new();
        var theme = InitFullyInstantiatedTheme();
        cache.Add( theme with { }, "" );

        cache.Remove( cache.First().Key );

        // validate
        Assert.Empty( cache );
    }


    [Fact]
    public void EqualThemes_FullyInstiated_AreEqual()
    {
        // setup
        var theme1 = InitFullyInstantiatedTheme();
        theme1.Changed += OnThemeChanged;
        var theme2 = InitFullyInstantiatedTheme();
        theme2.Changed += OnThemeChanged;

        // validate
        Assert.Equal( theme1, theme2 );
        Assert.Equal( theme1.GetHashCode(), theme2.GetHashCode() );
    }

    [Fact]
    public void EqualThemes_AreEqual()
    {
        // setup
        var theme1 = new Theme();
        theme1.Changed += OnThemeChanged;
        var theme2 = new Theme();
        theme2.Changed += OnThemeChanged;

        // validate
        Assert.Equal( theme1, theme2 );
        Assert.Equal( theme1.GetHashCode(), theme2.GetHashCode() );
    }

    [Fact]
    public void EqualThemes_DifferentChangedEvent_AreEqual()
    {
        // setup
        var theme1 = InitFullyInstantiatedTheme();
        var theme2 = InitFullyInstantiatedTheme();
        theme2.Changed += OnThemeChanged;

        // validate
        Assert.Equal( theme1, theme2 );
    }

    private void OnThemeChanged( object sender, EventArgs eventArgs )
    {

    }

    [Fact]
    public void UnequalThemes_AreNotEqual()
    {
        // setup
        var theme1 = new Theme() { TextColorOptions = new() { Danger = "danger" } };
        var theme2 = new Theme() { TextColorOptions = new() { Danger = "warning" } };

        // validate
        Assert.NotEqual( theme1, theme2 );
    }
}