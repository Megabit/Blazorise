#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Ribbon;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class RibbonBackstageComponentTest : BunitContext
{
    public RibbonBackstageComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider();
        JSInterop.AddBlazoriseButton().AddBlazoriseTextInput();
    }

    [Fact]
    public async Task Navigation_ShouldAwaitSelectionBeforeClickAndKeepCommandsOnTheCurrentPage()
    {
        var notifications = new List<string>();

        var component = Render<RibbonBackstage>( parameters => parameters
            .Add( backstage => backstage.ElementId, "backstage" )
            .Add( backstage => backstage.Visible, true )
            .Add( backstage => backstage.SelectedItem, "info" )
            .Add( backstage => backstage.SelectedItemChanged, async name =>
            {
                await Task.Yield();
                notifications.Add( $"selected:{name}" );
            } )
            .Add( backstage => backstage.ChildContent, CreateItems( clicked: async name =>
            {
                await Task.Yield();
                notifications.Add( $"clicked:{name}" );
            } ) ) );

        await component.Find( "#backstage-save" ).ClickAsync( new MouseEventArgs() );

        Assert.Equal( "info", component.Instance.SelectedItem );
        Assert.True( component.Instance.Visible );
        Assert.False( component.Find( "#backstage-save" ).HasAttribute( "aria-current" ) );
        Assert.False( component.Find( "#backstage-save" ).HasAttribute( "aria-controls" ) );

        await component.Find( "#backstage-print" ).ClickAsync( new MouseEventArgs() );

        Assert.Equal( new[] { "clicked:save", "selected:print", "clicked:print" }, notifications );
        Assert.Equal( "print", component.Instance.SelectedItem );
        Assert.Equal( "page", component.Find( "#backstage-print" ).GetAttribute( "aria-current" ) );

        var panel = component.Find( "#backstage-print-content" );

        Assert.Equal( "backstage-print", panel.GetAttribute( "aria-labelledby" ) );
        Assert.False( panel.HasAttribute( "hidden" ) );
        Assert.True( component.Find( "#backstage-info-content" ).HasAttribute( "hidden" ) );
    }

    [Fact]
    public async Task Visibility_ShouldFocusBackAndCloseThroughEscapeAndTheBackButton()
    {
        var notifications = new List<string>();

        var component = Render<RibbonBackstage>( parameters => parameters
            .Add( backstage => backstage.ElementId, "backstage" )
            .Add( backstage => backstage.SelectedItem, "info" )
            .Add( backstage => backstage.VisibleChanged, async visible =>
            {
                await Task.Yield();
                notifications.Add( $"visible:{visible}" );
            } )
            .Add( backstage => backstage.Opened, () => notifications.Add( "opened" ) )
            .Add( backstage => backstage.Closed, () => notifications.Add( "closed" ) )
            .Add( backstage => backstage.ChildContent, CreateItems() ) );

        Assert.True( component.Find( "#backstage" ).HasAttribute( "hidden" ) );
        Assert.Empty( component.FindComponents<TextInput>() );
        Assert.Empty( notifications );

        await component.InvokeAsync( component.Instance.Show );
        await component.InvokeAsync( component.Instance.Show );

        component.WaitForAssertion( () =>
        {
            Assert.Equal( new[] { "visible:True", "opened" }, notifications );
            Assert.Equal( "backstage-back", JSInterop.Invocations["focus"][0].Arguments[1] );
            Assert.False( component.Find( "#backstage" ).HasAttribute( "hidden" ) );
        } );

        await component.Find( "#backstage" ).KeyDownAsync( new KeyboardEventArgs { Key = "Escape" } );

        component.WaitForAssertion( () =>
        {
            Assert.Equal( new[] { "visible:True", "opened", "visible:False", "closed" }, notifications );
            Assert.True( component.Find( "#backstage" ).HasAttribute( "hidden" ) );
        } );

        await component.InvokeAsync( component.Instance.Show );
        await component.Find( "#backstage-back" ).ClickAsync( new MouseEventArgs() );
        await component.InvokeAsync( component.Instance.Hide );

        component.WaitForAssertion( () =>
        {
            Assert.Equal( new[] { "visible:True", "opened", "visible:False", "closed", "visible:True", "opened", "visible:False", "closed" }, notifications );
            Assert.Equal( 2, JSInterop.Invocations["focus"].Count );
        } );
    }

    [Theory]
    [InlineData( TabsRenderMode.LazyLoad, true )]
    [InlineData( TabsRenderMode.LazyReload, false )]
    public async Task PageLifetime_ShouldPreserveHiddenPagesAndHonorTheRenderingPolicy( TabsRenderMode renderMode, bool shouldPreserve )
    {
        var component = Render<RibbonBackstage>( parameters => parameters
            .Add( backstage => backstage.SelectedItem, "info" )
            .Add( backstage => backstage.RenderMode, renderMode )
            .Add( backstage => backstage.ChildContent, CreateItems() ) );

        Assert.Empty( component.FindComponents<TextInput>() );

        await component.InvokeAsync( component.Instance.Show );
        var input = component.FindComponent<TextInput>().Instance;

        await component.InvokeAsync( component.Instance.Hide );
        Assert.Same( input, component.FindComponent<TextInput>().Instance );

        await component.InvokeAsync( component.Instance.Show );
        Assert.Same( input, component.FindComponent<TextInput>().Instance );

        await component.InvokeAsync( () => component.Instance.SelectItem( "print" ) );
        await component.InvokeAsync( () => component.Instance.SelectItem( "info" ) );

        Assert.Equal( shouldPreserve, ReferenceEquals( input, component.FindComponent<TextInput>().Instance ) );
    }

    [Fact]
    public void VisibilityParameters_ShouldNotifyRenderedTransitionsWithoutEchoingTheBinding()
    {
        var notifications = new List<string>();

        var component = Render<RibbonBackstage>( parameters => parameters
            .Add( backstage => backstage.ElementId, "backstage" )
            .Add( backstage => backstage.SelectedItem, "info" )
            .Add( backstage => backstage.VisibleChanged, _ => notifications.Add( "binding" ) )
            .Add( backstage => backstage.Opened, () => notifications.Add( "opened" ) )
            .Add( backstage => backstage.Closed, () => notifications.Add( "closed" ) )
            .Add( backstage => backstage.ChildContent, CreateItems() ) );

        component.Render( parameters => parameters.Add( backstage => backstage.Visible, true ) );

        component.WaitForAssertion( () => Assert.Equal( new[] { "opened" }, notifications ) );

        component.Render( parameters => parameters.Add( backstage => backstage.Visible, false ) );

        component.WaitForAssertion( () => Assert.Equal( new[] { "opened", "closed" }, notifications ) );
    }

    [Fact]
    public async Task UnavailablePages_ShouldFallBackAndIgnoreCommandsAndDisabledEntries()
    {
        var notifications = new List<string>();

        var component = Render<RibbonBackstage>( parameters => parameters
            .Add( backstage => backstage.Visible, true )
            .Add( backstage => backstage.SelectedItem, "print" )
            .Add( backstage => backstage.SelectedItemChanged, name => notifications.Add( name ) )
            .Add( backstage => backstage.ChildContent, CreateItems() ) );

        component.Render( parameters => parameters.Add( backstage => backstage.ChildContent, CreateItems( printVisible: false ) ) );

        component.WaitForAssertion( () =>
        {
            Assert.Equal( "info", component.Instance.SelectedItem );
            Assert.Equal( new[] { "info" }, notifications );
            Assert.Empty( component.FindAll( "#backstage-print" ) );
        } );

        await component.InvokeAsync( () => component.Instance.SelectItem( "save" ) );
        await component.InvokeAsync( () => component.Instance.SelectItem( "print" ) );
        Assert.Equal( "info", component.Instance.SelectedItem );

        component.Render( parameters => parameters.Add( backstage => backstage.ChildContent, CreateItems( printDisabled: true ) ) );

        await component.Find( "#backstage-print" ).ClickAsync( new MouseEventArgs() );
        await component.InvokeAsync( () => component.Instance.SelectItem( "print" ) );

        Assert.Equal( "info", component.Instance.SelectedItem );
        Assert.Equal( new[] { "info" }, notifications );
    }

    [Fact]
    public void NavigationAttributesAndText_ShouldRefreshWithoutMutatingCallerAttributes()
    {
        var attributes = new Dictionary<string, object>
        {
            ["aria-label"] = "Document properties",
            ["data-page"] = "info",
        };

        var component = Render<RibbonBackstage>( parameters => parameters
            .Add( backstage => backstage.SelectedItem, "info" )
            .Add( backstage => backstage.ChildContent, CreateItems( infoAttributes: attributes ) ) );

        Assert.Equal( "Document properties", component.Find( "#backstage-info" ).GetAttribute( "aria-label" ) );
        Assert.Equal( "info", component.Find( "#backstage-info" ).GetAttribute( "data-page" ) );

        component.Render( parameters => parameters.Add( backstage => backstage.ChildContent, CreateItems( infoText: "Properties", infoAttributes: attributes ) ) );

        component.WaitForAssertion( () => Assert.Equal( "Properties", component.Find( "#backstage-info" ).TextContent.Trim() ) );
        Assert.Equal( 2, attributes.Count );
    }

    private RenderFragment CreateItems( Func<string, Task> clicked = null, bool printVisible = true, bool printDisabled = false, string infoText = "Info", Dictionary<string, object> infoAttributes = null ) => builder =>
    {
        builder.OpenComponent<RibbonBackstageItem>( 0 );
        builder.AddAttribute( 1, nameof( RibbonBackstageItem.Name ), "save" );
        builder.AddAttribute( 2, nameof( RibbonBackstageItem.ElementId ), "backstage-save" );
        builder.AddAttribute( 3, nameof( RibbonBackstageItem.Text ), "Save" );
        builder.AddAttribute( 4, nameof( RibbonBackstageItem.Clicked ), EventCallback.Factory.Create<MouseEventArgs>( this, () => clicked?.Invoke( "save" ) ?? Task.CompletedTask ) );
        builder.CloseComponent();

        builder.OpenComponent<RibbonBackstageItem>( 5 );
        builder.AddAttribute( 6, nameof( RibbonBackstageItem.Name ), "info" );
        builder.AddAttribute( 7, nameof( RibbonBackstageItem.ElementId ), "backstage-info" );
        builder.AddAttribute( 8, nameof( RibbonBackstageItem.Text ), infoText );
        builder.AddAttribute( 9, nameof( RibbonBackstageItem.ChildContent ), (RenderFragment)( content =>
        {
            content.OpenComponent<TextInput>( 0 );
            content.CloseComponent();
        } ) );
        builder.AddAttribute( 10, nameof( RibbonBackstageItem.Attributes ), infoAttributes );
        builder.CloseComponent();

        builder.OpenComponent<RibbonBackstageItem>( 11 );
        builder.AddAttribute( 12, nameof( RibbonBackstageItem.Name ), "print" );
        builder.AddAttribute( 13, nameof( RibbonBackstageItem.ElementId ), "backstage-print" );
        builder.AddAttribute( 14, nameof( RibbonBackstageItem.Text ), "Print" );
        builder.AddAttribute( 15, nameof( RibbonBackstageItem.Visible ), printVisible );
        builder.AddAttribute( 16, nameof( RibbonBackstageItem.Disabled ), printDisabled );
        builder.AddAttribute( 17, nameof( RibbonBackstageItem.Clicked ), EventCallback.Factory.Create<MouseEventArgs>( this, () => clicked?.Invoke( "print" ) ?? Task.CompletedTask ) );
        builder.AddAttribute( 18, nameof( RibbonBackstageItem.ChildContent ), (RenderFragment)( content => content.AddContent( 0, "Print settings" ) ) );
        builder.CloseComponent();
    };
}