using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class ProgressContrastComponentTest : BunitContext
{
    public ProgressContrastComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop.AddBlazoriseUtilities();
    }

    [Theory]
    [InlineData( "#DBB5E6" )]
    [InlineData( "#123" )]
    [InlineData( "rgb(219 181 230)" )]
    [InlineData( "hsl(0 0% 100%)" )]
    [InlineData( "var(--accent,#DBB5E6)" )]
    [InlineData( "transparent" )]
    [InlineData( "#DBB5E680" )]
    [InlineData( "rgba(219,181,230,0.5)" )]
    public void CustomColors_DelegateAppearanceToProviderStylesWithTheThemeEnabled( string color )
    {
        Theme theme = new() { ColorOptions = new() { Primary = "#7c3aed" } };
        IRenderedComponent<ThemeProvider> component = Render<ThemeProvider>( parameters => parameters
            .Add( parameter => parameter.Theme, theme )
            .AddChildContent<Progress>( progress => progress
                .Add( parameter => parameter.Color, new Color( color ) )
                .Add( parameter => parameter.Value, 65 ) ) );

        string styles = component.Find( ".progress-bar" ).GetAttribute( "style" );
        Assert.Contains( "progress-bar-custom", component.Find( ".progress-bar" ).ClassList );
        Assert.Contains( $"--bs-progress-bar-bg: {color}", styles );
        Assert.DoesNotContain( "color:", styles );
        Assert.DoesNotContain( "contrast-color(", styles );
        Assert.NotEmpty( component.Find( "#b-theme-styles" ).TextContent );
    }

    [Fact]
    public async Task StackedBars_PreserveCustomColorWhenThemeChangesOrIsDisabled()
    {
        Theme theme = new() { ColorOptions = new() { Primary = "#7c3aed" } };
        IRenderedComponent<ThemeProvider> component = Render<ThemeProvider>( parameters => parameters
            .Add( parameter => parameter.Theme, theme )
            .AddChildContent<Progress>( progress => progress
                .AddChildContent<ProgressBar>( bar => bar
                    .Add( parameter => parameter.Color, new Color( "#DBB5E6" ) )
                    .Add( parameter => parameter.Value, 25 ) ) ) );

        string styles = component.Find( ".progress-bar" ).GetAttribute( "style" );
        Assert.Contains( "--bs-progress-bar-bg: #DBB5E6", styles );
        Assert.DoesNotContain( "color:", styles );

        await component.InvokeAsync( () =>
        {
            theme.ColorOptions.Primary = "#ff0000";
            theme.ThemeHasChanged();
        } );

        component.WaitForAssertion( () => Assert.Equal( styles, component.Find( ".progress-bar" ).GetAttribute( "style" ) ) );

        await component.InvokeAsync( () =>
        {
            theme.Enabled = false;
            theme.ThemeHasChanged();
        } );

        component.WaitForAssertion( () =>
        {
            Assert.Empty( component.FindAll( "#b-theme-styles" ) );
            Assert.Equal( styles, component.Find( ".progress-bar" ).GetAttribute( "style" ) );
            Assert.Contains( "progress-bar-custom", component.Find( ".progress-bar" ).ClassList );
        } );
    }

    [Theory]
    [InlineData( "#DBB5E6" )]
    [InlineData( "var(--accent,#DBB5E6)" )]
    public void ExplicitTextColor_TakesPrecedenceAndCanBeRemoved( string color )
    {
        IRenderedComponent<Progress> component = Render<Progress>( parameters => parameters
            .Add( parameter => parameter.Color, new Color( color ) )
            .Add( parameter => parameter.TextColor, TextColor.Danger )
            .Add( parameter => parameter.Value, 65 ) );

        Assert.Contains( "text-danger", component.Find( ".progress" ).ClassList );
        Assert.Contains( "color:inherit", component.Find( ".progress-bar" ).GetAttribute( "style" ) );
        Assert.Contains( "progress-bar-custom", component.Find( ".progress-bar" ).ClassList );

        component.Render( parameters => parameters.Add( parameter => parameter.TextColor, TextColor.Default ) );

        Assert.DoesNotContain( "text-danger", component.Find( ".progress" ).ClassList );
        Assert.DoesNotContain( "color:inherit", component.Find( ".progress-bar" ).GetAttribute( "style" ) );
        Assert.DoesNotContain( "color:", component.Find( ".progress-bar" ).GetAttribute( "style" ) );
        Assert.Contains( "progress-bar-custom", component.Find( ".progress-bar" ).ClassList );
    }

    [Fact]
    public void StackedBar_ExplicitCssTextColorOverridesProviderStyles()
    {
        IRenderedComponent<Progress> component = Render<Progress>( parameters => parameters
            .AddChildContent<ProgressBar>( bar => bar
                .Add( parameter => parameter.Color, new Color( "#DBB5E6" ) )
                .Add( parameter => parameter.TextColor, new TextColor( "#123456" ) )
                .Add( parameter => parameter.Value, 25 ) ) );
        IRenderedComponent<ProgressBar> bar = component.FindComponent<ProgressBar>();

        Assert.Contains( "progress-bar-custom", bar.Find( ".progress-bar" ).ClassList );
        Assert.Contains( "color:#123456 !important", bar.Find( ".progress-bar" ).GetAttribute( "style" ) );

        bar.Render( parameters => parameters.Add( parameter => parameter.TextColor, TextColor.Default ) );

        Assert.DoesNotContain( "color:#123456", bar.Find( ".progress-bar" ).GetAttribute( "style" ) );
        Assert.Contains( "progress-bar-custom", bar.Find( ".progress-bar" ).ClassList );
    }
}