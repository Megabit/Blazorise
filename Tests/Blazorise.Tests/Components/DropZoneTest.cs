using System;
using System.Linq;
using System.Threading.Tasks;
using BasicTestApp.Client;
using Blazorise.Tests.Helpers;
using Blazorise.Tests.TestServices;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Blazorise.Tests.Components;

public class DropZoneTest : TestContext
{
    public DropZoneTest()
    {
        var testServices = new TestServiceProvider( Services.AddSingleton<NavigationManager, TestNavigationManager>() );
        BlazoriseConfig.AddBootstrapProviders( testServices );
        BlazoriseConfig.JSInterop.AddDragDrop( this.JSInterop );
        BlazoriseConfig.JSInterop.AddUtilities( this.JSInterop );
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
        zone.DropNotAllowedClass.Should().BeNullOrEmpty();
        zone.OnlyZone.Should().BeFalse();
        zone.AllowReorder.Should().BeFalse();
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
    public async Task DropZone_Reorder_PlaceIntoEmptyZone()
    {
        var comp = RenderComponent<DropZoneReorderComponent>();

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

    [Fact]
    public async Task DropZone_Reorder_NoPreviewOnSameItem()
    {
        var comp = RenderComponent<DropZoneReorderComponent>();

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

        firstDropZone.Children.Should().HaveCount( 5 );
        firstDropZone.Children[0].TextContent.Should().BeNullOrEmpty();
        firstDropZone.Children[1].TextContent.Should().Be( "Item 1" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 2" );
        firstDropZone.Children[3].TextContent.Should().Be( "Item 3" );
        firstDropZone.Children[4].TextContent.Should().Be( "Item 4" );

        await firstDropZone.Children[3].DragEnterAsync( new DragEventArgs() );

        firstDropZone.Children.Should().HaveCount( 5 );
        firstDropZone.Children[0].TextContent.Should().BeNullOrEmpty();
        firstDropZone.Children[1].TextContent.Should().Be( "Item 1" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 2" );
        firstDropZone.Children[3].TextContent.Should().Be( "Item 3" );
        firstDropZone.Children[4].TextContent.Should().Be( "Item 4" );

        await firstDropZone.Children[2].DragEnterAsync( new DragEventArgs() );

        firstDropZone.Children.Should().HaveCount( 5 );
        firstDropZone.Children[0].TextContent.Should().BeNullOrEmpty();
        firstDropZone.Children[1].TextContent.Should().Be( "Item 1" );
        firstDropZone.Children[2].TextContent.Should().Be( "Item 2" );
        firstDropZone.Children[3].TextContent.Should().Be( "Item 3" );
        firstDropZone.Children[4].TextContent.Should().Be( "Item 4" );
    }

    [Fact]
    public async Task DropZone_Reorder_MoveWithinContainer_Down()
    {
        var comp = RenderComponent<DropZoneReorderComponent>();

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

        var thirdDropItem = firstDropZone.Children[3];
        thirdDropItem.TextContent.Should().Be( "Item 3" );
        await thirdDropItem.DragEnterAsync( new DragEventArgs() );

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
        var comp = RenderComponent<DropZoneReorderComponent>();

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
        await firstDropItem.DragEnterAsync( new DragEventArgs() );

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
        var comp = RenderComponent<DropZoneReorderComponent>();

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

        var lastDropItem = firstDropZone.Children[4];
        lastDropItem.TextContent.Should().Be( "Item 4" );
        await lastDropItem.DragEnterAsync( new DragEventArgs() );

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
        var comp = RenderComponent<DropZoneReorderComponent>();

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
        await firstDropItem.DragEnterAsync( new DragEventArgs() );

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
    public async Task DropZone_Reorder_MoveBetweenZones()
    {
        var comp = RenderComponent<DropZoneReorderComponent>();

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
        await firstItemInSecondDropZone.DragEnterAsync( new DragEventArgs() );

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
        await firstDropZone.Children[3].DragEnterAsync( new DragEventArgs() );
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
        await secondDropZone.Children[3].DragEnterAsync( new DragEventArgs() );
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
        await firstDropZone.Children[2].DragEnterAsync( new DragEventArgs() );
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
}