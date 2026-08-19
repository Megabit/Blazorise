#region Using directives
using AngleSharp.Dom;
using Blazorise.Bootstrap.Providers;
using Blazorise.Modules;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class TooltipComponentTest : BunitContext
{
    public TooltipComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseUtilities();
        JSInterop.AddBlazoriseDocumentObserver();
    }

    [Fact]
    public void Render_ShouldUseProviderTooltipClasses()
    {
        IRenderedComponent<Tooltip> component = Render<Tooltip>( parameters => parameters
            .Add( x => x.Text, "Tooltip text" )
            .Add( x => x.Fade, true )
            .Add( x => x.FadeDuration, 180 )
            .Add( x => x.ShowDelay, 75 )
            .Add( x => x.HideDelay, 100 )
            .Add( x => x.ZIndex, 2000 )
            .AddChildContent( "Target" ) );

        IElement host = component.Find( ".tooltip-host" );
        IElement content = component.Find( ".tooltip" );
        string style = host.GetAttribute( "style" );

        Assert.Equal( content.Id, host.GetAttribute( "aria-describedby" ) );
        Assert.Equal( "tooltip", content.GetAttribute( "role" ) );
        Assert.Equal( "top", content.GetAttribute( "data-tooltip-placement" ) );
        Assert.Equal( "auto", host.GetAttribute( "data-tooltip-animation" ) );
        Assert.Equal( "Tooltip text", component.Find( ".tooltip-inner" ).TextContent );
        Assert.NotNull( component.Find( ".arrow" ) );
        Assert.Contains( "--tooltip-anchor: --tooltip-", style );
        Assert.Contains( "--tooltip-show-delay: 75ms", style );
        Assert.Contains( "--tooltip-hide-delay: 100ms", style );
        Assert.Contains( "--tooltip-fade-duration: 180ms", style );
        Assert.Contains( "--tooltip-z-index: 2000", style );
        Assert.DoesNotContain( "--blazorise-", style );
        this.JSInterop.VerifyNotInvoke( "initialize" );
    }

    [Theory]
    [InlineData( TooltipAnimation.None, "none" )]
    [InlineData( TooltipAnimation.Auto, "auto" )]
    [InlineData( TooltipAnimation.Fade, "fade" )]
    [InlineData( TooltipAnimation.Scale, "scale" )]
    [InlineData( TooltipAnimation.Shift, "shift" )]
    [InlineData( TooltipAnimation.Blur, "blur" )]
    [InlineData( TooltipAnimation.FadeScale, "fade scale" )]
    [InlineData( TooltipAnimation.FadeShift, "fade shift" )]
    [InlineData( TooltipAnimation.FadeBlur, "fade blur" )]
    [InlineData( TooltipAnimation.Fade | TooltipAnimation.Scale | TooltipAnimation.Shift | TooltipAnimation.Blur, "fade scale shift blur" )]
    [InlineData( TooltipAnimation.Auto | TooltipAnimation.FadeBlur, "auto" )]
    public void Animation_ShouldUseExpectedAnimationTokens( TooltipAnimation animation, string expected )
    {
        IRenderedComponent<Tooltip> component = Render<Tooltip>( parameters => parameters
            .Add( x => x.Text, "Tooltip text" )
            .Add( x => x.Animation, animation )
            .AddChildContent( "Target" ) );

        IElement host = component.Find( ".tooltip-host" );

        Assert.Equal( expected, host.GetAttribute( "data-tooltip-animation" ) );
        Assert.Contains( $"--tooltip-fade-duration: {( animation == TooltipAnimation.None ? 0 : 300 )}ms", host.GetAttribute( "style" ) );
    }

    [Fact]
    public void Animation_WhenExplicitlyDefined_ShouldTakePrecedenceOverFade()
    {
        IRenderedComponent<Tooltip> component = Render<Tooltip>( parameters => parameters
            .Add( x => x.Text, "Tooltip text" )
            .Add( x => x.Fade, true )
            .Add( x => x.Animation, TooltipAnimation.None )
            .AddChildContent( "Target" ) );

        IElement host = component.Find( ".tooltip-host" );

        Assert.Equal( "none", host.GetAttribute( "data-tooltip-animation" ) );
        Assert.Contains( "--tooltip-fade-duration: 0ms", host.GetAttribute( "style" ) );
    }

    [Fact]
    public void TooltipContent_ShouldTakePrecedenceOverText()
    {
        RenderFragment tooltipContent = builder =>
        {
            builder.OpenElement( 0, "strong" );
            builder.AddContent( 1, "Rich tooltip content" );
            builder.CloseElement();
        };
        IRenderedComponent<Tooltip> component = Render<Tooltip>( parameters => parameters
            .Add( x => x.Text, "Fallback text" )
            .Add( x => x.TooltipContent, tooltipContent )
            .AddChildContent( "Target" ) );

        IElement host = component.Find( ".tooltip-host" );
        IElement content = component.Find( ".tooltip" );

        Assert.Equal( content.Id, host.GetAttribute( "aria-describedby" ) );
        Assert.Equal( "Rich tooltip content", component.Find( ".tooltip-inner strong" ).TextContent );
        Assert.DoesNotContain( "Fallback text", component.Find( ".tooltip-inner" ).TextContent );
    }

    [Fact]
    public void TriggerTargetId_ShouldSubscribeToExternalTargetEvents()
    {
        IRenderedComponent<Tooltip> component = Render<Tooltip>( parameters => parameters
            .Add( x => x.Text, "Tooltip text" )
            .Add( x => x.Trigger, TooltipTrigger.MouseEnterClick )
            .Add( x => x.TriggerTargetId, "external-target" )
            .AddChildContent( "Target" ) );

        IElement host = component.Find( ".tooltip-host" );

        Assert.Equal( "manual", host.GetAttribute( "data-tooltip-trigger" ) );
        Assert.Equal( 3, JSInterop.Invocations["addSubscription"].Count );

        DocumentObserverJsSubscription targetSubscription = Assert.IsType<DocumentObserverJsSubscription>(
            JSInterop.Invocations["addSubscription"][0].Arguments[0] );

        Assert.Equal( new[] { "click", "mouseenter", "mouseleave" }, targetSubscription.EventNames );
        Assert.Equal( "[id=\"external-target\"]", targetSubscription.Selector );

        DocumentObserverJsSubscription outsideClickSubscription = Assert.IsType<DocumentObserverJsSubscription>(
            JSInterop.Invocations["addSubscription"][1].Arguments[0] );

        Assert.Equal( "click", Assert.Single( outsideClickSubscription.EventNames ) );
        Assert.Contains( "[id=\"external-target\"]", outsideClickSubscription.ExcludeSelector );

        DocumentObserverJsSubscription keyDownSubscription = Assert.IsType<DocumentObserverJsSubscription>(
            JSInterop.Invocations["addSubscription"][2].Arguments[0] );

        Assert.Equal( "keydown", Assert.Single( keyDownSubscription.EventNames ) );
        Assert.Equal( new[] { "Escape" }, keyDownSubscription.KeysFilter );
    }

    [Fact]
    public void TooltipTheme_ShouldUseBootstrapProviderVariables()
    {
        BootstrapStyleProvider styleProvider = new();
        ThemeTooltipOptions options = new()
        {
            BackgroundColor = "#123456",
            Color = "#abcdef",
            FontSize = "1rem",
            BorderRadius = ".5rem",
            MaxWidth = "20rem",
            Padding = ".75rem",
            FadeTime = ".2s",
            ZIndex = "1200",
        };

        string style = styleProvider.TooltipTheme( options );

        Assert.Contains( "--tooltip-bg: #123456", style );
        Assert.Contains( "--tooltip-color: #abcdef", style );
        Assert.Contains( "--tooltip-font-size: 1rem", style );
        Assert.Contains( "--tooltip-border-radius: .5rem", style );
        Assert.Contains( "--tooltip-max-width: 20rem", style );
        Assert.Contains( "--tooltip-padding: .75rem", style );
        Assert.Contains( "--tooltip-fade-duration: .2s", style );
        Assert.Contains( "--tooltip-z-index: 1200", style );
        Assert.DoesNotContain( "--bootstrap-", style );
        Assert.DoesNotContain( "--blazorise-", style );
    }

    [Fact]
    public void ThemeTooltipOptions_ShouldApplyToTooltipHost()
    {
        Theme theme = new()
        {
            TooltipOptions = new()
            {
                BackgroundColor = "#123456",
                Color = "#abcdef",
                FontSize = "1rem",
                BorderRadius = ".5rem",
                MaxWidth = "20rem",
                Padding = ".75rem",
                FadeTime = ".2s",
                ZIndex = "1200",
            },
        };
        RenderFragment tooltipContent = builder =>
        {
            builder.OpenComponent<Tooltip>( 0 );
            builder.AddAttribute( 1, nameof( Tooltip.Text ), "Tooltip text" );
            builder.AddAttribute( 2, nameof( Tooltip.Fade ), true );
            builder.AddAttribute( 3, nameof( Tooltip.ChildContent ), (RenderFragment)( childBuilder => childBuilder.AddContent( 0, "Target" ) ) );
            builder.CloseComponent();
        };
        IRenderedComponent<CascadingValue<Theme>> component = Render<CascadingValue<Theme>>( parameters => parameters
            .Add( x => x.Value, theme )
            .Add( x => x.ChildContent, tooltipContent ) );

        string style = component.Find( ".tooltip-host" ).GetAttribute( "style" );

        Assert.Contains( "--tooltip-bg: #123456", style );
        Assert.Contains( "--tooltip-color: #abcdef", style );
        Assert.Contains( "--tooltip-font-size: 1rem", style );
        Assert.Contains( "--tooltip-border-radius: .5rem", style );
        Assert.Contains( "--tooltip-max-width: 20rem", style );
        Assert.Contains( "--tooltip-padding: .75rem", style );
        Assert.Contains( "--tooltip-fade-duration: .2s", style );
        Assert.Contains( "--tooltip-z-index: 1200", style );
        Assert.DoesNotContain( "--tooltip-fade-duration: 300ms", style );
    }

    [Fact]
    public void ClickTrigger_ShouldToggleActiveState()
    {
        IRenderedComponent<Tooltip> component = Render<Tooltip>( parameters => parameters
            .Add( x => x.Text, "Tooltip text" )
            .Add( x => x.Trigger, TooltipTrigger.Click )
            .AddChildContent( "Target" ) );

        IElement host = component.Find( ".tooltip-host" );

        Assert.Equal( "false", host.GetAttribute( "data-tooltip-active" ) );

        host.Click();

        Assert.Equal( "true", host.GetAttribute( "data-tooltip-active" ) );

        host.Click();

        Assert.Equal( "false", host.GetAttribute( "data-tooltip-active" ) );
    }

    [Fact]
    public void Inline_WhenNotDefined_ShouldUseAutoDetection()
    {
        IRenderedComponent<Tooltip> component = Render<Tooltip>( parameters => parameters
            .Add( x => x.Text, "Tooltip text" )
            .AddChildContent( "Target" ) );

        Assert.Equal( "auto", component.Find( ".tooltip-host" ).GetAttribute( "data-tooltip-inline" ) );
    }

    [Theory]
    [InlineData( true, "true" )]
    [InlineData( false, "false" )]
    public void Inline_WhenExplicitlyDefined_ShouldUseParameterValue( bool inline, string expected )
    {
        IRenderedComponent<Tooltip> component = Render<Tooltip>( parameters => parameters
            .Add( x => x.Text, "Tooltip text" )
            .Add( x => x.Inline, inline )
            .AddChildContent( "Target" ) );

        Assert.Equal( expected, component.Find( ".tooltip-host" ).GetAttribute( "data-tooltip-inline" ) );
    }

    [Fact]
    public void Utilities_ShouldApplyToTooltipSurfaceByDefault()
    {
        IRenderedComponent<Tooltip> component = Render<Tooltip>( parameters => parameters
            .Add( x => x.Text, "Tooltip text" )
            .Add( x => x.Background, Background.Success )
            .Add( x => x.TextColor, TextColor.Danger )
            .AddChildContent( "Target" ) );

        IElement host = component.Find( ".tooltip-host" );
        IElement surface = component.Find( ".tooltip-inner" );

        Assert.Equal( UtilityTarget.Self, component.Instance.UtilityTarget );
        Assert.DoesNotContain( "bg-success", host.ClassList );
        Assert.DoesNotContain( "text-danger", host.ClassList );
        Assert.Contains( "bg-success", surface.ClassList );
        Assert.Contains( "text-danger", surface.ClassList );
    }

    [Fact]
    public void CssValueUtilities_ShouldApplyToTooltipSurfaceByDefault()
    {
        IRenderedComponent<Tooltip> component = Render<Tooltip>( parameters => parameters
            .Add( x => x.Text, "Tooltip text" )
            .Add( x => x.Background, (Background)"#123456" )
            .Add( x => x.TextColor, (TextColor)"#abcdef" )
            .AddChildContent( "Target" ) );

        IElement host = component.Find( ".tooltip-host" );
        IElement surface = component.Find( ".tooltip-inner" );
        string surfaceStyle = surface.GetAttribute( "style" );

        Assert.DoesNotContain( "background-color", host.GetAttribute( "style" ) );
        Assert.DoesNotContain( "color:", host.GetAttribute( "style" ) );
        Assert.Contains( "background-color:#123456 !important", surfaceStyle );
        Assert.Contains( "color:#abcdef !important", surfaceStyle );
    }
}