using System.Linq;
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.Tests.TestServices;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Blazorise.Tests.Components;

public class DropZoneTest : BunitContext
{
    public DropZoneTest()
    {
        Services.AddSingleton<NavigationManager, TestNavigationManager>();
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseDragDrop()
            .AddBlazoriseUtilities();
    }

    [Fact]
    public void DropContainer_Defaults()
    {
        var container = new DropContainer<object>();

        container.ApplyDropClassesOnDragStarted.Should().BeFalse();
        container.DropAllowed.Should().BeNull();
        container.DropAllowedClass.Should().BeNullOrEmpty();
        container.DisabledClass.Should().BeNullOrEmpty();
        container.DraggingClass.Should().BeNullOrEmpty();
        container.ItemDraggingClass.Should().BeNullOrEmpty();
        container.ItemDisabled.Should().BeNull();
        container.Items.Should().BeNull();
        container.ItemsFilter.Should().BeNull();
        container.PlaceholderTemplate.Should().BeNull();
        container.DropNotAllowedClass.Should().BeNullOrEmpty();
    }

    [Fact]
    public void DropZone_Defaults()
    {
        var zone = new DropZone<object>();

        zone.ApplyDropClassesOnDragStarted.Should().BeNull();
        zone.DropAllowed.Should().BeNull();
        zone.DropAllowedClass.Should().BeNullOrEmpty();
        zone.DisabledClass.Should().BeNullOrEmpty();
        zone.DraggingClass.Should().BeNullOrEmpty();
        zone.ItemDraggingClass.Should().BeNullOrEmpty();
        zone.ItemDisabled.Should().BeNull();
        zone.ItemsFilter.Should().BeNull();
        zone.PlaceholderTemplate.Should().BeNull();
        zone.DropNotAllowedClass.Should().BeNullOrEmpty();
        zone.OnlyZone.Should().BeFalse();
        zone.AllowReorder.Should().BeFalse();
        zone.Animated.Should().BeFalse();
        zone.ShowPlaceholder.Should().BeNull();
        zone.AnimationDuration.Should().Be( 200 );
    }

    [Fact]
    public void DropZone_ReorderOptions_UpdateAfterParameterChanges()
    {
        var comp = Render<DropZoneReorderComponent>();

        AssertReorderOptions( true, false, 200, 3 );

        comp.Render( parameters => parameters
            .Add( x => x.Animated, true )
            .Add( x => x.AnimationDuration, 400 ) );

        AssertReorderOptions( true, true, 400, 6 );

        comp.Render( parameters => parameters.Add( x => x.AnimationDuration, -1 ) );
        AssertReorderOptions( true, false, 0, 9 );

        comp.Render( parameters => parameters
            .Add( x => x.AnimationDuration, 200 )
            .Add( x => x.AllowReorder, false ) );
        AssertReorderOptions( false, false, 200, 12 );

        comp.Render( parameters => parameters
            .Add( x => x.AllowReorder, true )
            .Add( x => x.OnlyZone, true ) );
        AssertReorderOptions( false, false, 200, 15 );

        comp.Render( parameters => parameters.Add( x => x.OnlyZone, false ) );
        AssertReorderOptions( true, true, 200, 18 );

        comp.Render( parameters => parameters.Add( x => x.Animated, false ) );
        AssertReorderOptions( true, false, 200, 21 );

        void AssertReorderOptions( bool allowReorder, bool animated, int duration, int count )
        {
            comp.WaitForAssertion( () =>
            {
                var invocations = JSInterop.Invocations["updateOptions"];
                invocations.Should().HaveCount( count );

                foreach ( var invocation in invocations.TakeLast( 3 ) )
                {
                    var options = Assert.IsType<DragDropJSOptions>( invocation.Arguments[3] );
                    options.AllowReorder.Should().Be( allowReorder );
                    options.Animated.Should().Be( animated );
                    options.AnimationDuration.Should().Be( duration );
                }
            } );
        }
    }

    [Theory]
    [InlineData( true, 200 )]
    [InlineData( false, 200 )]
    [InlineData( true, 0 )]
    public async Task DropZone_Reorder_PreservesItemIdentityAndDropSemantics( bool animated, int duration )
    {
        var comp = Render<DropZoneReorderComponent>( parameters => parameters
            .Add( x => x.Animated, animated )
            .Add( x => x.AnimationDuration, duration ) );
        var zone = comp.FindComponents<DropZone<DropZoneReorderComponent.DropItem>>().First();
        var container = comp.FindComponent<DropContainer<DropZoneReorderComponent.DropItem>>().Instance;
        var selector = ".b-drop-zone-draggable:not(.draggable-preview-start)";
        var items = zone.FindAll( selector );
        var elementIds = items.ToDictionary( x => x.TextContent, x => x.Id );

        await items[0].DragStartAsync( new DragEventArgs() );

        items[0].GetAttribute( "data-reorder-source" ).Should().Be( "true" );
        var placeholder = zone.Find( "[data-reorder-placeholder='true']" );
        placeholder.PreviousElementSibling.Should().BeSameAs( items[0] );
        placeholder.ClassList.Should().NotContain( "d-none" );

        await zone.InvokeAsync( () => zone.Instance.OnReorderDragOver( 2 ) );
        await items[3].DragEnterAsync( new DragEventArgs() );

        zone.Find( "[data-reorder-placeholder='true']" ).PreviousElementSibling.Should().BeSameAs( items[2] );
        container.GetTransactionIndex().Should().Be( 2 );
        comp.Instance.IndexHistory.Should().BeEmpty();

        await zone.Find( ".b-drop-zone" ).DropAsync( new DragEventArgs() );

        zone.FindAll( selector ).Select( x => x.TextContent ).Should().Equal( "Item 2", "Item 3", "Item 1", "Item 4" );
        comp.Instance.IndexHistory.Should().Equal( 2 );

        foreach ( var item in zone.FindAll( selector ) )
        {
            item.Id.Should().Be( elementIds[item.TextContent] );
            item.HasAttribute( "data-reorder-source" ).Should().BeFalse();
        }

        // A delayed browser callback after the transaction ended must be harmless.
        await zone.InvokeAsync( () => zone.Instance.OnReorderDragOver( 1 ) );
        container.TransactionInProgress.Should().BeFalse();
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    public async Task DropZone_Reorder_CancelPreservesOrder( bool animated )
    {
        var comp = Render<DropZoneReorderComponent>( parameters => parameters.Add( x => x.Animated, animated ) );
        var zone = comp.FindComponents<DropZone<DropZoneReorderComponent.DropItem>>().First();
        var selector = ".b-drop-zone-draggable:not(.draggable-preview-start)";
        var source = zone.FindAll( selector )[0];

        await source.DragStartAsync( new DragEventArgs() );
        await zone.InvokeAsync( () => zone.Instance.OnReorderDragOver( 2 ) );
        await source.DragEndAsync( new DragEventArgs() );

        zone.FindAll( selector ).Select( x => x.TextContent ).Should().Equal( "Item 1", "Item 2", "Item 3", "Item 4" );
        comp.Instance.IndexHistory.Should().BeEmpty();
        zone.Find( ".b-drop-zone" ).GetAttribute( "data-transaction-active" ).Should().Be( "false" );
        source.HasAttribute( "data-reorder-source" ).Should().BeFalse();
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    public async Task DropZone_Placeholder_TracksZoneAndPreservesCustomTemplate( bool animated )
    {
        var comp = Render<DropZoneReorderComponent>( parameters => parameters
            .Add( x => x.Animated, animated )
            .Add( x => x.PlaceholderTemplate, item => builder => builder.AddContent( 0, $"Move {item.Name} here" ) ) );
        var source = comp.Find( ".dropzone-1 .b-drop-zone-draggable:not(.draggable-preview-start)" );

        await source.DragStartAsync( new DragEventArgs() );

        comp.Find( ".dropzone-1 [data-reorder-placeholder='true']" ).TextContent.Should().Be( "Move Item 1 here" );

        await comp.Find( ".dropzone-3" ).DragEnterAsync( new DragEventArgs() );

        source.GetAttribute( "data-reorder-source" ).Should().Be( "true" );
        comp.Find( ".dropzone-1 [data-reorder-placeholder='true']" ).ClassList.Should().Contain( "d-none" );
        var placeholder = comp.Find( ".dropzone-3 [data-reorder-placeholder='true']" );
        placeholder.TextContent.Should().Be( "Move Item 1 here" );
        placeholder.ClassList.Should().NotContain( "d-none" );

        await source.DragEndAsync( new DragEventArgs() );

        source.HasAttribute( "data-reorder-source" ).Should().BeFalse();
        comp.Find( ".dropzone-3 [data-reorder-placeholder='true']" ).ClassList.Should().Contain( "d-none" );
    }

    [Theory]
    [InlineData( true, null, false, false )]
    [InlineData( false, null, false, true )]
    [InlineData( true, null, true, true )]
    [InlineData( false, null, true, true )]
    [InlineData( true, true, false, true )]
    [InlineData( false, false, false, false )]
    [InlineData( true, false, true, false )]
    [InlineData( false, false, true, false )]
    public async Task DropZone_ShowPlaceholder_ResolvesVisibility( bool animated, bool? showPlaceholder, bool hasTemplate, bool expected )
    {
        RenderFragment<DropZoneReorderComponent.DropItem> template = hasTemplate
            ? item => builder => builder.AddContent( 0, $"Move {item.Name} here" )
            : null;
        var comp = Render<DropZoneReorderComponent>( parameters => parameters
            .Add( x => x.Animated, animated )
            .Add( x => x.ShowPlaceholder, showPlaceholder )
            .Add( x => x.PlaceholderTemplate, template ) );

        await comp.Find( ".dropzone-1 [data-index='0']" ).DragStartAsync( new DragEventArgs() );

        var placeholder = comp.Find( ".dropzone-1 [data-reorder-placeholder='true']" );
        placeholder.GetAttribute( "data-placeholder-visible" ).Should().Be( expected ? "true" : "false" );
        placeholder.HasAttribute( "inert" ).Should().Be( !expected );
        placeholder.ClassList.Should().NotContain( "d-none" );
        placeholder.TextContent.Should().Be( hasTemplate ? "Move Item 1 here" : string.Empty );
    }

    [Theory]
    [InlineData( true )]
    [InlineData( false )]
    public async Task DropZone_ShowPlaceholder_ChangesDuringReorderWithoutReplacingSlot( bool animated )
    {
        var comp = Render<DropZoneReorderComponent>( parameters => parameters
            .Add( x => x.Animated, animated )
            .Add( x => x.ShowPlaceholder, true )
            .Add( x => x.PlaceholderTemplate, item => builder => builder.AddContent( 0, $"Move {item.Name} here" ) ) );
        var zone = comp.FindComponents<DropZone<DropZoneReorderComponent.DropItem>>().First();

        await zone.Find( "[data-index='0']" ).DragStartAsync( new DragEventArgs() );
        await zone.InvokeAsync( () => zone.Instance.OnReorderDragOver( 2 ) );

        var placeholder = zone.Find( "[data-reorder-placeholder='true']" );
        comp.Render( parameters => parameters.Add( x => x.ShowPlaceholder, false ) );

        zone.Find( "[data-reorder-placeholder='true']" ).Should().BeSameAs( placeholder );
        placeholder.GetAttribute( "data-placeholder-visible" ).Should().Be( "false" );
        placeholder.TextContent.Should().Be( "Move Item 1 here" );
        placeholder.PreviousElementSibling.GetAttribute( "data-index" ).Should().Be( "2" );

        comp.Render( parameters => parameters.Add( x => x.ShowPlaceholder, true ) );

        placeholder.GetAttribute( "data-placeholder-visible" ).Should().Be( "true" );
        placeholder.HasAttribute( "inert" ).Should().BeFalse();

        await zone.Find( ".b-drop-zone" ).DropAsync( new DragEventArgs() );
        comp.Instance.IndexHistory.Should().Equal( 2 );
    }

    [Fact]
    public void DropItem_Defaults()
    {
        var item = new _Draggable<object>();

        item.Disabled.Should().BeFalse();
        item.DisabledClass.Should().BeNullOrEmpty();
        item.DraggingClass.Should().BeNullOrEmpty();
        item.ZoneName.Should().BeNullOrEmpty();
        item.Item.Should().BeNull();
        item.HideContent.Should().BeFalse();
    }

    [Fact]
    public void DropZone_DisposeWork()
    {
        var container = new DropZone<object>();

        var mockComponentDisposer = new Mock<IComponentDisposer>();
        container.ComponentDisposer = mockComponentDisposer.Object;

        container.Dispose();
    }

    [Fact]
    public async Task DropZone_PlaceholderTemplate_UsesContainerTemplateAndZoneOverride()
    {
        var comp = Render<DropZonePlaceholderComponent>();

        comp.FindAll( ".container-placeholder" ).Should().BeEmpty();
        comp.FindAll( ".zone-placeholder" ).Should().BeEmpty();

        var sourceItem = comp.Find( ".placeholder-source .b-drop-zone-draggable:not(.draggable-preview-start)" );

        await sourceItem.DragStartAsync( new DragEventArgs() );
        await comp.Find( ".placeholder-container" ).DragEnterAsync( new DragEventArgs() );

        comp.Find( ".placeholder-container .container-placeholder" ).TextContent.Should().Be( "Move Item 1 here" );

        await comp.Find( ".placeholder-zone" ).DragEnterAsync( new DragEventArgs() );

        comp.Find( ".placeholder-zone .zone-placeholder" ).TextContent.Should().Be( "Add Item 1 to this zone" );
    }

    [Fact]
    public async Task DropZone_Reorder_PlaceIntoEmptyZone()
    {
        var comp = Render<DropZoneReorderComponent>();

        comp.Find( ".b-drop-zone" );
        var firstDropZone = comp.Find( ".dropzone-1" );
        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        firstDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        firstDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 1" );
        firstDropZone.Children[3].TextContent.Should().Be( "Item 2" );
        firstDropZone.Children[4].TextContent.Should().Be( "Item 3" );
        firstDropZone.Children[5].TextContent.Should().Be( "Item 4" );

        var firstDropItem = firstDropZone.Children[2];

        await firstDropItem.DragStartAsync( new DragEventArgs() );

        var thirdDropZone = comp.Find( ".dropzone-3" );
        thirdDropZone.Children.Should().ContainSingle();
        thirdDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );

        await thirdDropZone.DragEnterAsync( new DragEventArgs() );

        thirdDropZone.Children.Should().ContainSingle();
        thirdDropZone.Children[0].ClassList.Should().Contain( "draggable-placeholder" ).And.NotContain( "d-none" );

        await thirdDropZone.DropAsync( new DragEventArgs() );

        thirdDropZone.Children.Should().HaveCount( 3 );
        thirdDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        thirdDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        thirdDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        thirdDropZone.Children[2].TextContent.Should().Be( "Item 1" );

        comp.Instance.IndexHistory.Distinct().Should().ContainSingle().And.Contain( 0 );
    }

    [Theory]
    [InlineData( 2 )]
    [InlineData( 1 )]
    public async Task DropZone_Reorder_PreservesSlotAtOriginalPosition( int index )
    {
        var comp = Render<DropZoneReorderComponent>();
        var zone = comp.FindComponents<DropZone<DropZoneReorderComponent.DropItem>>().First();
        var source = zone.Find( "[data-index='2']" );

        await source.DragStartAsync( new DragEventArgs() );
        await zone.InvokeAsync( () => zone.Instance.OnReorderDragOver( index ) );

        source.GetAttribute( "data-reorder-source" ).Should().Be( "true" );
        var placeholder = zone.Find( "[data-reorder-placeholder='true']" );
        placeholder.ClassList.Should().NotContain( "d-none" );
        placeholder.PreviousElementSibling.GetAttribute( "data-index" ).Should().Be( index.ToString() );

        await zone.Find( ".b-drop-zone" ).DropAsync( new DragEventArgs() );

        zone.FindAll( ".b-drop-zone-draggable:not(.draggable-preview-start)" )
            .Select( x => x.TextContent ).Should().Equal( "Item 1", "Item 2", "Item 3", "Item 4" );
        source.HasAttribute( "data-reorder-source" ).Should().BeFalse();
    }

    [Fact]
    public async Task DropZone_Reorder_MoveWithinContainer_Down()
    {
        var comp = Render<DropZoneReorderComponent>();

        comp.Find( ".b-drop-zone" );
        var firstDropZone = comp.Find( ".dropzone-1" );
        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        firstDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        firstDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 1" );

        var secondDropItem = firstDropZone.Children[3];
        secondDropItem.TextContent.Should().Be( "Item 2" );
        await secondDropItem.DragStartAsync( new DragEventArgs() );

        var thirdDropItem = firstDropZone.QuerySelector( "[data-index='2']" );
        thirdDropItem.TextContent.Should().Be( "Item 3" );
        await ReorderOverAsync( comp, "1", 2 );

        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[4].ClassList.Should().Contain( "draggable-placeholder" ).And.NotContain( "d-none" );

        await firstDropZone.DropAsync( new DragEventArgs() );
        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        firstDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        firstDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 1" );
        firstDropZone.Children[3].TextContent.Should().Be( "Item 3" );
        firstDropZone.Children[4].TextContent.Should().Be( "Item 2" );
        firstDropZone.Children[5].TextContent.Should().Be( "Item 4" );

        comp.Instance.IndexHistory.Distinct().Should().ContainSingle().And.Contain( 2 );
    }

    [Fact]
    public async Task DropZone_Reorder_MoveWithinContainer_Up()
    {
        var comp = Render<DropZoneReorderComponent>();

        comp.Find( ".b-drop-zone" );
        var firstDropZone = comp.Find( ".dropzone-1" );
        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        firstDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        firstDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 1" );

        var thirdDropItem = firstDropZone.Children[4];
        thirdDropItem.TextContent.Should().Be( "Item 3" );
        await thirdDropItem.DragStartAsync( new DragEventArgs() );

        var firstDropItem = firstDropZone.Children[1];
        firstDropItem.TextContent.Should().Be( "Item 1" );
        await ReorderOverAsync( comp, "1", 0 );

        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[2].ClassList.Should().Contain( "draggable-placeholder" ).And.NotContain( "d-none" );

        await firstDropZone.DropAsync( new DragEventArgs() );
        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        firstDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        firstDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 1" );
        firstDropZone.Children[3].TextContent.Should().Be( "Item 3" );
        firstDropZone.Children[4].TextContent.Should().Be( "Item 2" );
        firstDropZone.Children[5].TextContent.Should().Be( "Item 4" );

        comp.Instance.IndexHistory.Distinct().Should().ContainSingle().And.Contain( 1 );
    }

    [Fact]
    public async Task DropZone_Reorder_MoveWithinContainer_ToBottom()
    {
        var comp = Render<DropZoneReorderComponent>();

        comp.Find( ".b-drop-zone" );
        var firstDropZone = comp.Find( ".dropzone-1" );
        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        firstDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        firstDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 1" );

        var secondDropItem = firstDropZone.Children[3];
        secondDropItem.TextContent.Should().Be( "Item 2" );
        await secondDropItem.DragStartAsync( new DragEventArgs() );

        var lastDropItem = firstDropZone.QuerySelector( "[data-index='3']" );
        lastDropItem.TextContent.Should().Be( "Item 4" );
        await ReorderOverAsync( comp, "1", 3 );

        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[5].ClassList.Should().Contain( "draggable-placeholder" ).And.NotContain( "d-none" );

        await firstDropZone.DropAsync( new DragEventArgs() );
        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        firstDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        firstDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 1" );
        firstDropZone.Children[3].TextContent.Should().Be( "Item 3" );
        firstDropZone.Children[4].TextContent.Should().Be( "Item 4" );
        firstDropZone.Children[5].TextContent.Should().Be( "Item 2" );

        comp.Instance.IndexHistory.Distinct().Should().ContainSingle().And.Contain( 3 );

    }

    [Fact]
    public async Task DropZone_Reorder_MoveWithinContainer_Top()
    {
        var comp = Render<DropZoneReorderComponent>();

        comp.Find( ".b-drop-zone" );
        var firstDropZone = comp.Find( ".dropzone-1" );
        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        firstDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        firstDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 1" );

        var thirdDropItem = firstDropZone.Children[4];
        thirdDropItem.TextContent.Should().Be( "Item 3" );
        await thirdDropItem.DragStartAsync( new DragEventArgs() );

        var firstDropItem = firstDropZone.Children[0];
        firstDropItem.TextContent.Should().BeEmpty();
        await ReorderOverAsync( comp, "1", -1 );

        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[0].ClassList.Should().Contain( "draggable-placeholder" ).And.NotContain( "d-none" );

        await firstDropZone.DropAsync( new DragEventArgs() );
        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        firstDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        firstDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 3" );
        firstDropZone.Children[3].TextContent.Should().Be( "Item 1" );
        firstDropZone.Children[4].TextContent.Should().Be( "Item 2" );
        firstDropZone.Children[5].TextContent.Should().Be( "Item 4" );

        comp.Instance.IndexHistory.Distinct().Should().ContainSingle().And.Contain( 0 );
    }

    [Fact]
    public async Task DropZone_Reorder_MoveBetweenZones_BetweenItemsAfterMovingBelow()
    {
        var comp = Render<DropZoneReorderComponent>();

        var firstDropZone = comp.Find( ".dropzone-1" );
        var secondDropZone = comp.Find( ".dropzone-2" );
        var secondDropItemInFirstZone = firstDropZone.Children.Single( x => x.TextContent == "Item 2" );

        await secondDropItemInFirstZone.DragStartAsync( new DragEventArgs() );
        await secondDropZone.DragEnterAsync( new DragEventArgs() );

        await ReorderOverAsync( comp, "2", 1 );

        await ReorderOverAsync( comp, "2", 0 );

        secondDropZone.Children[2].ClassList.Should().Contain( "draggable-placeholder" ).And.NotContain( "d-none" );

        await secondDropZone.DropAsync( new DragEventArgs() );

        secondDropZone.Children[2].TextContent.Should().Be( "Item 5" );
        secondDropZone.Children[3].TextContent.Should().Be( "Item 2" );
        secondDropZone.Children[4].TextContent.Should().Be( "Item 6" );
        comp.Instance.IndexHistory.Should().ContainSingle().And.Contain( 1 );
    }

    [Fact]
    public async Task DropZone_Reorder_MoveBetweenZones()
    {
        var comp = Render<DropZoneReorderComponent>();

        comp.Find( ".b-drop-zone" );
        var firstDropZone = comp.Find( ".dropzone-1" );
        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        firstDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        firstDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 1" );

        var secondDropZone = comp.Find( ".dropzone-2" );

        var secondDropItemInFirstZone = firstDropZone.Children[3];
        secondDropItemInFirstZone.TextContent.Should().Be( "Item 2" );
        await secondDropItemInFirstZone.DragStartAsync( new DragEventArgs() );

        await secondDropZone.DragEnterAsync( new DragEventArgs() );

        var firstItemInSecondDropZone = secondDropZone.Children[3];
        firstItemInSecondDropZone.TextContent.Should().Be( "Item 6" );
        await ReorderOverAsync( comp, "2", 1 );

        secondDropZone.Children.Should().HaveCount( 4 );
        secondDropZone.Children[3].ClassList.Should().Contain( "draggable-placeholder" ).And.NotContain( "d-none" );

        await secondDropZone.DropAsync( new DragEventArgs() );
        firstDropZone.Children.Should().HaveCount( 5 );
        firstDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        firstDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        firstDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 1" );
        firstDropZone.Children[3].TextContent.Should().Be( "Item 3" );
        firstDropZone.Children[4].TextContent.Should().Be( "Item 4" );

        secondDropZone.Children.Should().HaveCount( 5 );
        secondDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        secondDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        secondDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        secondDropZone.Children[2].TextContent.Should().Be( "Item 5" );
        secondDropZone.Children[3].TextContent.Should().Be( "Item 6" );
        secondDropZone.Children[4].TextContent.Should().Be( "Item 2" );

        await secondDropZone.Children[3].DragStartAsync( new DragEventArgs() );
        await firstDropZone.DragEnterAsync( new DragEventArgs() );
        await ReorderOverAsync( comp, "1", 1 );
        await firstDropZone.DropAsync( new DragEventArgs() );

        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        firstDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        firstDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 1" );
        firstDropZone.Children[3].TextContent.Should().Be( "Item 3" );
        firstDropZone.Children[4].TextContent.Should().Be( "Item 6" );
        firstDropZone.Children[5].TextContent.Should().Be( "Item 4" );

        secondDropZone.Children.Should().HaveCount( 4 );
        secondDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        secondDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        secondDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        secondDropZone.Children[2].TextContent.Should().Be( "Item 5" );
        secondDropZone.Children[3].TextContent.Should().Be( "Item 2" );

        await firstDropZone.Children[4].DragStartAsync( new DragEventArgs() );
        await secondDropZone.DragEnterAsync( new DragEventArgs() );
        await ReorderOverAsync( comp, "2", 1 );
        await secondDropZone.DropAsync( new DragEventArgs() );

        firstDropZone.Children.Should().HaveCount( 5 );
        firstDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        firstDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        firstDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 1" );
        firstDropZone.Children[3].TextContent.Should().Be( "Item 3" );
        firstDropZone.Children[4].TextContent.Should().Be( "Item 4" );

        secondDropZone.Children.Should().HaveCount( 5 );
        secondDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        secondDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        secondDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        secondDropZone.Children[2].TextContent.Should().Be( "Item 5" );
        secondDropZone.Children[3].TextContent.Should().Be( "Item 2" );
        secondDropZone.Children[4].TextContent.Should().Be( "Item 6" );

        await secondDropZone.Children[3].DragStartAsync( new DragEventArgs() );
        await firstDropZone.DragEnterAsync( new DragEventArgs() );
        await ReorderOverAsync( comp, "1", 0 );
        await firstDropZone.DropAsync( new DragEventArgs() );

        firstDropZone.Children.Should().HaveCount( 6 );
        firstDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        firstDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        firstDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 1" );
        firstDropZone.Children[3].TextContent.Should().Be( "Item 2" );
        firstDropZone.Children[4].TextContent.Should().Be( "Item 3" );
        firstDropZone.Children[5].TextContent.Should().Be( "Item 4" );

        secondDropZone.Children.Should().HaveCount( 4 );
        secondDropZone.Children[0].ClassList.Should().Contain( new[] { "d-none", "draggable-placeholder" } );
        secondDropZone.Children[1].ClassList.Should().Contain( "draggable-preview-start" );
        secondDropZone.Children[1].GetAttribute( "draggable" ).Should().Be( "false" );
        secondDropZone.Children[2].TextContent.Should().Be( "Item 5" );
        secondDropZone.Children[3].TextContent.Should().Be( "Item 6" );

        comp.Instance.IndexHistory.Should().ContainInOrder( new[] { 2, 2, 2, 1 } );
    }

    [Fact]
    public async Task DropZone_SourceZone_MatchesTransactionSourceZone()
    {
        // Arrange
        DraggableDroppedEventArgs<object> returnedArgs = null;

        DropContainer<object> sut = new DropContainer<object>()
        {
            ItemDropped = new EventCallback<DraggableDroppedEventArgs<object>>( null, DropEvent )
        };

        Task DropEvent( DraggableDroppedEventArgs<object> e )
        {
            returnedArgs = e;

            return Task.CompletedTask;
        }

        sut.StartTransaction( new object(), "source_zone_name", 0, () => Task.CompletedTask, () => Task.CompletedTask );

        // Act
        await sut.CommitTransaction( "destination_zone_name", false );

        // Assert
        returnedArgs.Should().NotBe( null );
        returnedArgs.SourceDropZoneName.Should().Be( "source_zone_name" );
        returnedArgs.DropZoneName.Should().Be( "destination_zone_name" );
    }

    private static Task ReorderOverAsync( IRenderedComponent<DropZoneReorderComponent> comp, string zoneName, int index )
    {
        var zone = comp.FindComponents<DropZone<DropZoneReorderComponent.DropItem>>()
            .Single( x => x.Instance.Name == zoneName );

        return zone.InvokeAsync( () => zone.Instance.OnReorderDragOver( index ) );
    }
}