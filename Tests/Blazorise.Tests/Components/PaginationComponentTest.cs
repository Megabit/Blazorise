using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace Blazorise.Tests.Components;

public class PaginationComponentTest : TestContext
{
    public PaginationComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
    }

    [Fact]
    public void EnabledPaginationLink_ShouldBeTabbable()
    {
        var cut = RenderPaginationLink();

        var link = cut.Find( "a" );

        Assert.Equal( "0", link.GetAttribute( "tabindex" ) );
        Assert.False( link.HasAttribute( "aria-disabled" ) );
    }

    [Fact]
    public void DisabledPaginationLink_ShouldNotBeTabbable()
    {
        var cut = RenderPaginationLink( disabled: true );

        var link = cut.Find( "a" );

        Assert.Equal( "-1", link.GetAttribute( "tabindex" ) );
        Assert.Equal( "true", link.GetAttribute( "aria-disabled" ) );
    }

    [Fact]
    public void ActivePaginationLink_ShouldSetAriaCurrent()
    {
        var cut = RenderPaginationLink( active: true );

        var link = cut.Find( "a" );

        Assert.Equal( "page", link.GetAttribute( "aria-current" ) );
    }

    private IRenderedComponent<Pagination> RenderPaginationLink( bool disabled = false, bool active = false )
    {
        return RenderComponent<Pagination>( parameters => parameters
            .AddChildContent( builder =>
            {
                builder.OpenComponent<PaginationItem>( 0 );
                builder.AddAttribute( 1, nameof( PaginationItem.Disabled ), disabled );
                builder.AddAttribute( 2, nameof( PaginationItem.Active ), active );
                builder.AddAttribute( 3, nameof( PaginationItem.ChildContent ), (RenderFragment)( childBuilder =>
                {
                    childBuilder.OpenComponent<PaginationLink>( 0 );
                    childBuilder.AddAttribute( 1, nameof( PaginationLink.Page ), "1" );
                    childBuilder.AddAttribute( 2, nameof( PaginationLink.ChildContent ), (RenderFragment)( contentBuilder => contentBuilder.AddContent( 0, "1" ) ) );
                    childBuilder.CloseComponent();
                } ) );
                builder.CloseComponent();
            } ) );
    }
}