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

        var distance = await items.Nth( 1 ).EvaluateAsync<double>( """
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
        await Expect( items.Nth( 2 ) ).ToBeVisibleAsync();
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
        await DragFirstItemOverThird( zone, !reducedMotion );

        var animationCount = await zone.EvaluateAsync<int>( "element => element.getAnimations({ subtree: true }).length" );
        Assert.That( animationCount, Is.Zero );

        await Page.Mouse.UpAsync();

        await Expect( zone.Locator( ItemSelector ) ).ToHaveTextAsync( new[] { "Item 2", "Item 3", "Item 1", "Item 4" } );
    }

    [TestCase( true )]
    [TestCase( false )]
    public async Task CancelingReorderRestoresSourceAndRemovesSlot( bool animated )
    {
        await SelectTestComponent<DropZoneAnimationComponent>();

        if ( !animated )
            await Page.Locator( "#toggle-animation" ).ClickAsync();

        var zone = Page.Locator( ".dropzone-1" );
        await DragFirstItemOverThird( zone, !animated );
        await Page.Keyboard.PressAsync( "Escape" );
        await Page.Mouse.UpAsync();

        await Expect( zone ).ToHaveAttributeAsync( "data-transaction-active", "false" );
        await Expect( zone.Locator( ItemSelector ).Nth( 0 ) ).ToBeVisibleAsync();
        await Expect( zone.Locator( ".draggable-placeholder" ) ).ToBeHiddenAsync();
        await Expect( zone.Locator( ItemSelector ) ).ToHaveTextAsync( new[] { "Item 1", "Item 2", "Item 3", "Item 4" } );
    }

    [TestCase( true, true )]
    [TestCase( true, false )]
    [TestCase( false, true )]
    [TestCase( false, false )]
    public async Task PlaceholderVisibilityPreservesMovingSlot( bool animated, bool showPlaceholder )
    {
        await SelectTestComponent<DropZoneAnimationComponent>();

        if ( !animated )
            await Page.Locator( "#toggle-animation" ).ClickAsync();

        if ( showPlaceholder != !animated )
            await Page.Locator( "#toggle-placeholder" ).ClickAsync();

        var zone = Page.Locator( ".dropzone-1" );
        await DragFirstItemOverThird( zone, showPlaceholder );

        if ( showPlaceholder )
            await Expect( zone.Locator( ".draggable-placeholder" ) ).ToHaveCSSAsync( "outline-style", "dashed" );

        await Page.Mouse.UpAsync();

        await Expect( zone.Locator( ItemSelector ) ).ToHaveTextAsync( new[] { "Item 2", "Item 3", "Item 1", "Item 4" } );
        await Expect( zone.Locator( ItemSelector ).Nth( 2 ) ).ToBeVisibleAsync();
    }

    [TestCase( true )]
    [TestCase( false )]
    public async Task ReorderingPreservesFlexZoneAndItemWidths( bool animated )
    {
        await Page.EmulateMediaAsync( new() { ReducedMotion = ReducedMotion.NoPreference } );
        await SelectTestComponent<DropZoneAnimationComponent>();

        if ( !animated )
            await Page.Locator( "#toggle-animation" ).ClickAsync();

        var zones = Page.Locator( ".b-drop-zone" );
        var widths = await zones.EvaluateAllAsync<double[]>( "elements => elements.map(element => element.getBoundingClientRect().width)" );
        var sourceZone = Page.Locator( ".dropzone-1" );
        var sourceItem = await sourceZone.Locator( ItemSelector ).Nth( 1 ).BoundingBoxAsync();
        var targetZone = Page.Locator( ".dropzone-2" );
        var targetItem = await targetZone.Locator( ItemSelector ).Nth( 0 ).BoundingBoxAsync();

        await DragFirstItemOverThird( sourceZone, !animated );
        await AssertWidths( sourceZone, sourceItem.Width );

        await Page.Mouse.MoveAsync( targetItem.X + targetItem.Width / 2, targetItem.Y + targetItem.Height / 2 );
        await Page.Mouse.MoveAsync( targetItem.X + targetItem.Width / 2 + 1, targetItem.Y + targetItem.Height / 2 );
        await Expect( targetZone ).ToHaveAttributeAsync( "data-transaction-current", "true" );
        await AssertWidths( targetZone, targetItem.Width );

        await Page.Keyboard.PressAsync( "Escape" );
        await Page.Mouse.UpAsync();
        await Expect( sourceZone ).ToHaveAttributeAsync( "data-transaction-active", "false" );

        async Task AssertWidths( ILocator currentZone, float expectedSlotWidth )
        {
            var currentWidths = await zones.EvaluateAllAsync<double[]>( "elements => elements.map(element => element.getBoundingClientRect().width)" );

            for ( var i = 0; i < widths.Length; i++ )
                Assert.That( currentWidths[i], Is.EqualTo( widths[i] ).Within( 1 ) );

            var slotWidth = await currentZone.Locator( "[data-reorder-placeholder='true']" )
                .EvaluateAsync<double>( "element => element.getBoundingClientRect().width" );
            Assert.That( slotWidth, Is.EqualTo( expectedSlotWidth ).Within( 1 ) );
            Assert.That( ( await sourceZone.Locator( ItemSelector ).Nth( 1 ).BoundingBoxAsync() ).Width, Is.EqualTo( sourceItem.Width ).Within( 1 ) );
            Assert.That( ( await targetZone.Locator( ItemSelector ).Nth( 0 ).BoundingBoxAsync() ).Width, Is.EqualTo( targetItem.Width ).Within( 1 ) );
        }
    }

    private async Task DragFirstItemOverThird( ILocator zone, bool showPlaceholder = false )
    {
        var items = zone.Locator( ItemSelector );
        var source = await items.Nth( 0 ).BoundingBoxAsync();

        await Page.Mouse.MoveAsync( source.X + source.Width / 2, source.Y + source.Height / 2 );
        await Page.Mouse.DownAsync();
        await Page.Mouse.MoveAsync( source.X + source.Width / 2 + 10, source.Y + source.Height / 2, new() { Steps = 5 } );
        await Expect( items.Nth( 0 ) ).ToHaveAttributeAsync( "data-dragging", "true" );

        await Expect( items.Nth( 0 ) ).ToBeHiddenAsync();
        var placeholder = zone.Locator( "[data-reorder-placeholder='true']" );
        await Expect( placeholder ).ToHaveCSSAsync( "visibility", showPlaceholder ? "visible" : "hidden" );
        await Expect( placeholder ).ToHaveAttributeAsync( "style", new System.Text.RegularExpressions.Regex( "min-height:" ) );
        var slotHeight = await placeholder.EvaluateAsync<double>( "element => element.getBoundingClientRect().height" );
        Assert.That( slotHeight, Is.EqualTo( source.Height ).Within( 1 ) );
        await Expect( zone.Locator( ItemSelector + ":visible" ) ).ToHaveTextAsync( new[] { "Item 2", "Item 3", "Item 4" } );

        var target = await items.Nth( 2 ).BoundingBoxAsync();

        await Page.Mouse.MoveAsync( target.X + target.Width / 2, target.Y + target.Height / 2 );
        await Page.Mouse.MoveAsync( target.X + target.Width / 2 + 1, target.Y + target.Height / 2 );
        await Expect( zone.Locator( "[data-index='2'] + .draggable-placeholder" ) ).ToHaveCountAsync( 1 );
        await Expect( placeholder ).ToHaveCSSAsync( "visibility", showPlaceholder ? "visible" : "hidden" );
    }
}