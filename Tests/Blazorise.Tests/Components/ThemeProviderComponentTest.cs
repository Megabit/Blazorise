#region Using directives
using System.Threading.Tasks;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class ThemeProviderComponentTest : BunitContext
{
    public ThemeProviderComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop.AddBlazoriseUtilities();
    }

    [Fact]
    public async Task RegisteredTheme_ShouldCascadeAndNotifyIndependentRootComponents()
    {
        Services.AddBlazoriseTheme( theme => theme.ColorOptions = new() { Primary = "#312E81" } );

        var firstComponent = Render<ThemeConsumerComponent>();
        var secondComponent = Render<ThemeConsumerComponent>();
        var theme = Services.GetRequiredService<Theme>();

        Assert.Same( theme, firstComponent.Instance.Theme );
        Assert.Same( theme, secondComponent.Instance.Theme );
        Assert.Equal( "#312E81", firstComponent.Find( "#theme-primary" ).TextContent );
        Assert.Equal( "#312E81", secondComponent.Find( "#theme-primary" ).TextContent );

        await firstComponent.InvokeAsync( () =>
        {
            theme.ColorOptions.Primary = "#7C3AED";
            theme.ThemeHasChanged();
        } );

        firstComponent.WaitForAssertion( () => Assert.Equal( "#7C3AED", firstComponent.Find( "#theme-primary" ).TextContent ) );
        secondComponent.WaitForAssertion( () => Assert.Equal( "#7C3AED", secondComponent.Find( "#theme-primary" ).TextContent ) );
    }

    [Fact]
    public async Task Provider_ShouldGenerateAndUpdateCssFromRegisteredTheme()
    {
        Services.AddBlazoriseTheme( theme => theme.ColorOptions = new() { Primary = "#312E81" } );

        var component = Render<ThemeProvider>( parameters => parameters.AddChildContent<ThemeConsumerComponent>() );
        var theme = Services.GetRequiredService<Theme>();

        Assert.Same( theme, component.FindComponent<ThemeConsumerComponent>().Instance.Theme );
        Assert.Contains( "#312E81", component.Find( "#b-theme-variables" ).TextContent );
        Assert.Contains( "#312E81", component.Find( "#b-theme-styles" ).TextContent );

        await component.InvokeAsync( () =>
        {
            theme.ColorOptions.Primary = "#7C3AED";
            theme.ThemeHasChanged();
        } );

        component.WaitForAssertion( () =>
        {
            Assert.Contains( "#7C3AED", component.Find( "#b-theme-variables" ).TextContent );
            Assert.Contains( "#7C3AED", component.Find( "#b-theme-styles" ).TextContent );
        } );
    }

    [Fact]
    public async Task ExplicitTheme_ShouldOverrideRegisteredThemeAndAllowReturningToIt()
    {
        Services.AddBlazoriseTheme( theme => theme.ColorOptions = new() { Primary = "#312E81" } );

        var explicitTheme = new Theme
        {
            ColorOptions = new() { Primary = "#DBB5E6" },
        };

        var component = Render<ThemeProvider>( parameters => parameters
            .Add( provider => provider.Theme, explicitTheme )
            .AddChildContent<ThemeConsumerComponent>() );

        var rootComponent = Render<ThemeConsumerComponent>();
        var registeredTheme = Services.GetRequiredService<Theme>();

        Assert.Same( explicitTheme, component.FindComponent<ThemeConsumerComponent>().Instance.Theme );
        Assert.Same( registeredTheme, rootComponent.Instance.Theme );
        Assert.Contains( "#DBB5E6", component.Find( "#b-theme-variables" ).TextContent );

        await component.InvokeAsync( () =>
        {
            explicitTheme.ColorOptions.Primary = "#0F766E";
            explicitTheme.ThemeHasChanged();
        } );

        component.WaitForAssertion( () => Assert.Contains( "#0F766E", component.Find( "#b-theme-variables" ).TextContent ) );

        await rootComponent.InvokeAsync( () =>
        {
            registeredTheme.ColorOptions.Primary = "#7C3AED";
            registeredTheme.ThemeHasChanged();
        } );

        rootComponent.WaitForAssertion( () => Assert.Equal( "#7C3AED", rootComponent.Find( "#theme-primary" ).TextContent ) );
        Assert.Same( explicitTheme, component.FindComponent<ThemeConsumerComponent>().Instance.Theme );
        Assert.Contains( "#0F766E", component.Find( "#b-theme-variables" ).TextContent );

        component.Render( parameters => parameters.Add( provider => provider.Theme, (Theme)null ) );

        Assert.Same( registeredTheme, component.FindComponent<ThemeConsumerComponent>().Instance.Theme );
        Assert.Contains( "#7C3AED", component.Find( "#b-theme-variables" ).TextContent );

        var renderCount = component.RenderCount;

        await component.InvokeAsync( explicitTheme.ThemeHasChanged );

        Assert.Equal( renderCount, component.RenderCount );

        await component.InvokeAsync( () =>
        {
            registeredTheme.ColorOptions.Primary = "#312E81";
            registeredTheme.ThemeHasChanged();
        } );

        component.WaitForAssertion( () => Assert.Contains( "#312E81", component.Find( "#b-theme-variables" ).TextContent ) );
    }

    [Theory]
    [InlineData( false )]
    [InlineData( true )]
    public async Task Provider_ShouldTrackReplacementThemeWithEqualSettings( bool useCascadedTheme )
    {
        var originalTheme = new Theme
        {
            ColorOptions = new() { Primary = "#312E81" },
        };

        var replacementTheme = new Theme
        {
            ColorOptions = new() { Primary = "#312E81" },
        };

        var component = Render<ThemeProviderComponent>( parameters => parameters
            .Add( x => x.Theme, useCascadedTheme ? null : originalTheme )
            .Add( x => x.CascadedTheme, useCascadedTheme ? originalTheme : null ) );

        var provider = component.FindComponent<ThemeProvider>();

        component.Render( parameters => parameters
            .Add( x => x.Theme, useCascadedTheme ? null : replacementTheme )
            .Add( x => x.CascadedTheme, useCascadedTheme ? replacementTheme : null ) );

        Assert.Same( replacementTheme, component.FindComponent<ThemeConsumerComponent>().Instance.Theme );

        var renderCount = provider.RenderCount;

        await component.InvokeAsync( originalTheme.ThemeHasChanged );

        Assert.Equal( renderCount, provider.RenderCount );

        await component.InvokeAsync( () =>
        {
            replacementTheme.ColorOptions.Primary = "#7C3AED";
            replacementTheme.ThemeHasChanged();
        } );

        provider.WaitForAssertion( () =>
        {
            Assert.Contains( "#7C3AED", provider.Find( "#b-theme-variables" ).TextContent );
            Assert.Contains( "#7C3AED", provider.Find( "#b-theme-styles" ).TextContent );
            Assert.Equal( "#7C3AED", component.Find( "#theme-primary" ).TextContent );
        } );
    }

    [Fact]
    public void Provider_WithoutThemeOrRegistration_ShouldNotGenerateCss()
    {
        var component = Render<ThemeProvider>( parameters => parameters.AddChildContent<ThemeConsumerComponent>() );

        Assert.Null( component.FindComponent<ThemeConsumerComponent>().Instance.Theme );
        Assert.Empty( component.FindAll( "style" ) );
    }
}