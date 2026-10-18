#region Using directives
using Microsoft.Extensions.DependencyInjection;
using Xunit;
#endregion

namespace Blazorise.Tests;

public class ThemeRegistrationTest
{
    [Fact]
    public void AddBlazoriseTheme_ShouldConfigureEachScopeOnce()
    {
        // setup
        var services = new ServiceCollection();
        var configureCount = 0;

        services.AddBlazoriseTheme( theme =>
        {
            configureCount++;
            theme.ColorOptions = new() { Primary = "#312E81" };
        } );

        using ( var serviceProvider = services.BuildServiceProvider( validateScopes: true ) )
        using ( var firstScope = serviceProvider.CreateScope() )
        using ( var secondScope = serviceProvider.CreateScope() )
        {
            // test
            var firstTheme = firstScope.ServiceProvider.GetRequiredService<Theme>();
            var sameTheme = firstScope.ServiceProvider.GetRequiredService<Theme>();
            var secondTheme = secondScope.ServiceProvider.GetRequiredService<Theme>();

            // validate
            Assert.Equal( 2, configureCount );
            Assert.Same( firstTheme, sameTheme );
            Assert.NotSame( firstTheme, secondTheme );
            Assert.Equal( "#312E81", firstTheme.ColorOptions.Primary );

            firstTheme.ColorOptions.Primary = "#7C3AED";

            Assert.Equal( "#312E81", secondTheme.ColorOptions.Primary );
        }
    }

    [Fact]
    public void AddBlazoriseTheme_ShouldCreateOneThemePerScopeUsingScopedDependencies()
    {
        // setup
        var services = new ServiceCollection();
        services.AddScoped( _ => new ThemeColorOptions { Primary = "#312E81" } );
        services.AddBlazoriseTheme( serviceProvider => new Theme
        {
            ColorOptions = serviceProvider.GetRequiredService<ThemeColorOptions>(),
        } );

        using ( var serviceProvider = services.BuildServiceProvider( validateScopes: true ) )
        using ( var firstScope = serviceProvider.CreateScope() )
        using ( var secondScope = serviceProvider.CreateScope() )
        {
            // test
            var firstTheme = firstScope.ServiceProvider.GetRequiredService<Theme>();
            var secondTheme = secondScope.ServiceProvider.GetRequiredService<Theme>();

            // validate
            Assert.Same( firstTheme, firstScope.ServiceProvider.GetRequiredService<Theme>() );
            Assert.Same( firstScope.ServiceProvider.GetRequiredService<ThemeColorOptions>(), firstTheme.ColorOptions );
            Assert.NotSame( firstTheme, secondTheme );

            firstTheme.ColorOptions.Primary = "#7C3AED";

            Assert.Equal( "#312E81", secondTheme.ColorOptions.Primary );
        }
    }
}