using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class CssColorUtilityComponentTest : BunitContext
{
    public CssColorUtilityComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
    }

    [Fact]
    public void ExplicitCssColors_RenderAsInlineUtilityStyles()
    {
        var component = Render<Div>( parameters => parameters
            .Add( parameter => parameter.TextColor, (TextColor)CssColor.Rgba( 255, 255, 255, 0.75 ) )
            .Add( parameter => parameter.Background, (Background)CssColor.Variable( "surface", "#000" ) )
            .Add( parameter => parameter.Border, Border.WithColor( CssColor.Hsl( 228, 88, 60 ) ) ) );

        string style = component.Find( "div" ).GetAttribute( "style" );

        Assert.Contains( "color:rgba(255,255,255,0.75) !important", style );
        Assert.Contains( "background-color:var(--surface,#000) !important", style );
        Assert.Contains( "border-color:hsl(228 88% 60%) !important", style );
        Assert.DoesNotContain( "text-rgba", component.Markup );
        Assert.DoesNotContain( "bg-var", component.Markup );
        Assert.DoesNotContain( "border-hsl", component.Markup );

        component.Render( parameters => parameters
            .Add( parameter => parameter.TextColor, (TextColor)CssColor.Rgb( 12, 34, 56 ) ) );

        Assert.Contains( "color:rgb(12,34,56) !important", component.Find( "div" ).GetAttribute( "style" ) );

        component.Render( parameters => parameters
            .Add( parameter => parameter.TextColor, TextColor.Primary ) );

        Assert.Contains( "text-primary", component.Find( "div" ).ClassName );
        Assert.DoesNotContain( "color:rgb(12,34,56)", component.Find( "div" ).GetAttribute( "style" ) );
    }

    [Fact]
    public void ContextualColors_KeepUsingProviderClasses()
    {
        var component = Render<Div>( parameters => parameters
            .Add( parameter => parameter.TextColor, TextColor.Primary )
            .Add( parameter => parameter.Background, Background.Danger )
            .Add( parameter => parameter.Border, Border.Success ) );

        string classNames = component.Find( "div" ).ClassName;

        Assert.Contains( "text-primary", classNames );
        Assert.Contains( "bg-danger", classNames );
        Assert.Contains( "border-success", classNames );
    }

    [Fact]
    public void CssColorFactories_UseInvariantClampedValues()
    {
        Assert.Equal( "rgb(12,34,56)", CssColor.Rgb( 12, 34, 56 ) );
        Assert.Equal( "rgba(12,34,56,1)", CssColor.Rgba( 12, 34, 56, 2 ) );
        Assert.Equal( "hsl(228 100% 0%)", CssColor.Hsl( 228, 120, -10 ) );
        Assert.Equal( "var(--chart-color,#fff)", CssColor.Variable( "chart-color", "#fff" ) );
        Assert.False( CssColor.IsValue( "rgb(0,0,0);display:none" ) );
    }

    [Fact]
    public void ColorTypes_CacheCssValueClassification()
    {
        Assert.True( ( (Color)CssColor.Rgb( 12, 34, 56 ) ).IsCssValue );
        Assert.True( ( (TextColor)CssColor.Rgb( 12, 34, 56 ) ).OnWrapper.IsCssValue );
        Assert.True( ( (Background)CssColor.Rgb( 12, 34, 56 ) ).OnSelf.IsCssValue );
        Assert.True( ( (BorderColor)CssColor.Rgb( 12, 34, 56 ) ).IsCssValue );

        Assert.False( Color.Primary.IsCssValue );
        Assert.False( TextColor.Primary.Emphasis.IsCssValue );
        Assert.False( ( (Background)Background.Primary.Subtle ).IsCssValue );
        Assert.False( BorderColor.Primary.IsCssValue );
    }
}