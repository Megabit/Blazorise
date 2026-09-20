using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class ButtonColorComponentTest : BunitContext
{
    public ButtonColorComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop.AddBlazoriseButton().AddBlazoriseClosable().AddBlazoriseDropdown();
    }

    [Theory]
    [InlineData( "#DBB5E6" )]
    [InlineData( "rgb(14,165,233)" )]
    [InlineData( "hsl(240 50% 30%)" )]
    [InlineData( "var(--accent,#DBB5E6)" )]
    public void Button_ChangingColorAndOutlineReplacesPreviousClassesAndStyles( string color )
    {
        IRenderedComponent<Button> component = Render<Button>( parameters => parameters
            .Add( parameter => parameter.Color, Color.Success ) );

        Assert.Contains( "btn-success", component.Find( "button" ).ClassList );

        component.Render( parameters => parameters.Add( parameter => parameter.Color, new Color( color ) ) );

        Assert.DoesNotContain( "btn-success", component.Find( "button" ).ClassList );
        Assert.Contains( "btn-custom", component.Find( "button" ).ClassList );
        Assert.Contains( $"--bs-btn-bg: {color}", component.Find( "button" ).GetAttribute( "style" ) );
        Assert.DoesNotContain( color, component.Find( "button" ).ClassName );

        component.Render( parameters => parameters.Add( parameter => parameter.Outline, true ) );

        Assert.Contains( "btn-outline-custom", component.Find( "button" ).ClassList );
        Assert.DoesNotContain( "btn-custom", component.Find( "button" ).ClassList );
        Assert.Contains( $"--bs-btn-bg: {color}", component.Find( "button" ).GetAttribute( "style" ) );

        component.Render( parameters => parameters.Add( parameter => parameter.Color, new Color( "#123456" ) ) );

        Assert.Contains( "--bs-btn-bg: #123456", component.Find( "button" ).GetAttribute( "style" ) );
        Assert.DoesNotContain( color, component.Find( "button" ).GetAttribute( "style" ) );

        component.Render( parameters => parameters.Add( parameter => parameter.Color, Color.Danger ) );

        Assert.Contains( "btn-outline-danger", component.Find( "button" ).ClassList );
        Assert.DoesNotContain( "btn-outline-custom", component.Find( "button" ).ClassList );
        Assert.DoesNotContain( "--bs-btn-bg", component.Find( "button" ).GetAttribute( "style" ) ?? string.Empty );
    }

    [Fact]
    public void Button_ExplicitTextColorCanBeChangedAndRemoved()
    {
        IRenderedComponent<Button> component = Render<Button>( parameters => parameters
            .Add( parameter => parameter.Color, new Color( "#DBB5E6" ) )
            .Add( parameter => parameter.TextColor, new TextColor( "#123456" ) ) );

        Assert.Contains( "color:#123456 !important", component.Find( "button" ).GetAttribute( "style" ) );

        component.Render( parameters => parameters.Add( parameter => parameter.TextColor, TextColor.Dark ) );

        Assert.Contains( "text-dark", component.Find( "button" ).ClassList );
        Assert.DoesNotContain( "color:#123456", component.Find( "button" ).GetAttribute( "style" ) );

        component.Render( parameters => parameters.Add( parameter => parameter.TextColor, TextColor.Default ) );

        Assert.DoesNotContain( "text-dark", component.Find( "button" ).ClassList );
        Assert.Contains( "--bs-btn-bg: #DBB5E6", component.Find( "button" ).GetAttribute( "style" ) );
        Assert.DoesNotContain( "color:", component.Find( "button" ).GetAttribute( "style" ) );
    }

    [Fact]
    public void DropdownToggle_ChangingColorAndOutlineKeepsTheSplitButtonIndependent()
    {
        IRenderedComponent<Dropdown> component = Render<Dropdown>( parameters => parameters
            .AddChildContent<Button>( button => button
                .Add( parameter => parameter.Color, new Color( "#312E81" ) ) )
            .AddChildContent<DropdownToggle>( toggleParameters => toggleParameters
                .Add( parameter => parameter.Color, Color.Success )
                .Add( parameter => parameter.Split, true ) ) );
        IRenderedComponent<DropdownToggle> toggle = component.FindComponent<DropdownToggle>();

        toggle.Render( parameters => parameters
            .Add( parameter => parameter.Color, new Color( "#DBB5E6" ) )
            .Add( parameter => parameter.Outline, true ) );

        Assert.Contains( "btn-outline-custom", toggle.Find( "button" ).ClassList );
        Assert.DoesNotContain( "btn-success", toggle.Find( "button" ).ClassList );
        Assert.Contains( "--bs-btn-bg: #DBB5E6", toggle.Find( "button" ).GetAttribute( "style" ) );
        Assert.Contains( "--bs-btn-bg: #312E81", component.FindComponent<Button>().Find( "button" ).GetAttribute( "style" ) );

        toggle.Render( parameters => parameters
            .Add( parameter => parameter.Color, new Color( "var(--accent,#DBB5E6)" ) )
            .Add( parameter => parameter.Outline, false )
            .Add( parameter => parameter.TextColor, new TextColor( "#123456" ) ) );

        Assert.Contains( "btn-custom", toggle.Find( "button" ).ClassList );
        Assert.DoesNotContain( "btn-outline-custom", toggle.Find( "button" ).ClassList );
        Assert.Contains( "--bs-btn-bg: var(--accent,#DBB5E6)", toggle.Find( "button" ).GetAttribute( "style" ) );
        Assert.Contains( "color:#123456 !important", toggle.Find( "button" ).GetAttribute( "style" ) );

        toggle.Render( parameters => parameters
            .Add( parameter => parameter.Color, Color.Danger )
            .Add( parameter => parameter.TextColor, TextColor.Default ) );

        Assert.Contains( "btn-danger", toggle.Find( "button" ).ClassList );
        Assert.DoesNotContain( "btn-custom", toggle.Find( "button" ).ClassList );
        Assert.DoesNotContain( "--bs-btn-bg", toggle.Find( "button" ).GetAttribute( "style" ) ?? string.Empty );
        Assert.Contains( "--bs-btn-bg: #312E81", component.FindComponent<Button>().Find( "button" ).GetAttribute( "style" ) );
    }

    [Fact]
    public async Task SplitDropdown_PreservesCustomColorsWhenOpenedAndTheThemeChanges()
    {
        Theme theme = new() { ColorOptions = new() { Primary = "#7c3aed" } };
        IRenderedComponent<ThemeProvider> component = Render<ThemeProvider>( parameters => parameters
            .Add( parameter => parameter.Theme, theme )
            .AddChildContent<Dropdown>( dropdown => dropdown
                .AddChildContent<Button>( buttonParameters => buttonParameters
                    .Add( parameter => parameter.Color, new Color( "#DBB5E6" ) ) )
                .AddChildContent<DropdownToggle>( toggleParameters => toggleParameters
                    .Add( parameter => parameter.Color, new Color( "#312E81" ) )
                    .Add( parameter => parameter.Split, true ) )
                .AddChildContent<DropdownMenu>( menu => menu
                    .AddChildContent<DropdownItem>( item => item.AddChildContent( "Action" ) ) ) ) );
        IRenderedComponent<Button> button = component.FindComponent<Button>();
        IRenderedComponent<DropdownToggle> toggle = component.FindComponent<DropdownToggle>();
        string buttonStyle = button.Find( "button" ).GetAttribute( "style" );
        string toggleStyle = toggle.Find( "button" ).GetAttribute( "style" );

        Assert.Contains( "--bs-btn-bg: #DBB5E6", buttonStyle );
        Assert.Contains( "--bs-btn-bg: #312E81", toggleStyle );
        Assert.NotEmpty( component.Find( "#b-theme-styles" ).TextContent );

        await toggle.Find( "button" ).ClickAsync();

        Assert.Equal( "true", toggle.Find( "button" ).GetAttribute( "aria-expanded" ) );
        Assert.Equal( toggleStyle, toggle.Find( "button" ).GetAttribute( "style" ) );

        await component.InvokeAsync( () =>
        {
            theme.ColorOptions.Primary = "#ff0000";
            theme.ThemeHasChanged();
        } );

        component.WaitForAssertion( () =>
        {
            Assert.Equal( buttonStyle, button.Find( "button" ).GetAttribute( "style" ) );
            Assert.Equal( toggleStyle, toggle.Find( "button" ).GetAttribute( "style" ) );
        } );

        await component.InvokeAsync( () =>
        {
            theme.Enabled = false;
            theme.ThemeHasChanged();
        } );

        component.WaitForAssertion( () =>
        {
            Assert.Empty( component.FindAll( "#b-theme-styles" ) );
            Assert.Contains( "btn-custom", button.Find( "button" ).ClassList );
            Assert.Contains( "btn-custom", toggle.Find( "button" ).ClassList );
            Assert.Equal( buttonStyle, button.Find( "button" ).GetAttribute( "style" ) );
            Assert.Equal( toggleStyle, toggle.Find( "button" ).GetAttribute( "style" ) );
        } );
    }
}