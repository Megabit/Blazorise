#region Using directives
using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class CarouselComponentTest : BunitContext
{
    public CarouselComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseUtilities();
    }

    [Fact]
    public void UnboundSelectedSlide_ShouldDefaultToFirstSlide()
    {
        // setup
        var selectedSlideChangedCount = 0;

        // test
        var comp = Render<Carousel>( parameters => parameters
            .Add( x => x.Interval, 0 )
            .Add( x => x.SelectedSlideChanged, ( string _ ) => selectedSlideChangedCount++ )
            .AddChildContent( CreateUnnamedSlides() ) );

        // validate
        comp.WaitForAssertion( () =>
        {
            var slides = comp.FindAll( ".carousel-item" );

            Assert.Equal( 2, slides.Count );
            Assert.Contains( "active", slides[0].GetAttribute( "class" ) );
            Assert.DoesNotContain( "active", slides[1].GetAttribute( "class" ) );
            Assert.Equal( 0, selectedSlideChangedCount );
        } );
    }

    private static RenderFragment CreateUnnamedSlides()
        => builder =>
        {
            builder.OpenComponent<CarouselSlide>( 0 );
            builder.AddAttribute( 1, nameof( CarouselSlide.ChildContent ), (RenderFragment)( slideBuilder => slideBuilder.AddContent( 0, "First slide" ) ) );
            builder.CloseComponent();

            builder.OpenComponent<CarouselSlide>( 2 );
            builder.AddAttribute( 3, nameof( CarouselSlide.ChildContent ), (RenderFragment)( slideBuilder => slideBuilder.AddContent( 0, "Second slide" ) ) );
            builder.CloseComponent();
        };
}