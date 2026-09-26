using System.Linq;
using System.Threading.Tasks;
using Blazorise.Modules;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Blazorise.Tests.Components;

public class TabsComponentTest : BunitContext
{
    public TabsComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseUtilities();

        var module = JSInterop.SetupModule( new JSTabsModule( JSInterop.JSRuntime, new MockVersionProvider(), new( null, ( Options ) => { } ) ).ModuleFileName );
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();
        module.SetupVoid( "destroy", _ => true ).SetVoidResult();
    }

    [Fact]
    public void Tabs_ShouldAssociateEachPanelWithItsOwnTabGroup()
    {
        var component = Render<TabsKeyboardComponent>();

        component.WaitForAssertion( () =>
        {
            foreach ( string groupId in new[] { "keyboard-tabs-Top", "keyboard-tabs-Start", "keyboard-tabs-skip-panel" } )
            {
                var group = component.Find( $"#{groupId}" );

                foreach ( var tab in group.QuerySelectorAll( "[role=tab]" ) )
                {
                    string panelId = tab.GetAttribute( "aria-controls" );
                    Assert.False( string.IsNullOrEmpty( panelId ) );

                    var panel = group.QuerySelectorAll( "[role=tabpanel]" ).Single( candidate => candidate.Id == panelId );
                    Assert.False( string.IsNullOrEmpty( tab.Id ) );
                    Assert.Equal( tab.Id, panel.GetAttribute( "aria-labelledby" ) );
                }
            }

            string[] ids = component.FindAll( "[id]" ).Select( element => element.Id ).ToArray();
            Assert.Equal( ids.Length, ids.Distinct().Count() );
        } );
    }

    [Fact]
    public void Tabs_ShouldLabelTheTabList()
    {
        var component = Render<TabsKeyboardComponent>();

        Assert.Equal( "Top tabs", component.Find( "#keyboard-tabs-Top [role=tablist]" ).GetAttribute( "aria-label" ) );
        Assert.Equal( "skip-panel-heading", component.Find( "#keyboard-tabs-skip-panel [role=tablist]" ).GetAttribute( "aria-labelledby" ) );

        var tabs = component.FindComponents<Tabs>()[0];
        tabs.Render( parameters => parameters.Add( tab => tab.AriaLabel, "Custom label" ) );
        Assert.Equal( "Custom label", component.Find( "#keyboard-tabs-Top [role=tablist]" ).GetAttribute( "aria-label" ) );

        tabs.Render( parameters => parameters.Add( tab => tab.AriaLabelledBy, "skip-panel-heading" ) );
        Assert.Equal( "skip-panel-heading", component.Find( "#keyboard-tabs-Top [role=tablist]" ).GetAttribute( "aria-labelledby" ) );
    }

    [Fact]
    public void Tab_ShouldApplyAriaLabelledByToTheTabElement()
    {
        var component = Render<TabsKeyboardComponent>();
        var tab = component.FindComponents<Tab>()[0];

        tab.Render( parameters => parameters.Add( item => item.AriaLabelledBy, "skip-panel-heading" ) );

        var tabElement = component.Find( "#keyboard-tabs-Top [role=tab]" );
        Assert.Equal( "skip-panel-heading", tabElement.GetAttribute( "aria-labelledby" ) );
        Assert.Equal( tabElement.Id, component.Find( "#keyboard-tabs-Top [role=tabpanel]" ).GetAttribute( "aria-labelledby" ) );
    }

    [Fact]
    public void Associations_ShouldUseExplicitElementIds()
    {
        var component = Render<TabsKeyboardComponent>();

        component.WaitForAssertion( () =>
        {
            var tabElement = component.Find( "#first-tab-Top [role=tab]" );
            Assert.Equal( "first-panel-Top", tabElement.GetAttribute( "aria-controls" ) );
            Assert.Equal( tabElement.Id, component.Find( "#first-panel-Top" ).GetAttribute( "aria-labelledby" ) );
        } );
    }

    [Fact]
    public void Associations_ShouldRefreshWhenTabsAreRemovedAndRecreated()
    {
        var component = Render<TabsKeyboardComponent>();
        var tabs = component.FindComponents<Tabs>()[0];
        RenderFragment items = tabs.Instance.Items;
        string panelId = component.Find( "#keyboard-tabs-Top [role=tabpanel]" ).Id;

        tabs.Render( parameters => parameters.Add( item => item.Items, (RenderFragment)null ) );

        component.WaitForAssertion( () =>
        {
            Assert.Empty( component.FindAll( "#keyboard-tabs-Top [role=tab]" ) );
            Assert.Null( component.Find( "#keyboard-tabs-Top [role=tabpanel]" ).GetAttribute( "aria-labelledby" ) );
        } );

        tabs.Render( parameters => parameters.Add( item => item.Items, items ) );

        component.WaitForAssertion( () =>
        {
            var tabElement = component.Find( "#keyboard-tabs-Top [role=tab]" );
            var panelElement = component.Find( "#keyboard-tabs-Top [role=tabpanel]" );
            Assert.Equal( panelId, panelElement.Id );
            Assert.Equal( panelId, tabElement.GetAttribute( "aria-controls" ) );
            Assert.Equal( tabElement.Id, panelElement.GetAttribute( "aria-labelledby" ) );
        } );
    }

    [Fact]
    public async Task CanSelectTabs()
    {
        // setup
        var comp = Render<TabsComponent>();
        var paragraph = comp.Find( "#basic-tabs" );
        var links = comp.FindAll( "a" );
        var panels = comp.FindAll( ".tab-pane" );

        Assert.NotEmpty( links );
        Assert.NotEmpty( panels );

        // test 1
        Assert.DoesNotContain( "show", links[0].GetAttribute( "class" ) );
        Assert.Contains( "show", links[1].GetAttribute( "class" ) );
        Assert.DoesNotContain( "show", links[2].GetAttribute( "class" ) );
        Assert.DoesNotContain( "show", panels[0].GetAttribute( "class" ) );
        Assert.Contains( "show", panels[1].GetAttribute( "class" ) );
        Assert.DoesNotContain( "show", panels[2].GetAttribute( "class" ) );

        // test 2
        await links[0].ClickAsync();
        panels = comp.FindAll( "a" );
        Assert.Contains( "show", panels[0].GetAttribute( "class" ) );
        Assert.DoesNotContain( "show", panels[1].GetAttribute( "class" ) );
        Assert.DoesNotContain( "show", panels[2].GetAttribute( "class" ) );

        // test 3
        await links[2].ClickAsync();
        panels = comp.FindAll( "a" );
        Assert.DoesNotContain( "show", panels[0].GetAttribute( "class" ) );
        Assert.DoesNotContain( "show", panels[1].GetAttribute( "class" ) );
        Assert.Contains( "show", panels[2].GetAttribute( "class" ) );
    }
}