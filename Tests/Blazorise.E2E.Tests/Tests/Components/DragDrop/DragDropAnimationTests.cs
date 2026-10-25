namespace Blazorise.E2E.Tests.Tests.Components.DragDrop;

[Parallelizable( ParallelScope.Self )]
[TestFixture]
public class DragDropAnimationTests : BlazorisePageTest
{
    private const string ItemSelector = "[draggable='true']";

    [TestCase( true )]
    [TestCase( false )]
    public async Task ReorderingUpdatesItemsAndRaisesDropped( bool animated )
    {
        var sut = await SelectExample( animated );
        var zone = sut.Locator( "#drop-zone-1" );
        var items = zone.Locator( ItemSelector );
        var sourceId = await items.Nth( 0 ).GetAttributeAsync( "id" );

        await DragFirstItemOverThird( zone );

        await Expect( items ).ToHaveTextAsync( new[] { "Item 1", "Item 2", "Item 3", "Item 4" } );
        await Expect( sut.Locator( "#drop-count" ) ).ToHaveTextAsync( "0" );

        await CompleteDrop( zone, "Item 3" );

        await Expect( items ).ToHaveTextAsync( new[] { "Item 2", "Item 3", "Item 1", "Item 4" } );
        await Expect( items.Nth( 2 ) ).ToHaveAttributeAsync( "id", sourceId );
        await Expect( items.Nth( 2 ) ).ToBeVisibleAsync();
        await Expect( sut.Locator( "#drop-result" ) ).ToHaveTextAsync( "Item 1: 1 -> 1 (2)" );
        await Expect( sut.Locator( "#drop-count" ) ).ToHaveTextAsync( "1" );
    }

    [Test]
    public async Task ReorderingWorksWithReducedMotion()
    {
        await Page.EmulateMediaAsync( new() { ReducedMotion = ReducedMotion.Reduce } );
        var sut = await SelectExample( true );
        var zone = sut.Locator( "#drop-zone-1" );

        await DragFirstItemOverThird( zone );
        await CompleteDrop( zone, "Item 3" );

        await Expect( zone.Locator( ItemSelector ) ).ToHaveTextAsync( new[] { "Item 2", "Item 3", "Item 1", "Item 4" } );
        await Expect( sut.Locator( "#drop-result" ) ).ToHaveTextAsync( "Item 1: 1 -> 1 (2)" );
        await Expect( sut.Locator( "#drop-count" ) ).ToHaveTextAsync( "1" );
    }

    [TestCase( true )]
    [TestCase( false )]
    public async Task CancelingReorderRestoresItemsWithoutRaisingDropped( bool animated )
    {
        var sut = await SelectExample( animated );
        var zone = sut.Locator( "#drop-zone-1" );

        await DragFirstItemOverThird( zone );
        await Page.Keyboard.PressAsync( "Escape" );
        await Page.Mouse.UpAsync();

        await Expect( zone.Locator( ItemSelector ).Nth( 0 ) ).ToBeVisibleAsync();
        await Expect( zone.Locator( ".drop-placeholder" ) ).ToBeHiddenAsync();
        await Expect( zone.Locator( ItemSelector ) ).ToHaveTextAsync( new[] { "Item 1", "Item 2", "Item 3", "Item 4" } );
        await Expect( sut.Locator( "#drop-result" ) ).ToHaveTextAsync( string.Empty );
        await Expect( sut.Locator( "#drop-count" ) ).ToHaveTextAsync( "0" );
    }

    [TestCase( true, true )]
    [TestCase( true, false )]
    [TestCase( false, true )]
    [TestCase( false, false )]
    public async Task PlaceholderVisibilityPreservesMovingSlot( bool animated, bool showPlaceholder )
    {
        var sut = await SelectExample( animated );

        if ( showPlaceholder != !animated )
        {
            await sut.Locator( "#toggle-placeholder" ).ClickAsync();
        }

        await Expect( sut.Locator( "#toggle-placeholder" ) ).ToHaveTextAsync( $"Show placeholder: {showPlaceholder}" );

        var zone = sut.Locator( "#drop-zone-1" );
        var source = await zone.Locator( ItemSelector ).Nth( 0 ).BoundingBoxAsync();

        await DragFirstItemOverThird( zone );

        var placeholder = zone.Locator( ".drop-placeholder" );
        await Expect( placeholder ).ToHaveCSSAsync( "visibility", showPlaceholder ? "visible" : "hidden" );
        var slot = await placeholder.BoundingBoxAsync();
        Assert.That( slot, Is.Not.Null );
        Assert.That( slot.Height, Is.EqualTo( source.Height ).Within( 1 ) );

        if ( showPlaceholder )
        {
            await Expect( placeholder ).ToBeVisibleAsync();
            await Expect( placeholder ).ToHaveCSSAsync( "outline-style", "dashed" );
        }

        await CompleteDrop( zone, "Item 3" );

        await Expect( zone.Locator( ItemSelector ) ).ToHaveTextAsync( new[] { "Item 2", "Item 3", "Item 1", "Item 4" } );
        await Expect( sut.Locator( "#drop-count" ) ).ToHaveTextAsync( "1" );
    }

    [TestCase( true )]
    [TestCase( false )]
    public async Task CanMoveItemToAnotherZone( bool animated )
    {
        var sut = await SelectExample( animated );
        var sourceZone = sut.Locator( "#drop-zone-1" );
        var targetZone = sut.Locator( "#drop-zone-2" );

        await StartDrag( sourceZone.Locator( ItemSelector ).Nth( 0 ) );
        await MoveOver( targetZone.Locator( ItemSelector ).Nth( 0 ) );
        await CompleteDrop( targetZone, "Item 5" );

        await Expect( sourceZone.Locator( ItemSelector ) ).ToHaveTextAsync( new[] { "Item 2", "Item 3", "Item 4" } );
        await Expect( targetZone.Locator( ItemSelector ) ).ToHaveTextAsync( new[] { "Item 5", "Item 1", "Item 6" } );
        await Expect( sut.Locator( "#drop-result" ) ).ToHaveTextAsync( "Item 1: 1 -> 2 (1)" );
        await Expect( sut.Locator( "#drop-count" ) ).ToHaveTextAsync( "1" );
    }

    [Test]
    public async Task CanMoveItemToEmptyZone()
    {
        var sut = await SelectExample( true );
        var sourceZone = sut.Locator( "#drop-zone-1" );
        var targetZone = sut.Locator( "#drop-zone-3" );
        await Expect( targetZone.Locator( ItemSelector ) ).ToHaveCountAsync( 0 );

        await StartDrag( sourceZone.Locator( ItemSelector ).Nth( 0 ) );
        await MoveOver( targetZone );
        await CompleteDrop( targetZone );

        await Expect( sourceZone.Locator( ItemSelector ) ).ToHaveTextAsync( new[] { "Item 2", "Item 3", "Item 4" } );
        await Expect( targetZone.Locator( ItemSelector ) ).ToHaveTextAsync( new[] { "Item 1" } );
        await Expect( sut.Locator( "#drop-result" ) ).ToHaveTextAsync( "Item 1: 1 -> 3 (0)" );
        await Expect( sut.Locator( "#drop-count" ) ).ToHaveTextAsync( "1" );
    }

    [TestCase( true )]
    [TestCase( false )]
    public async Task ReorderingPreservesFlexZoneAndItemWidths( bool animated )
    {
        var sut = await SelectExample( animated );
        var zones = new[] { sut.Locator( "#drop-zone-1" ), sut.Locator( "#drop-zone-2" ), sut.Locator( "#drop-zone-3" ) };
        var widths = new List<float>();

        foreach ( var zone in zones )
        {
            await zone.HoverAsync();
            widths.Add( ( await zone.BoundingBoxAsync() ).Width );
        }

        var sourceZone = zones[0];
        var targetZone = zones[1];
        var sourceItem = await sourceZone.Locator( ItemSelector ).Nth( 1 ).BoundingBoxAsync();
        var targetItem = await targetZone.Locator( ItemSelector ).Nth( 0 ).BoundingBoxAsync();

        await DragFirstItemOverThird( sourceZone );
        await AssertWidths( sourceZone, sourceItem.Width );

        await MoveOver( targetZone.Locator( ItemSelector ).Nth( 0 ) );
        await Expect( SlotAfter( targetZone, "Item 5" ) ).ToHaveCountAsync( 1 );
        await AssertWidths( targetZone, targetItem.Width );

        await Page.Keyboard.PressAsync( "Escape" );
        await Page.Mouse.UpAsync();
        await Expect( sourceZone.Locator( ItemSelector ).Nth( 0 ) ).ToBeVisibleAsync();
        await Expect( sut.Locator( "#drop-count" ) ).ToHaveTextAsync( "0" );

        async Task AssertWidths( ILocator currentZone, float expectedSlotWidth )
        {
            for ( var index = 0; index < zones.Length; index++ )
            {
                Assert.That( ( await zones[index].BoundingBoxAsync() ).Width, Is.EqualTo( widths[index] ).Within( 1 ) );
            }

            var slot = await currentZone.Locator( ".drop-placeholder" ).BoundingBoxAsync();
            Assert.That( slot, Is.Not.Null );
            Assert.That( slot.Width, Is.EqualTo( expectedSlotWidth ).Within( 1 ) );
            Assert.That( ( await sourceZone.Locator( ItemSelector ).Nth( 1 ).BoundingBoxAsync() ).Width, Is.EqualTo( sourceItem.Width ).Within( 1 ) );
            Assert.That( ( await targetZone.Locator( ItemSelector ).Nth( 0 ).BoundingBoxAsync() ).Width, Is.EqualTo( targetItem.Width ).Within( 1 ) );
        }
    }

    private async Task<ILocator> SelectExample( bool animated )
    {
        await SelectTestComponent<DropZoneAnimationComponent>();

        var sut = Page.Locator( "#drop-editing" );
        await Expect( sut.Locator( "#drop-board" ) ).ToHaveCSSAsync( "display", "flex" );
        await Expect( sut.Locator( "#drop-zone-1" ).Locator( ItemSelector ) ).ToHaveTextAsync( new[] { "Item 1", "Item 2", "Item 3", "Item 4" } );
        await Expect( sut.Locator( "#drop-zone-2" ).Locator( ItemSelector ) ).ToHaveTextAsync( new[] { "Item 5", "Item 6" } );
        await Expect( sut.Locator( "#drop-count" ) ).ToHaveTextAsync( "0" );

        if ( !animated )
        {
            await sut.Locator( "#toggle-animation" ).ClickAsync();
        }

        await Expect( sut.Locator( "#toggle-animation" ) ).ToHaveTextAsync( $"Animated: {animated}" );

        return sut;
    }

    private async Task StartDrag( ILocator sourceItem )
    {
        await sourceItem.HoverAsync();
        var source = await sourceItem.BoundingBoxAsync();

        await Page.Mouse.DownAsync();
        await Page.Mouse.MoveAsync( source.X + source.Width / 2 + 10, source.Y + source.Height / 2, new() { Steps = 5 } );
        await Expect( sourceItem ).ToBeHiddenAsync();
    }

    private async Task MoveOver( ILocator target )
    {
        await target.HoverAsync();

        // Entering a zone inserts a slot and moves its items. Follow the target after it settles.
        await target.HoverAsync();
        var bounds = await target.BoundingBoxAsync();

        // A second mouse move delivers dragover after the pointer enters the target.
        await Page.Mouse.MoveAsync( bounds.X + bounds.Width / 2 + 1, bounds.Y + bounds.Height / 2 );
    }

    private async Task DragFirstItemOverThird( ILocator zone )
    {
        var items = zone.Locator( ItemSelector );
        await StartDrag( items.Nth( 0 ) );
        await MoveOver( items.Nth( 2 ) );
        await Expect( SlotAfter( zone, "Item 3" ) ).ToHaveCountAsync( 1 );
    }

    private async Task CompleteDrop( ILocator zone, string precedingItem = null )
    {
        var placeholder = zone.Locator( ".drop-placeholder" );

        if ( precedingItem is not null )
        {
            placeholder = SlotAfter( zone, precedingItem );
        }

        var dropTarget = precedingItem is null
            ? zone
            : zone.Locator( ItemSelector ).Filter( new() { HasText = precedingItem } );

        // Follow the settled item and send dragover before releasing on the browser's current target.
        await MoveOver( dropTarget );
        await Expect( placeholder ).ToHaveCountAsync( 1 );
        await Expect( placeholder ).Not.ToHaveCSSAsync( "display", "none" );

        await Page.Mouse.UpAsync();
    }

    private static ILocator SlotAfter( ILocator zone, string item )
        => zone.Locator( $"{ItemSelector}:has-text('{item}') + .drop-placeholder" );
}