using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class ProgressComponentTest : BunitContext
{
    public ProgressComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop.AddBlazoriseUtilities();
    }

    [Theory]
    [InlineData( "#DBB5E6" )]
    [InlineData( "rgb(219,181,230)" )]
    [InlineData( "hsl(287 50% 81%)" )]
    [InlineData( "var(--custom-progress-color)" )]
    public void Progress_CustomColorColorsTheFillAndPreservesStripes( string color )
    {
        IRenderedComponent<Progress> component = Render<Progress>( parameters => parameters
            .Add( parameter => parameter.Value, 20 )
            .Add( parameter => parameter.Color, new Color( color ) )
            .Add( parameter => parameter.Striped, true )
            .Add( parameter => parameter.Animated, true ) );

        string style = component.Find( ".progress-bar" ).GetAttribute( "style" );

        Assert.Contains( $"--bs-progress-bar-bg: {color}", style );
        Assert.Contains( "width: 20%", style );
        Assert.Contains( "progress-bar-custom", component.Find( ".progress-bar" ).ClassList );
        Assert.DoesNotContain( "--bs-progress-bar-bg", component.Find( ".progress" ).GetAttribute( "style" ) ?? string.Empty );
        Assert.DoesNotContain( color, component.Find( ".progress-bar" ).ClassName );
        Assert.Contains( "progress-bar-striped", component.Find( ".progress-bar" ).ClassList );
        Assert.Contains( "progress-bar-animated", component.Find( ".progress-bar" ).ClassList );
    }

    [Fact]
    public void Progress_ChangingColorReplacesThePreviousClassesAndStyles()
    {
        IRenderedComponent<Progress> component = Render<Progress>( parameters => parameters
            .Add( parameter => parameter.Value, 20 )
            .Add( parameter => parameter.Color, Color.Success ) );

        Assert.Contains( "bg-success", component.Find( ".progress-bar" ).ClassList );

        component.Render( parameters => parameters.Add( parameter => parameter.Color, new Color( "#DBB5E6" ) ) );

        Assert.DoesNotContain( "bg-success", component.Find( ".progress-bar" ).ClassList );
        Assert.Contains( "progress-bar-custom", component.Find( ".progress-bar" ).ClassList );
        Assert.Contains( "--bs-progress-bar-bg: #DBB5E6", component.Find( ".progress-bar" ).GetAttribute( "style" ) );

        component.Render( parameters => parameters.Add( parameter => parameter.Color, new Color( "var(--custom-progress-color)" ) ) );

        Assert.DoesNotContain( "#DBB5E6", component.Markup );
        Assert.Contains( "--bs-progress-bar-bg: var(--custom-progress-color)", component.Find( ".progress-bar" ).GetAttribute( "style" ) );

        component.Render( parameters => parameters.Add( parameter => parameter.Color, Color.Danger ) );

        Assert.Contains( "bg-danger", component.Find( ".progress-bar" ).ClassList );
        Assert.DoesNotContain( "progress-bar-custom", component.Find( ".progress-bar" ).ClassList );
        Assert.DoesNotContain( "--bs-progress-bar-bg", component.Find( ".progress-bar" ).GetAttribute( "style" ) );
        Assert.Contains( "width: 20%", component.Find( ".progress-bar" ).GetAttribute( "style" ) );
    }

    [Fact]
    public void ProgressBar_ChangingColorKeepsStackedSegmentsIndependent()
    {
        IRenderedComponent<Progress> component = Render<Progress>( parameters => parameters
            .AddChildContent<ProgressBar>( bar => bar
                .Add( parameter => parameter.Value, 15 )
                .Add( parameter => parameter.Color, Color.Success ) )
            .AddChildContent<ProgressBar>( bar => bar
                .Add( parameter => parameter.Value, 20 )
                .Add( parameter => parameter.Color, new Color( "#DBB5E6" ) ) ) );

        IRenderedComponent<ProgressBar> customBar = component.FindComponents<ProgressBar>()[1];

        Assert.Contains( "bg-success", component.FindAll( ".progress-bar" )[0].ClassList );
        Assert.DoesNotContain( "--bs-progress-bar-bg", component.FindAll( ".progress-bar" )[0].GetAttribute( "style" ) );
        Assert.Contains( "--bs-progress-bar-bg: #DBB5E6", customBar.Find( ".progress-bar" ).GetAttribute( "style" ) );
        Assert.DoesNotContain( "bg-#DBB5E6", customBar.Markup );

        customBar.Render( parameters => parameters.Add( parameter => parameter.Color, new Color( "rgb(12,34,56)" ) ) );

        Assert.Contains( "--bs-progress-bar-bg: rgb(12,34,56)", customBar.Find( ".progress-bar" ).GetAttribute( "style" ) );
        Assert.DoesNotContain( "#DBB5E6", customBar.Markup );

        customBar.Render( parameters => parameters.Add( parameter => parameter.Color, Color.Info ) );

        Assert.Contains( "bg-info", customBar.Find( ".progress-bar" ).ClassList );
        Assert.DoesNotContain( "progress-bar-custom", customBar.Find( ".progress-bar" ).ClassList );
        Assert.DoesNotContain( "--bs-progress-bar-bg", customBar.Find( ".progress-bar" ).GetAttribute( "style" ) );
        Assert.Contains( "width: 20%", customBar.Find( ".progress-bar" ).GetAttribute( "style" ) );
        Assert.Contains( "bg-success", component.FindAll( ".progress-bar" )[0].ClassList );
    }

    [Fact]
    public void Progress_CustomContextualNameKeepsUsingProviderClasses()
    {
        IRenderedComponent<Progress> component = Render<Progress>( parameters => parameters
            .Add( parameter => parameter.Value, 20 )
            .Add( parameter => parameter.Color, new Color( "brand" ) ) );

        Assert.Contains( "bg-brand", component.Find( ".progress-bar" ).ClassList );
        Assert.DoesNotContain( "--bs-progress-bar-bg", component.Find( ".progress-bar" ).GetAttribute( "style" ) );
    }
}