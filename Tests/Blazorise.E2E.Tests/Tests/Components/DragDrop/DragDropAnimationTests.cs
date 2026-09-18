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

        // Capture movement as the animation is created. CI can finish the drag
        // helper after the animation has already completed and been removed.
        var observation = await items.Nth( 1 ).EvaluateHandleAsync( """
            element => {
                const animate = element.animate;
                const observation = {
                    distance: 0,
                    restore: () => { delete element.animate; }
                };

                element.animate = function (...args) {
                    const animation = animate.apply(this, args);
                    animation.pause();
                    animation.currentTime = 0;
                    const start = this.getBoundingClientRect().top;
                    animation.currentTime = animation.effect.getTiming().duration / 2;
                    const end = this.getBoundingClientRect().top;
                    observation.distance = Math.max(observation.distance, Math.abs(end - start));
                    animation.currentTime = 0;
                    animation.play();
                    return animation;
                };

                return observation;
            }
            """ );

        try
        {
            await DragFirstItemOverThird( zone );
            await Page.WaitForFunctionAsync( "observation => observation.distance > 0", observation );

            var distance = await observation.EvaluateAsync<double>( "observation => observation.distance" );
            Assert.That( distance, Is.GreaterThan( 0 ) );
        }
        finally
        {
            await observation.EvaluateAsync( "observation => observation.restore()" );
            await observation.DisposeAsync();
        }

        await Expect( items ).ToHaveTextAsync( new[] { "Item 1", "Item 2", "Item 3", "Item 4" } );

        await Page.WaitForFunctionAsync( "selector => [...document.querySelectorAll(selector)].every(item => item.getAnimations().length === 0)", ".dropzone-1 " + ItemSelector );
        var precedingItem = await zone.Locator( ".draggable-placeholder" ).EvaluateAsync<string>( "element => element.previousElementSibling.textContent.trim()" );
        Assert.That( precedingItem, Is.EqualTo( "Item 3" ) );

        await DropIntoReorderSlot( zone );

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

        // Animated controls item reordering, not CSS transitions on the zone or item content.
        var animations = await zone.Locator( ItemSelector ).EvaluateAllAsync<string[]>( """
            elements => elements.flatMap(element => element.getAnimations().map(animation => JSON.stringify({
                item: element.textContent.trim(),
                type: animation.constructor.name,
                timing: animation.effect.getTiming(),
                keyframes: animation.effect.getKeyframes()
            })))
            """ );
        Assert.That( animations, Is.Empty, string.Join( Environment.NewLine, animations ) );

        await DropIntoReorderSlot( zone );

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

        await DropIntoReorderSlot( zone );

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

    private async Task DropIntoReorderSlot( ILocator zone )
    {
        var placeholder = zone.Locator( "[data-index='2'] + [data-reorder-placeholder='true']" );
        await Expect( placeholder ).ToHaveCountAsync( 1 );

        // Hidden placeholders still reserve space, so read their layout bounds directly.
        var position = await placeholder.EvaluateAsync<float[]>( """
            element => {
                const rect = element.getBoundingClientRect();
                return [rect.left + rect.width / 2, rect.top + rect.height / 2];
            }
            """ );

        // Reordering changes the element beneath the pointer. Send dragover to the
        // final drop location before releasing, even when the layout moved instantly.
        await Page.Mouse.MoveAsync( position[0], position[1] );
        await Page.Mouse.MoveAsync( position[0], position[1] );
        await Expect( placeholder ).ToHaveCountAsync( 1 );
        await Expect( zone ).ToHaveAttributeAsync( "data-transaction-current", "true" );
        await Page.Mouse.UpAsync();
        await Expect( zone ).ToHaveAttributeAsync( "data-transaction-active", "false" );
    }

    private async Task DragFirstItemOverThird( ILocator zone, bool showPlaceholder = false )
    {
        var items = zone.Locator( ItemSelector );
        var sourceItem = items.Nth( 0 );

        // Wait for the actual item to receive pointer events before starting the native drag.
        await sourceItem.HoverAsync();
        var source = await sourceItem.BoundingBoxAsync();

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