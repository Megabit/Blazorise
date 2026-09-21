using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class CustomColorComponentTest : BunitContext
{
    public CustomColorComponentTest()
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
    public async Task BadgeAndAlert_PreserveCustomColorsWhenTheThemeChangesOrIsDisabled()
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

    [Theory]
    [InlineData( "#DBB5E6" )]
    [InlineData( "#123" )]
    [InlineData( "rgb(219 181 230)" )]
    [InlineData( "hsl(0 0% 100%)" )]
    [InlineData( "var(--accent,#DBB5E6)" )]
    [InlineData( "transparent" )]
    [InlineData( "#DBB5E680" )]
    [InlineData( "rgba(219,181,230,0.5)" )]
    public void Progress_CustomColorsDelegateAppearanceToProviderStylesWithTheThemeEnabled( string color )
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
    public async Task ProgressBar_PreservesCustomColorWhenThemeChangesOrIsDisabled()
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
    public void Progress_ExplicitTextColorTakesPrecedenceAndCanBeRemoved( string color )
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
    public void ProgressBar_ExplicitCssTextColorOverridesProviderStyles()
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

    [Fact]
    public void ExplicitCssColors_RenderAsInlineUtilityStyles()
    {
        IRenderedComponent<Div> component = Render<Div>( parameters => parameters
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
        IRenderedComponent<Div> component = Render<Div>( parameters => parameters
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

    [Fact]
    public async Task ListItems_PreserveCustomColorsWhenSelectionChanges()
    {
        IRenderedComponent<ListGroup> component = Render<ListGroup>( parameters => parameters
            .Add( parameter => parameter.Mode, ListGroupMode.Selectable )
            .Add( parameter => parameter.SelectedItem, "first" )
            .AddChildContent<ListGroupItem>( item => item
                .Add( parameter => parameter.Name, "first" )
                .Add( parameter => parameter.Color, new Color( "#DBB5E6" ) )
                .AddChildContent( "First" ) )
            .AddChildContent<ListGroupItem>( item => item
                .Add( parameter => parameter.Name, "second" )
                .Add( parameter => parameter.Color, new Color( "#312E81" ) )
                .AddChildContent( "Second" ) ) );

        await component.FindAll( ".list-group-item" )[1].ClickAsync();

        Assert.Contains( "active", component.FindAll( ".list-group-item" )[1].ClassList );
        Assert.Contains( "--bs-list-group-bg: #DBB5E6", component.FindAll( ".list-group-item" )[0].GetAttribute( "style" ) );
        Assert.Contains( "--bs-list-group-bg: #312E81", component.FindAll( ".list-group-item" )[1].GetAttribute( "style" ) );

        component.FindComponents<ListGroupItem>()[1].Render( parameters => parameters
            .Add( parameter => parameter.Color, Color.Success ) );

        Assert.DoesNotContain( "--bs-list-group-bg", component.FindAll( ".list-group-item" )[1].GetAttribute( "style" ) ?? string.Empty );
        Assert.Contains( "list-group-item-success", component.FindAll( ".list-group-item" )[1].ClassList );
    }

    [Fact]
    public void TableCells_KeepTheirOwnColorWhenRowColorChanges()
    {
        IRenderedComponent<Table> component = Render<Table>( parameters => parameters
            .Add( parameter => parameter.Hoverable, true )
            .Add( parameter => parameter.Striped, true )
            .AddChildContent<TableBody>( body => body
                .AddChildContent<TableRow>( row => row
                    .Add( parameter => parameter.Color, new Color( "#DBB5E6" ) )
                    .AddChildContent<TableRowCell>( cell => cell.AddChildContent( "Inherited" ) )
                    .AddChildContent<TableRowCell>( cell => cell
                        .Add( parameter => parameter.Color, new Color( "#312E81" ) )
                        .AddChildContent( "Custom" ) )
                    .AddChildContent<TableRowCell>( cell => cell
                        .Add( parameter => parameter.Color, Color.Success )
                        .AddChildContent( "Contextual" ) ) ) ) );

        component.FindComponent<TableRow>().Render( parameters => parameters
            .Add( parameter => parameter.Color, new Color( "rgb(15,118,110)" ) ) );

        Assert.Contains( "--bs-table-bg: rgb(15,118,110)", component.Find( "tbody tr" ).GetAttribute( "style" ) );
        Assert.Contains( "--bs-table-bg: #312E81", component.FindAll( "td" )[1].GetAttribute( "style" ) );
        Assert.Contains( "table-success", component.FindAll( "td" )[2].ClassList );
        Assert.DoesNotContain( "--bs-table-bg", component.FindAll( "td" )[2].GetAttribute( "style" ) ?? string.Empty );

        component.FindComponent<TableRow>().Render( parameters => parameters
            .Add( parameter => parameter.Color, Color.Default ) );

        Assert.DoesNotContain( "table-custom", component.Find( "tbody tr" ).ClassList );
        Assert.DoesNotContain( "--bs-table-bg", component.Find( "tbody tr" ).GetAttribute( "style" ) ?? string.Empty );
        Assert.Contains( "--bs-table-bg: #312E81", component.FindAll( "td" )[1].GetAttribute( "style" ) );
    }

    [Fact]
    public async Task Switch_UpdatesTheTrackColorWithoutChangingItsValue()
    {
        IRenderedComponent<Switch<bool>> component = Render<Switch<bool>>( parameters => parameters
            .Add( parameter => parameter.Color, new Color( "#DBB5E6" ) )
            .Add( parameter => parameter.Value, true )
            .AddChildContent( "Notifications" ) );

        component.Render( parameters => parameters
            .Add( parameter => parameter.Color, new Color( "var(--accent,#34D399)" ) ) );

        Assert.True( component.Find( "input" ).HasAttribute( "checked" ) );
        Assert.Contains( "--bs-switch-bg: var(--accent,#34D399)", component.Find( "label" ).GetAttribute( "style" ) );

        await component.Find( "input" ).ChangeAsync( false );

        Assert.False( component.Instance.Value );
        Assert.Contains( "--bs-switch-bg: var(--accent,#34D399)", component.Find( "label" ).GetAttribute( "style" ) );

        component.Render( parameters => parameters.Add( parameter => parameter.Color, Color.Primary ) );

        Assert.DoesNotContain( "--bs-switch-bg", component.Find( "label" ).GetAttribute( "style" ) ?? string.Empty );
        Assert.Contains( "custom-control-input-primary", component.Find( "input" ).ClassList );
    }

    [Fact]
    public void RadioButtons_UpdateInheritedColorsAndPreserveOverrides()
    {
        IRenderedComponent<RadioGroup<string>> component = Render<RadioGroup<string>>( parameters => parameters
            .Add( parameter => parameter.Buttons, true )
            .Add( parameter => parameter.Value, "first" )
            .Add( parameter => parameter.Color, new Color( "#DBB5E6" ) )
            .AddChildContent<Radio<string>>( radio => radio
                .Add( parameter => parameter.Value, "first" )
                .AddChildContent( "Inherited" ) )
            .AddChildContent<Radio<string>>( radio => radio
                .Add( parameter => parameter.Value, "second" )
                .Add( parameter => parameter.Color, new Color( "#312E81" ) )
                .AddChildContent( "Override" ) ) );

        component.Render( parameters => parameters
            .Add( parameter => parameter.Color, new Color( "#0F766E" ) ) );

        Assert.Contains( "#0F766E", component.FindAll( "label.btn" )[0].GetAttribute( "style" ) );
        Assert.Contains( "#312E81", component.FindAll( "label.btn" )[1].GetAttribute( "style" ) );

        component.Render( parameters => parameters
            .Add( parameter => parameter.Color, Color.Success )
            .Add( parameter => parameter.Disabled, true ) );

        Assert.Contains( "btn-success", component.FindAll( "label.btn" )[0].ClassList );
        Assert.Contains( "btn-custom", component.FindAll( "label.btn" )[1].ClassList );
        Assert.DoesNotContain( "#0F766E", component.Markup );
        Assert.All( component.FindAll( "input" ), input => Assert.True( input.HasAttribute( "disabled" ) ) );
    }

    [Fact]
    public async Task Rating_UpdatesEveryIconColorAndStillAcceptsSelection()
    {
        IRenderedComponent<Rating> component = Render<Rating>( parameters => parameters
            .Add( parameter => parameter.Color, new Color( "#7C3AED" ) )
            .Add( parameter => parameter.SelectedValue, 2 ) );

        component.Render( parameters => parameters
            .Add( parameter => parameter.Color, new Color( "#0F766E" ) ) );

        Assert.All( component.FindAll( ".rating-item" ), item =>
            Assert.Contains( "--bs-rating-color: #0F766E", item.GetAttribute( "style" ) ) );

        await component.FindAll( ".rating-item" )[3].ClickAsync();

        Assert.Equal( 4, component.Instance.SelectedValue );

        component.Render( parameters => parameters.Add( parameter => parameter.Color, Color.Warning ) );

        Assert.All( component.FindAll( ".rating-item" ), item =>
        {
            Assert.Contains( "rating-item-warning", item.ClassList );
            Assert.DoesNotContain( "--bs-rating-color", item.GetAttribute( "style" ) ?? string.Empty );
        } );
    }

    [Fact]
    public void Step_PreservesCustomColorAcrossCompletedStateChanges()
    {
        IRenderedComponent<Step> component = Render<Step>( parameters => parameters
            .Add( parameter => parameter.Name, "review" )
            .Add( parameter => parameter.Index, 2 )
            .Add( parameter => parameter.Color, new Color( "#DBB5E6" ) )
            .AddChildContent( "Review" ) );

        component.Render( parameters => parameters.Add( parameter => parameter.Completed, true ) );

        Assert.Contains( "step-completed", component.Find( "li" ).ClassList );
        Assert.Contains( "step-custom", component.Find( "li" ).ClassList );
        Assert.Contains( "--bs-step-color: #DBB5E6", component.Find( "li" ).GetAttribute( "style" ) );

        component.Render( parameters => parameters.Add( parameter => parameter.Color, Color.Success ) );

        Assert.Contains( "step-success", component.Find( "li" ).ClassList );
        Assert.DoesNotContain( "--bs-step-color", component.Find( "li" ).GetAttribute( "style" ) ?? string.Empty );
    }

    [Fact]
    public async Task PageProgress_PreservesCustomColorAcrossModesAndThemeChanges()
    {
        Theme theme = new() { ColorOptions = new() { Primary = "#ff0000" } };
        IRenderedComponent<ThemeProvider> component = Render<ThemeProvider>( parameters => parameters
            .Add( parameter => parameter.Theme, theme )
            .AddChildContent<PageProgress>( progress => progress
                .Add( parameter => parameter.Color, new Color( "#7C3AED" ) )
                .Add( parameter => parameter.Visible, true )
                .Add( parameter => parameter.Value, 65 ) ) );

        Assert.Contains( "--bs-page-progress-bg: #7C3AED", component.Find( ".b-page-progress-indicator" ).GetAttribute( "style" ) );

        component.FindComponent<PageProgress>().Render( parameters => parameters.Add( parameter => parameter.Value, ( int? )null ) );

        Assert.Contains( "b-page-progress-indicator-indeterminate", component.Find( ".b-page-progress-indicator" ).ClassList );
        Assert.DoesNotContain( "width:", component.Find( ".b-page-progress-indicator" ).GetAttribute( "style" ) );

        await component.InvokeAsync( () =>
        {
            theme.ColorOptions.Primary = "#00ff00";
            theme.ThemeHasChanged();
        } );

        component.WaitForAssertion( () => Assert.Contains( "--bs-page-progress-bg: #7C3AED",
            component.Find( ".b-page-progress-indicator" ).GetAttribute( "style" ) ) );

        await component.InvokeAsync( () =>
        {
            theme.Enabled = false;
            theme.ThemeHasChanged();
        } );

        component.WaitForAssertion( () => Assert.Empty( component.FindAll( "#b-theme-styles" ) ) );
        Assert.Contains( "--bs-page-progress-bg: #7C3AED", component.Find( ".b-page-progress-indicator" ).GetAttribute( "style" ) );

        component.FindComponent<PageProgress>().Render( parameters => parameters.Add( parameter => parameter.Color, Color.Success ) );

        Assert.Contains( "b-page-progress-indicator-success", component.Find( ".b-page-progress-indicator" ).ClassList );
        Assert.DoesNotContain( "--bs-page-progress-bg", component.Find( ".b-page-progress-indicator" ).GetAttribute( "style" ) ?? string.Empty );
    }
}