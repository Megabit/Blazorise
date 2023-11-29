#region Using directives
using System.Linq;
using System.Web;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class ListViewComponentTest : TestContext
{
    public ListViewComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProvidersTests().AddTestData();
        JSInterop.AddBlazoriseUtilities();
    }

    [Fact]
    public void RenderTest()
    {
        // setup
        var liOpen = "<li";
        var liClose = "</li>";
        var ulOpen = "<ul";
        var ulClose = "</ul>";
        var listViewClass = "list-group-scrollable";

        // test
        var comp = RenderComponent<ListViewComponent>();
        var htmlLists = comp.FindAll( "li" );
        var htmlList = comp.Find( "ul" );

        // validate
        Assert.Contains( listViewClass, htmlList.OuterHtml );
        Assert.Contains( ulOpen, htmlList.OuterHtml );
        Assert.Contains( ulClose, htmlList.OuterHtml );

        for ( int i = 0; i < htmlLists.Count; i++ )
        {
            var currentList = htmlLists[i];
            Assert.Contains( liOpen, currentList.OuterHtml );
            Assert.Contains( liClose, currentList.OuterHtml );
        }
    }

    [Fact]
    public void ProvidedData_ShouldRender_ListItems()
    {
        // test
        var comp = RenderComponent<ListViewComponent>();
        var htmlLists = comp.FindAll( "li" );

        // validate
        for ( int i = 0; i < htmlLists.Count; i++ )
        {
            Assert.Equal( comp.Instance.Countries.ElementAt( i ).Name, HttpUtility.HtmlDecode( htmlLists[i].InnerHtml ) );
        }
    }

}