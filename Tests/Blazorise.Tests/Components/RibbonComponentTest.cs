#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.Ribbon;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class RibbonComponentTest : BunitContext
{
    public RibbonComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseButton().AddBlazoriseTextInput();

        var module = JSInterop.SetupModule( new JSTabsModule( JSInterop.JSRuntime, new MockVersionProvider(), new( null, options => { } ) ).ModuleFileName );
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();
        module.SetupVoid( "destroy", _ => true ).SetVoidResult();
    }

    [Fact]
    public async Task ApplicationCommand_ShouldPreserveTabSelectionAndCollapseStateAndSupportFocus()
    {
        var notifications = new List<string>();

        var attributes = new Dictionary<string, object>
        {
            ["aria-label"] = "Open document file commands",
            ["aria-controls"] = "document-backstage",
        };

        var component = Render<Blazorise.Ribbon.Ribbon>( parameters => parameters
            .Add( ribbon => ribbon.SelectedTab, "home" )
            .Add( ribbon => ribbon.Collapsed, true )
            .Add( ribbon => ribbon.SelectedTabChanged, _ => notifications.Add( "selection" ) )
            .Add( ribbon => ribbon.CollapsedChanged, _ => notifications.Add( "collapse" ) )
            .Add( ribbon => ribbon.RibbonApplicationTab, builder =>
            {
                builder.OpenComponent<RibbonApplicationButton>( 0 );
                builder.AddAttribute( 1, nameof( RibbonApplicationButton.ElementId ), "ribbon-file" );
                builder.AddAttribute( 2, nameof( RibbonApplicationButton.Text ), "File" );
                builder.AddAttribute( 3, nameof( RibbonApplicationButton.Clicked ), EventCallback.Factory.Create<MouseEventArgs>( this, async () =>
                {
                    await Task.Yield();
                    notifications.Add( "file" );
                } ) );
                builder.AddAttribute( 4, nameof( RibbonApplicationButton.Attributes ), attributes );
                builder.CloseComponent();
            } )
            .Add( ribbon => ribbon.RibbonTabs, CreateTabs() ) );

        var button = component.Find( "#ribbon-file" );

        Assert.Null( component.Find( "[role=tablist]" ).QuerySelector( "#ribbon-file" ) );
        Assert.False( button.HasAttribute( "aria-selected" ) );
        Assert.False( button.HasAttribute( "aria-pressed" ) );
        Assert.False( button.HasAttribute( "aria-expanded" ) );
        Assert.Equal( "Open document file commands", button.GetAttribute( "aria-label" ) );
        Assert.Equal( "document-backstage", button.GetAttribute( "aria-controls" ) );

        await button.ClickAsync( new MouseEventArgs() );

        Assert.Equal( new[] { "file" }, notifications );
        Assert.Equal( "home", component.Instance.SelectedTab );
        Assert.True( component.Instance.Collapsed );
        Assert.Equal( 2, attributes.Count );

        var applicationButton = component.FindComponent<RibbonApplicationButton>().Instance;

        await component.InvokeAsync( () => applicationButton.Focus( false ) );

        Assert.Equal( "ribbon-file", JSInterop.Invocations["focus"][0].Arguments[1] );
    }

    [Fact]
    public void ApplicationButton_ShouldFollowExpansionStateAndOmitItForOrdinaryCommands()
    {
        var component = Render<RibbonApplicationButton>( parameters => parameters.Add( button => button.Text, "File" ) );

        Assert.False( component.Find( "button" ).HasAttribute( "aria-expanded" ) );

        component.Render( parameters => parameters.Add( button => button.Expanded, false ) );
        Assert.Equal( "false", component.Find( "button" ).GetAttribute( "aria-expanded" ) );

        component.Render( parameters => parameters.Add( button => button.Expanded, true ) );
        Assert.Equal( "true", component.Find( "button" ).GetAttribute( "aria-expanded" ) );

        component.Render( parameters => parameters.Add( button => button.Expanded, false ) );
        Assert.Equal( "false", component.Find( "button" ).GetAttribute( "aria-expanded" ) );

        component.Render( parameters => parameters.Add( button => button.Expanded, null ) );
        Assert.False( component.Find( "button" ).HasAttribute( "aria-expanded" ) );
    }

    [Fact]
    public async Task Selection_ShouldAwaitBindingAndAssociateTheSelectedPanel()
    {
        var notifications = new List<string>();

        var component = Render<Blazorise.Ribbon.Ribbon>( parameters => parameters
            .Add( ribbon => ribbon.SelectedTab, "home" )
            .Add( ribbon => ribbon.SelectedTabChanged, async name =>
            {
                await Task.Yield();
                notifications.Add( name );
            } )
            .Add( ribbon => ribbon.RibbonTabs, CreateTabs() ) );

        await component.InvokeAsync( () => component.Instance.SelectTab( "insert" ) );

        Assert.Equal( new[] { "insert" }, notifications );

        var heading = component.Find( "[role=tab][aria-selected=true]" );
        var panel = component.Find( $"#{heading.GetAttribute( "aria-controls" )}" );

        Assert.Equal( "Insert", heading.TextContent.Trim() );
        Assert.Equal( heading.Id, panel.GetAttribute( "aria-labelledby" ) );
        Assert.False( panel.HasAttribute( "hidden" ) );
        Assert.Equal( "0", heading.GetAttribute( "tabindex" ) );
        Assert.Single( component.FindAll( "[role=tab][tabindex='0']" ) );
    }

    [Fact]
    public async Task Selection_ShouldIgnoreDisabledAndHiddenTabs()
    {
        var component = Render<Blazorise.Ribbon.Ribbon>( parameters => parameters
            .Add( ribbon => ribbon.SelectedTab, "home" )
            .Add( ribbon => ribbon.RibbonTabs, CreateTabs( insertDisabled: true ) ) );

        await component.InvokeAsync( () => component.Instance.SelectTab( "insert" ) );
        Assert.Equal( "home", component.Instance.SelectedTab );

        component.Render( parameters => parameters.Add( ribbon => ribbon.RibbonTabs, CreateTabs( insertVisible: false ) ) );

        await component.InvokeAsync( () => component.Instance.SelectTab( "insert" ) );

        Assert.Equal( "home", component.Instance.SelectedTab );
        Assert.Single( component.FindAll( "[role=tab]" ) );
    }

    [Fact]
    public void HidingTheSelectedContextualTab_ShouldSelectTheFirstAvailableTab()
    {
        var notifications = new List<string>();

        var component = Render<Blazorise.Ribbon.Ribbon>( parameters => parameters
            .Add( ribbon => ribbon.SelectedTab, "insert" )
            .Add( ribbon => ribbon.SelectedTabChanged, name => notifications.Add( name ) )
            .Add( ribbon => ribbon.RibbonTabs, CreateTabs() ) );

        component.Render( parameters => parameters.Add( ribbon => ribbon.RibbonTabs, CreateTabs( insertVisible: false ) ) );

        component.WaitForAssertion( () =>
        {
            Assert.Equal( "home", component.Instance.SelectedTab );
            Assert.Equal( new[] { "home" }, notifications );
        } );
    }

    [Fact]
    public void RemovingTheSelectedTab_ShouldNotifySelectionOnce()
    {
        var notifications = new List<string>();

        var component = Render<Blazorise.Ribbon.Ribbon>( parameters => parameters
            .Add( ribbon => ribbon.SelectedTab, "insert" )
            .Add( ribbon => ribbon.SelectedTabChanged, name => notifications.Add( name ) )
            .Add( ribbon => ribbon.RibbonTabs, CreateTabs() ) );

        component.Render( parameters => parameters.Add( ribbon => ribbon.RibbonTabs, (RenderFragment)( builder =>
        {
            builder.OpenComponent<RibbonTab>( 0 );
            builder.AddAttribute( 1, nameof( RibbonTab.Name ), "home" );
            builder.AddAttribute( 2, nameof( RibbonTab.Text ), "Home" );
            builder.CloseComponent();
        } ) ) );

        component.WaitForAssertion( () =>
        {
            Assert.Equal( "home", component.Instance.SelectedTab );
            Assert.Equal( new[] { "home" }, notifications );
            Assert.Single( component.FindAll( "[role=tab]" ) );
        } );
    }

    [Fact]
    public void CustomHeadingFragments_ShouldRefreshWithoutRerenderingTheirTabDeclarations()
    {
        var declarationRenders = 0;
        var heading = "First heading";

        RenderFragment content = builder =>
        {
            declarationRenders++;
            var currentHeading = heading;

            builder.OpenComponent<RibbonTab>( 0 );
            builder.AddAttribute( 1, nameof( RibbonTab.Name ), "home" );
            builder.AddAttribute( 2, nameof( RibbonTab.HeaderContent ), (RenderFragment)( header => header.AddContent( 0, currentHeading ) ) );
            builder.CloseComponent();
        };

        var component = Render<Blazorise.Ribbon.Ribbon>( parameters => parameters
            .Add( ribbon => ribbon.SelectedTab, "home" )
            .Add( ribbon => ribbon.RibbonTabs, content ) );

        heading = "Updated heading";
        var previousRenders = declarationRenders;
        component.Render();

        component.WaitForAssertion( () =>
        {
            Assert.Equal( "Updated heading", component.Find( "[role=tab]" ).TextContent.Trim() );
            Assert.Equal( previousRenders + 1, declarationRenders );
        } );
    }

    [Fact]
    public async Task TabActivation_ShouldExpandACollapsedRibbonWithoutRecreatingItsControls()
    {
        var notifications = new List<bool>();

        var component = Render<Blazorise.Ribbon.Ribbon>( parameters => parameters
            .Add( ribbon => ribbon.SelectedTab, "home" )
            .Add( ribbon => ribbon.CollapsedChanged, value => notifications.Add( value ) )
            .Add( ribbon => ribbon.RibbonTabs, CreateTabs() ) );

        var input = component.FindComponent<TextInput>().Instance;

        await component.InvokeAsync( () => component.Instance.SetCollapsed( true ) );
        Assert.True( component.Find( $"#{component.Instance.ElementId}-content" ).HasAttribute( "hidden" ) );

        await component.InvokeAsync( () => component.Instance.SelectTab( "insert" ) );
        await component.InvokeAsync( () => component.Instance.SelectTab( "home" ) );

        Assert.False( component.Instance.Collapsed );
        Assert.Equal( new[] { true, false }, notifications );
        Assert.Same( input, component.FindComponent<TextInput>().Instance );
    }

    [Theory]
    [InlineData( TabsRenderMode.LazyLoad, true )]
    [InlineData( TabsRenderMode.LazyReload, false )]
    public async Task RenderMode_ShouldControlWhetherVisitedPanelInstancesArePreserved( TabsRenderMode renderMode, bool shouldPreserve )
    {
        var component = Render<Blazorise.Ribbon.Ribbon>( parameters => parameters
            .Add( ribbon => ribbon.SelectedTab, "home" )
            .Add( ribbon => ribbon.RenderMode, renderMode )
            .Add( ribbon => ribbon.RibbonTabs, CreateTabs() ) );

        var input = component.FindComponent<TextInput>().Instance;

        await component.InvokeAsync( () => component.Instance.SelectTab( "insert" ) );
        await component.InvokeAsync( () => component.Instance.SelectTab( "home" ) );

        Assert.Equal( shouldPreserve, ReferenceEquals( input, component.FindComponent<TextInput>().Instance ) );
    }

    [Fact]
    public async Task Toggle_ShouldUpdateCheckedStateBeforeInvokingTheCommand()
    {
        var notifications = new List<string>();

        var component = Render<RibbonToggleButton>( parameters => parameters
            .Add( button => button.Text, "Bold" )
            .Add( button => button.CheckedChanged, value => notifications.Add( $"checked:{value}" ) )
            .Add( button => button.Clicked, _ => notifications.Add( "clicked" ) ) );

        await component.Find( "button" ).ClickAsync( new MouseEventArgs() );

        Assert.Equal( new[] { "checked:True", "clicked" }, notifications );
        Assert.True( component.Instance.Checked );
        Assert.Equal( "true", component.Find( "button" ).GetAttribute( "aria-pressed" ) );
    }

    [Fact]
    public async Task Gallery_ShouldShareSelectionAndDisabledStateWithItsItems()
    {
        var notifications = new List<string>();

        var component = Render<RibbonGallery>( parameters => parameters
            .Add( gallery => gallery.SelectedValue, "Normal" )
            .Add( gallery => gallery.SelectedValueChanged, value => notifications.Add( value ) )
            .Add( gallery => gallery.ChildContent, builder =>
            {
                builder.OpenComponent<RibbonGalleryItem>( 0 );
                builder.AddAttribute( 1, nameof( RibbonGalleryItem.Value ), "Normal" );
                builder.AddAttribute( 2, nameof( RibbonGalleryItem.Text ), "Normal" );
                builder.CloseComponent();
                builder.OpenComponent<RibbonGalleryItem>( 3 );
                builder.AddAttribute( 4, nameof( RibbonGalleryItem.Value ), "Heading" );
                builder.AddAttribute( 5, nameof( RibbonGalleryItem.Text ), "Heading" );
                builder.CloseComponent();
            } ) );

        await component.FindAll( "button" )[1].ClickAsync( new MouseEventArgs() );

        Assert.Equal( new[] { "Heading" }, notifications );
        Assert.Equal( "false", component.FindAll( "button" )[0].GetAttribute( "aria-pressed" ) );
        Assert.Equal( "true", component.FindAll( "button" )[1].GetAttribute( "aria-pressed" ) );

        component.Render( parameters => parameters.Add( gallery => gallery.Disabled, true ) );

        Assert.All( component.FindAll( "button" ), button => Assert.True( button.HasAttribute( "disabled" ) ) );

        await component.InvokeAsync( () => component.Instance.SelectValue( "Normal" ) );
        Assert.Equal( "Heading", component.Instance.SelectedValue );
    }

    [Fact]
    public void CallerAttributes_ShouldOverrideCommandDefaultsWithoutChangingTheDictionary()
    {
        var attributes = new Dictionary<string, object>
        {
            ["aria-label"] = "Copy document text",
            ["data-command"] = "copy",
        };

        var component = Render<RibbonButton>( parameters => parameters
            .Add( button => button.Text, "Copy" )
            .Add( button => button.Attributes, attributes ) );

        Assert.Equal( "Copy document text", component.Find( "button" ).GetAttribute( "aria-label" ) );
        Assert.Equal( "copy", component.Find( "button" ).GetAttribute( "data-command" ) );
        Assert.Equal( "Copy", component.Find( "button" ).GetAttribute( "title" ) );

        component.Render( parameters => parameters.Add( button => button.Text, "Cut" ) );

        Assert.Equal( "Copy document text", component.Find( "button" ).GetAttribute( "aria-label" ) );
        Assert.Equal( "Cut", component.Find( "button" ).GetAttribute( "title" ) );
        Assert.Equal( 2, attributes.Count );
    }

    [Fact]
    public void CallerAttributes_ShouldReachTheTabPanelAlongsideItsAccessibilityDefaults()
    {
        var attributes = new Dictionary<string, object>
        {
            ["aria-label"] = "Home commands",
            ["data-panel"] = "home",
        };

        var component = Render<Blazorise.Ribbon.Ribbon>( parameters => parameters
            .Add( ribbon => ribbon.SelectedTab, "home" )
            .Add( ribbon => ribbon.RibbonTabs, CreateTabs( homeAttributes: attributes ) ) );

        var panel = component.Find( "[role=tabpanel][data-panel=home]" );
        var heading = component.Find( "[role=tab][aria-selected=true]" );

        Assert.Equal( "Home commands", panel.GetAttribute( "aria-label" ) );
        Assert.Equal( heading.Id, panel.GetAttribute( "aria-labelledby" ) );
        Assert.Equal( "false", panel.GetAttribute( "aria-hidden" ) );
        Assert.False( panel.HasAttribute( "hidden" ) );
        Assert.Equal( 2, attributes.Count );
    }

    private static RenderFragment CreateTabs( bool insertDisabled = false, bool insertVisible = true, Dictionary<string, object> homeAttributes = null ) => builder =>
    {
        builder.OpenComponent<RibbonTab>( 0 );
        builder.AddAttribute( 1, nameof( RibbonTab.Name ), "home" );
        builder.AddAttribute( 2, nameof( RibbonTab.Text ), "Home" );
        builder.AddAttribute( 3, nameof( RibbonTab.ChildContent ), (RenderFragment)( content =>
        {
            content.OpenComponent<TextInput>( 0 );
            content.AddAttribute( 1, nameof( TextInput.ElementId ), "ribbon-sample-input" );
            content.CloseComponent();
        } ) );
        builder.AddAttribute( 4, nameof( RibbonTab.Attributes ), homeAttributes );
        builder.CloseComponent();
        builder.OpenComponent<RibbonTab>( 5 );
        builder.AddAttribute( 6, nameof( RibbonTab.Name ), "insert" );
        builder.AddAttribute( 7, nameof( RibbonTab.Text ), "Insert" );
        builder.AddAttribute( 8, nameof( RibbonTab.Disabled ), insertDisabled );
        builder.AddAttribute( 9, nameof( RibbonTab.Visible ), insertVisible );
        builder.AddAttribute( 10, nameof( RibbonTab.ChildContent ), (RenderFragment)( content => content.AddContent( 0, "Insert commands" ) ) );
        builder.CloseComponent();
    };
}