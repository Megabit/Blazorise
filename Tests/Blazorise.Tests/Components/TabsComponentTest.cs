using System.Threading.Tasks;
using Bunit;
using Xunit;

namespace Blazorise.Tests.Components;

public class TabsComponentTest : TestContext
{
    public TabsComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseUtilities();
    }

    [Fact]
    public async Task CanSelectTabs()
    {
        // setup
        var comp = RenderComponent<TabsComponent>();
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