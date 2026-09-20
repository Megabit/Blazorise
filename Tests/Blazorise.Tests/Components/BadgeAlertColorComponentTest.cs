using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class BadgeAlertColorComponentTest : BunitContext
{
    public BadgeAlertColorComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop.AddBlazoriseUtilities();
    }

    [Theory]
    [InlineData( "#DBB5E6" )]
    [InlineData( "rgb(15,118,110)" )]
    [InlineData( "var(--accent,#34D399)" )]
    public void Badge_ChangingColorAndSubtleReplacesPreviousClassesAndStyles( string cssColor )
    {
        IRenderedComponent<Badge> component = Render<Badge>( parameters => parameters
            .Add( parameter => parameter.Color, Color.Success ) );

        Assert.Contains( "badge-success", component.Find( ".badge" ).ClassList );

        Color customColor = cssColor;
        component.Render( parameters => parameters.Add( parameter => parameter.Color, customColor ) );

        Assert.Contains( "badge-custom", component.Find( ".badge" ).ClassList );
        Assert.DoesNotContain( "badge-success", component.Find( ".badge" ).ClassList );
        Assert.Contains( $"--bs-badge-bg: {cssColor}", component.Find( ".badge" ).GetAttribute( "style" ) );
        Assert.DoesNotContain( cssColor, component.Find( ".badge" ).ClassName );

        component.Render( parameters => parameters
            .Add( parameter => parameter.Subtle, true )
            .Add( parameter => parameter.Link, "/badge-details" ) );

        Assert.Contains( "badge-custom-subtle", component.Find( "a.badge" ).ClassList );
        Assert.DoesNotContain( "badge-custom", component.Find( "a.badge" ).ClassList );
        Assert.Contains( $"--bs-badge-bg: {cssColor}", component.Find( "a.badge" ).GetAttribute( "style" ) );

        component.Render( parameters => parameters.Add( parameter => parameter.Color, new Color( "#312E81" ) ) );

        Assert.Contains( "--bs-badge-bg: #312E81", component.Find( ".badge" ).GetAttribute( "style" ) );
        Assert.DoesNotContain( cssColor, component.Find( ".badge" ).GetAttribute( "style" ) );

        component.Render( parameters => parameters.Add( parameter => parameter.Color, Color.Danger ) );

        Assert.Contains( "badge-danger-subtle", component.Find( ".badge" ).ClassList );
        Assert.DoesNotContain( "badge-custom-subtle", component.Find( ".badge" ).ClassList );
        Assert.DoesNotContain( "--bs-badge-bg", component.Find( ".badge" ).GetAttribute( "style" ) ?? string.Empty );
    }

    [Fact]
    public async Task Badge_CloseAndExplicitTextColorWorkWithCustomColors()
    {
        int closeCount = 0;
        IRenderedComponent<Badge> component = Render<Badge>( parameters => parameters
            .Add( parameter => parameter.Color, new Color( "#FDE68A" ) )
            .Add( parameter => parameter.TextColor, new TextColor( "#123456" ) )
            .Add( parameter => parameter.CloseClicked, () => closeCount++ )
            .AddChildContent( "Closable" ) );

        Assert.Contains( "color:#123456 !important", component.Find( ".badge" ).GetAttribute( "style" ) );
        Assert.DoesNotContain( "#FDE68A", component.Find( ".badge-close" ).ClassName );
        Assert.DoesNotContain( "badge-custom", component.Find( ".badge-close" ).ClassList );

        await component.Find( ".badge-close" ).ClickAsync();

        Assert.Equal( 1, closeCount );

        component.Render( parameters => parameters
            .Add( parameter => parameter.Subtle, true )
            .Add( parameter => parameter.TextColor, TextColor.Default ) );

        Assert.Contains( "badge-custom-subtle", component.Find( ".badge" ).ClassList );
        Assert.DoesNotContain( "color:#123456", component.Find( ".badge" ).GetAttribute( "style" ) );
        Assert.Contains( "--bs-badge-bg: #FDE68A", component.Find( ".badge" ).GetAttribute( "style" ) );

        await component.Find( ".badge-close" ).ClickAsync();

        Assert.Equal( 2, closeCount );
    }

    [Fact]
    public async Task Alert_ChangingColorPreservesContentAndDismissal()
    {
        bool? lastVisible = null;
        IRenderedComponent<Alert> component = Render<Alert>( parameters => parameters
            .Add( parameter => parameter.Color, Color.Success )
            .Add( parameter => parameter.Visible, true )
            .Add( parameter => parameter.Dismisable, true )
            .Add( parameter => parameter.VisibleChanged, ( bool value ) => { lastVisible = value; } )
            .AddChildContent<AlertMessage>( message => message.AddChildContent( "Export ready." ) )
            .AddChildContent<AlertDescription>( description => description.AddChildContent( "Download the report." ) )
            .AddChildContent<CloseButton>( close => close.Add( parameter => parameter.AutoClose, true ) ) );

        component.Render( parameters => parameters
            .Add( parameter => parameter.Color, new Color( "#0F766E" ) )
            .Add( parameter => parameter.TextColor, new TextColor( "#123456" ) ) );

        Assert.Contains( "alert-custom", component.Find( "[role=alert]" ).ClassList );
        Assert.DoesNotContain( "alert-success", component.Find( "[role=alert]" ).ClassList );
        Assert.Contains( "--bs-alert-bg: #0F766E", component.Find( "[role=alert]" ).GetAttribute( "style" ) );
        Assert.Contains( "color:#123456 !important", component.Find( "[role=alert]" ).GetAttribute( "style" ) );
        Assert.Contains( "Export ready.", component.Markup );
        Assert.Contains( "Download the report.", component.Markup );

        component.Render( parameters => parameters
            .Add( parameter => parameter.Color, new Color( "var(--accent,#7C3AED)" ) )
            .Add( parameter => parameter.TextColor, TextColor.Default ) );

        Assert.Contains( "--bs-alert-bg: var(--accent,#7C3AED)", component.Find( "[role=alert]" ).GetAttribute( "style" ) );
        Assert.DoesNotContain( "#0F766E", component.Markup );
        Assert.DoesNotContain( "color:#123456", component.Find( "[role=alert]" ).GetAttribute( "style" ) );

        await component.Find( "button" ).ClickAsync();

        Assert.False( component.Instance.Visible );
        Assert.Equal( false, lastVisible );

        component.Render( parameters => parameters
            .Add( parameter => parameter.Color, Color.Danger )
            .Add( parameter => parameter.Visible, true ) );

        Assert.Contains( "alert-danger", component.Find( "[role=alert]" ).ClassList );
        Assert.DoesNotContain( "alert-custom", component.Find( "[role=alert]" ).ClassList );
        Assert.DoesNotContain( "--bs-alert-bg", component.Find( "[role=alert]" ).GetAttribute( "style" ) ?? string.Empty );
    }

    [Fact]
    public async Task CustomColors_PersistWhenTheThemeChangesOrIsDisabled()
    {
        Theme theme = new() { ColorOptions = new() { Primary = "#7c3aed" } };
        IRenderedComponent<ThemeProvider> component = Render<ThemeProvider>( parameters => parameters
            .Add( parameter => parameter.Theme, theme )
            .AddChildContent<Badge>( badge => badge
                .Add( parameter => parameter.Color, new Color( "#DBB5E6" ) )
                .Add( parameter => parameter.Subtle, true ) )
            .AddChildContent<Alert>( alert => alert
                .Add( parameter => parameter.Color, new Color( "#0F766E" ) )
                .Add( parameter => parameter.Visible, true ) ) );
        string badgeStyles = component.Find( ".badge" ).GetAttribute( "style" );
        string alertStyles = component.Find( "[role=alert]" ).GetAttribute( "style" );

        Assert.Contains( "--bs-badge-bg: #DBB5E6", badgeStyles );
        Assert.Contains( "--bs-alert-bg: #0F766E", alertStyles );
        Assert.NotEmpty( component.Find( "#b-theme-styles" ).TextContent );

        await component.InvokeAsync( () =>
        {
            theme.ColorOptions.Primary = "#ff0000";
            theme.ThemeHasChanged();
        } );

        component.WaitForAssertion( () =>
        {
            Assert.Equal( badgeStyles, component.Find( ".badge" ).GetAttribute( "style" ) );
            Assert.Equal( alertStyles, component.Find( "[role=alert]" ).GetAttribute( "style" ) );
        } );

        await component.InvokeAsync( () =>
        {
            theme.Enabled = false;
            theme.ThemeHasChanged();
        } );

        component.WaitForAssertion( () =>
        {
            Assert.Empty( component.FindAll( "#b-theme-styles" ) );
            Assert.Contains( "badge-custom-subtle", component.Find( ".badge" ).ClassList );
            Assert.Contains( "alert-custom", component.Find( "[role=alert]" ).ClassList );
            Assert.Equal( badgeStyles, component.Find( ".badge" ).GetAttribute( "style" ) );
            Assert.Equal( alertStyles, component.Find( "[role=alert]" ).GetAttribute( "style" ) );
        } );
    }
}