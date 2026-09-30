#region Using directives
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.States;
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class StepsComponentTest : BunitContext
{
    public StepsComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseUtilities();

        var module = JSInterop.SetupModule( new JSTabsModule( JSInterop.JSRuntime, new MockVersionProvider(), new( null, _ => { } ) ).ModuleFileName );
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();
        module.SetupVoid( "destroy", _ => true ).SetVoidResult();
    }

    [Fact]
    public void Steps_ShouldAssociatePanelsWithinTheirOwnGroup()
    {
        var component = Render<StepsKeyboardComponent>();

        component.WaitForAssertion( () =>
        {
            foreach ( var groupName in new[] { "basic", "guarded" } )
            {
                var group = component.Find( $"#keyboard-steps-{groupName}" );
                Assert.Equal( $"{groupName} steps", group.QuerySelector( "[role=tablist]" ).GetAttribute( "aria-label" ) );

                foreach ( var step in group.QuerySelectorAll( "[role=tab]" ) )
                {
                    var panelId = step.GetAttribute( "aria-controls" );
                    var panel = group.QuerySelectorAll( "[role=tabpanel]" ).Single( candidate => candidate.Id == panelId );
                    Assert.Equal( step.Id, panel.GetAttribute( "aria-labelledby" ) );
                }

                Assert.Equal( $"first-panel-{groupName}", group.QuerySelector( "[role=tab]" ).GetAttribute( "aria-controls" ) );
            }

            Assert.All( component.FindAll( "#keyboard-steps-items-only [role=tab]" ), step => Assert.Null( step.GetAttribute( "aria-controls" ) ) );
            var ids = component.FindAll( "[id]" ).Select( element => element.Id ).ToArray();
            Assert.Equal( ids.Length, ids.Distinct().Count() );
        } );
    }

    [Fact]
    public async Task RemovingPanels_ShouldClearAssociationsAndPreserveStepNavigation()
    {
        var component = Render<StepsKeyboardComponent>();
        var steps = component.FindComponents<Steps>()[0];

        steps.Render( parameters => parameters.Add( item => item.Content, (RenderFragment)null ) );

        component.WaitForAssertion( () =>
        {
            Assert.Empty( component.FindAll( "#keyboard-steps-basic [role=tabpanel]" ) );
            var items = component.FindAll( "#keyboard-steps-basic [role=tab]" );
            Assert.Equal( 3, items.Count );
            Assert.All( items, item => Assert.Null( item.GetAttribute( "aria-controls" ) ) );
        } );

        await steps.InvokeAsync( () => steps.Instance.NextStep() );

        component.WaitForAssertion( () => Assert.Equal( "true", component.FindAll( "#keyboard-steps-basic [role=tab]" )[2].GetAttribute( "aria-selected" ) ) );
    }

    [Fact]
    public async Task Activation_ShouldRespectNavigationAllowed()
    {
        var component = Render<StepsKeyboardComponent>();
        var steps = component.FindAll( "#keyboard-steps-guarded [role=tab]" );

        await steps[2].ClickAsync();

        steps = component.FindAll( "#keyboard-steps-guarded [role=tab]" );
        Assert.Equal( "true", steps[1].GetAttribute( "aria-selected" ) );
        Assert.Equal( "false", steps[2].GetAttribute( "aria-selected" ) );

        await steps[0].ClickAsync();

        component.WaitForAssertion( () =>
        {
            var items = component.FindAll( "#keyboard-steps-guarded [role=tab]" );
            Assert.Equal( "true", items[0].GetAttribute( "aria-selected" ) );
            Assert.Equal( "0", items[0].GetAttribute( "tabindex" ) );
            Assert.Equal( "false", items[1].GetAttribute( "aria-selected" ) );
            Assert.Equal( "-1", items[1].GetAttribute( "tabindex" ) );
            Assert.Equal( "false", component.Find( "#first-panel-guarded" ).GetAttribute( "aria-hidden" ) );
        } );
    }

    [Theory]
    [InlineData( StepsRenderMode.Default, true )]
    [InlineData( StepsRenderMode.Default, false )]
    [InlineData( StepsRenderMode.LazyLoad, true )]
    [InlineData( StepsRenderMode.LazyLoad, false )]
    [InlineData( StepsRenderMode.LazyReload, true )]
    [InlineData( StepsRenderMode.LazyReload, false )]
    public void ActivePanel_ShouldUpdateTabIndexWithoutChangingContent( StepsRenderMode renderMode, bool focusable )
    {
        var component = Render<StepPanel>( parameters => parameters
            .AddCascadingValue( new StepsState { SelectedStep = "panel", RenderMode = renderMode } )
            .Add( panel => panel.Name, "panel" )
            .Add( panel => panel.Focusable, focusable )
            .AddChildContent( "<button type=\"button\">Panel action</button>" ) );

        var panel = component.Find( "[role=tabpanel]" );
        Assert.Equal( focusable ? "0" : "-1", panel.GetAttribute( "tabindex" ) );
        Assert.Equal( "false", panel.GetAttribute( "aria-hidden" ) );
        Assert.Contains( "active", panel.ClassList );
        var buttonMarkup = component.Find( "button" ).OuterHtml;

        component.Render( parameters => parameters.Add( item => item.Focusable, !focusable ) );

        panel = component.Find( "[role=tabpanel]" );
        Assert.Equal( focusable ? "-1" : "0", panel.GetAttribute( "tabindex" ) );
        Assert.Equal( "false", panel.GetAttribute( "aria-hidden" ) );
        Assert.Contains( "active", panel.ClassList );
        Assert.Equal( buttonMarkup, component.Find( "button" ).OuterHtml );
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    public void InactivePanel_ShouldStayOutsideTabSequence( bool focusable )
    {
        var component = Render<StepPanel>( parameters => parameters
            .AddCascadingValue( new StepsState { SelectedStep = "other" } )
            .Add( panel => panel.Name, "panel" )
            .Add( panel => panel.Focusable, focusable ) );

        var panel = component.Find( "[role=tabpanel]" );
        Assert.Equal( "-1", panel.GetAttribute( "tabindex" ) );
        Assert.Equal( "true", panel.GetAttribute( "aria-hidden" ) );
        Assert.DoesNotContain( "active", panel.ClassList );
    }

    [Theory]
    [InlineData( StepsRenderMode.Default )]
    [InlineData( StepsRenderMode.LazyLoad )]
    [InlineData( StepsRenderMode.LazyReload )]
    public void Panels_ShouldUpdateFocusAndVisibilityWithSelection( StepsRenderMode renderMode )
    {
        var component = Render<StepsKeyboardComponent>();
        var steps = component.FindComponents<Steps>()[0];

        steps.Render( parameters => parameters.Add( item => item.RenderMode, renderMode ) );

        var panels = component.FindAll( "#keyboard-steps-basic [role=tabpanel]" );
        Assert.Equal( "0", panels[1].GetAttribute( "tabindex" ) );
        Assert.Equal( "false", panels[1].GetAttribute( "aria-hidden" ) );
        Assert.Equal( "Second panel", panels[1].TextContent.Trim() );

        steps.Render( parameters => parameters.Add( item => item.SelectedStep, "1" ) );

        panels = component.FindAll( "#keyboard-steps-basic [role=tabpanel]" );
        Assert.Equal( "0", panels[0].GetAttribute( "tabindex" ) );
        Assert.Equal( "false", panels[0].GetAttribute( "aria-hidden" ) );
        Assert.Contains( "active", panels[0].ClassList );
        Assert.Equal( "First panel", panels[0].TextContent.Trim() );
        Assert.Equal( "-1", panels[1].GetAttribute( "tabindex" ) );
        Assert.Equal( "true", panels[1].GetAttribute( "aria-hidden" ) );
        Assert.DoesNotContain( "active", panels[1].ClassList );
        Assert.Equal( renderMode == StepsRenderMode.LazyReload ? string.Empty : "Second panel", panels[1].TextContent.Trim() );
    }
}