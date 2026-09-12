namespace Blazorise.E2E.Tests.Tests.Components.DragDrop;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class DragDropAnimationTests : BlazorisePageTest
{
    private const string ItemSelector = ".b-drop-zone-draggable:not(.draggable-preview-start)";

    [Test]
    public async Task ReorderingAnimatesNeighborsBeforeDrop()
    {
        await Page.EmulateMediaAsync( new() { ReducedMotion = ReducedMotion.NoPreference } );
        await SelectTestComponent<DropZoneAnimationComponent>();

        var zone = Page.Locator( ".dropzone-1" );
        var items = zone.Locator( ItemSelector );
        var sourceId = await items.Nth( 0 ).GetAttributeAsync( "id" );

        await DragFirstItemOverThird( zone );

        await Page.WaitForFunctionAsync( "selector => [...document.querySelectorAll(selector)].some(item => item.getAnimations().length > 0)", ".dropzone-1 " + ItemSelector );

        var distance = await items.Nth( 3 ).EvaluateAsync<double>( """
            element => {
                const animation = element.getAnimations()[0];
                animation.pause();
                animation.currentTime = 0;
                const start = element.getBoundingClientRect().top;
                animation.currentTime = 500;
                const end = element.getBoundingClientRect().top;
                animation.play();
                return Math.abs(end - start);
            }
            """ );

        Assert.That( distance, Is.GreaterThan( 0 ) );
        await Expect( items ).ToHaveTextAsync( new[] { "Item 1", "Item 2", "Item 3", "Item 4" } );

        await Page.WaitForFunctionAsync( "selector => [...document.querySelectorAll(selector)].every(item => item.getAnimations().length === 0)", ".dropzone-1 " + ItemSelector );
        var precedingItem = await zone.Locator( ".draggable-placeholder" ).EvaluateAsync<string>( "element => element.previousElementSibling.textContent.trim()" );
        Assert.That( precedingItem, Is.EqualTo( "Item 3" ) );

        await Page.Mouse.UpAsync();

        await Expect( items ).ToHaveTextAsync( new[] { "Item 2", "Item 3", "Item 1", "Item 4" } );
        await Expect( items.Nth( 2 ) ).ToHaveAttributeAsync( "id", sourceId );
        await Expect( zone ).ToHaveAttributeAsync( "data-transaction-active", "false" );
    }

    [TestCase( true )]
    [TestCase( false )]
    public async Task ReorderingWorksWithoutMotion( bool reducedMotion )
    {
        await Page.EmulateMediaAsync( new() { ReducedMotion = reducedMotion ? ReducedMotion.Reduce : ReducedMotion.NoPreference } );
        await SelectTestComponent<DropZoneAnimationComponent>();

        if ( !reducedMotion )
            await Page.Locator( "#toggle-animation" ).ClickAsync();

        var zone = Page.Locator( ".dropzone-1" );
        await DragFirstItemOverThird( zone );

        var animationCount = await zone.EvaluateAsync<int>( "element => element.getAnimations({ subtree: true }).length" );
        Assert.That( animationCount, Is.Zero );

        await Page.Mouse.UpAsync();

        await Expect( zone.Locator( ItemSelector ) ).ToHaveTextAsync( new[] { "Item 2", "Item 3", "Item 1", "Item 4" } );
    }

    private async Task DragFirstItemOverThird( ILocator zone )
    {
        var items = zone.Locator( ItemSelector );
        var source = await items.Nth( 0 ).BoundingBoxAsync();

        await Page.Mouse.MoveAsync( source.X + source.Width / 2, source.Y + source.Height / 2 );
        await Page.Mouse.DownAsync();
        await Page.Mouse.MoveAsync( source.X + source.Width / 2 + 10, source.Y + source.Height / 2, new() { Steps = 5 } );
        await Expect( items.Nth( 0 ) ).ToHaveAttributeAsync( "data-dragging", "true" );

        var target = await items.Nth( 2 ).BoundingBoxAsync();

        await Page.Mouse.MoveAsync( target.X + target.Width / 2, target.Y + target.Height / 2 );
        await Page.Mouse.MoveAsync( target.X + target.Width / 2 + 1, target.Y + target.Height / 2 );
        await Expect( zone.Locator( ".draggable-placeholder" ) ).ToBeVisibleAsync();
    }
}