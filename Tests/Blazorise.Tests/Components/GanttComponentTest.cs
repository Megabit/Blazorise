#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BasicTestApp.Client;
using Blazorise.Bootstrap;
using Blazorise.Gantt;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class GanttComponentTest : BunitContext
{
    public GanttComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseGantt();
    }

    [Fact]
    public void Sort_Should_Request_ReadData_WithUpdatedSort()
    {
        var readDataEvents = new List<GanttReadDataEventArgs<GanttComponent.TaskItem>>();

        var comp = Render<GanttComponent>( parameters =>
        {
            parameters.Add( x => x.UseInternalEditing, false );
            parameters.Add( x => x.ReadData, e => readDataEvents.Add( e ) );
        } );

        comp.WaitForAssertion( () => Assert.NotEmpty( readDataEvents ) );

        var initialEventCount = readDataEvents.Count;

        comp.Find( "#sort-start" ).Click();

        comp.WaitForAssertion( () => Assert.True( readDataEvents.Count > initialEventCount ) );

        var ascendingSort = readDataEvents[^1];

        Assert.Equal( "Start", ascendingSort.SortField );
        Assert.Equal( "Start", ascendingSort.SortColumnField );
        Assert.Equal( SortDirection.Ascending, ascendingSort.SortDirection );

        comp.Find( "#sort-start" ).Click();

        comp.WaitForAssertion( () => Assert.True( readDataEvents.Count > initialEventCount + 1 ) );

        var descendingSort = readDataEvents[^1];

        Assert.Equal( "Start", descendingSort.SortField );
        Assert.Equal( "Start", descendingSort.SortColumnField );
        Assert.Equal( SortDirection.Descending, descendingSort.SortDirection );
    }

    [Theory]
    [InlineData( false )]
    [InlineData( true )]
    public async Task SearchMode_Should_PreserveMatchingSubtrees_AndAncestorPaths( bool hierarchicalData )
    {
        var items = new List<SearchTask>
        {
            new() { Id = "1", Title = "Project" },
            new() { Id = "2", ParentId = "1", Title = "Planning" },
            new() { Id = "3", ParentId = "2", Title = "Research" },
            new() { Id = "4", ParentId = "1", Title = "Delivery" },
            new() { Id = "5", Title = "Unrelated" },
        };

        foreach ( var item in items )
        {
            item.Start = DateTime.Today;
            item.End = DateTime.Today.AddDays( 1 );
        }

        items[0].Items.AddRange( new[] { items[1], items[3] } );
        items[1].Items.Add( items[2] );

        var comp = Render<Gantt<SearchTask>>( parameters =>
        {
            parameters.Add( x => x.Data, hierarchicalData ? new[] { items[0], items[4] } : items );
            parameters.Add( x => x.HierarchicalData, hierarchicalData );
        } );

        await comp.InvokeAsync( () => comp.Instance.CollapseAll() );

        comp.Render( parameters => parameters.Add( x => x.SearchText, "planning" ) );

        Assert.Collection( comp.FindAll( ".b-gantt-tree-row" ),
            row => Assert.Contains( "Project", row.TextContent ),
            row => Assert.Contains( "Planning", row.TextContent ) );

        comp.Render( parameters => parameters.Add( x => x.SearchMode, GanttSearchMode.Subtree ) );

        Assert.Collection( comp.FindAll( ".b-gantt-tree-row" ),
            row => Assert.Contains( "Project", row.TextContent ),
            row => Assert.Contains( "Planning", row.TextContent ),
            row => Assert.Contains( "Research", row.TextContent ) );

        comp.Render( parameters => parameters.Add( x => x.SearchText, "Project" ) );

        Assert.Equal( 4, comp.FindAll( ".b-gantt-tree-row" ).Count );

        comp.Render( parameters => parameters.Add( x => x.SearchText, "Research" ) );

        Assert.Collection( comp.FindAll( ".b-gantt-tree-row" ),
            row => Assert.Contains( "Project", row.TextContent ),
            row => Assert.Contains( "Planning", row.TextContent ),
            row => Assert.Contains( "Research", row.TextContent ) );

        comp.Render( parameters => parameters.Add( x => x.SearchText, "Missing" ) );

        Assert.Empty( comp.FindAll( ".b-gantt-tree-row" ) );

        comp.Render( parameters => parameters.Add( x => x.SearchText, " " ) );

        Assert.Collection( comp.FindAll( ".b-gantt-tree-row" ),
            row => Assert.Contains( "Project", row.TextContent ),
            row => Assert.Contains( "Unrelated", row.TextContent ) );
    }

    [Theory]
    [InlineData( " 2 ", "2" )]
    [InlineData( "", "" )]
    [InlineData( " ", "" )]
    [InlineData( null, "" )]
    public void CustomFilter_Should_ReplaceDefaultMatching_AndReceiveTrimmedSearchText( string searchText, string expectedSearchText )
    {
        var comp = Render<Gantt<GanttComponent.TaskItem>>( parameters =>
        {
            parameters.Add( x => x.Data, GanttComponent.CreateTasks() );
            parameters.Add( x => x.SearchText, searchText );
            parameters.Add( x => x.CustomFilter, ( item, term ) =>
            {
                Assert.Equal( expectedSearchText, term );

                return item.Id == "2";
            } );
        } );

        Assert.Collection( comp.FindAll( ".b-gantt-tree-row" ),
            row => Assert.Contains( "Project launch", row.TextContent ),
            row => Assert.Contains( "Planning", row.TextContent ) );

        comp.Render( parameters => parameters.Add( x => x.CustomFilter, ( item, term ) => item.Id == "1" ) );

        Assert.Single( comp.FindAll( ".b-gantt-tree-row" ) );

        comp.Render( parameters => parameters.Add( x => x.SearchMode, GanttSearchMode.Subtree ) );

        Assert.Equal( 3, comp.FindAll( ".b-gantt-tree-row" ).Count );

        comp.Render( parameters => parameters.Add( x => x.CustomFilter, ( item, term ) => false ) );

        Assert.Empty( comp.FindAll( ".b-gantt-tree-row" ) );
    }

    [Fact]
    public void UseInternalEditing_False_Should_Invoke_NewItemClicked()
    {
        GanttCommandContext<GanttComponent.TaskItem> capturedContext = null;

        var comp = Render<GanttComponent>( parameters =>
        {
            parameters.Add( x => x.UseInternalEditing, false );
            parameters.Add( x => x.NewItemClicked, e => capturedContext = e );
        } );

        comp.Find( "#btnNew" ).Click();

        comp.WaitForAssertion( () => Assert.NotNull( capturedContext ) );

        Assert.Equal( GanttCommandType.New, capturedContext.CommandType );
        Assert.NotNull( capturedContext.Item );
        Assert.Null( capturedContext.ParentItem );
        Assert.True( capturedContext.Item.End > capturedContext.Item.Start );
    }

    [Fact]
    public void UseInternalEditing_False_Should_Invoke_AddChildItemClicked()
    {
        GanttCommandContext<GanttComponent.TaskItem> capturedContext = null;

        var comp = Render<GanttComponent>( parameters =>
        {
            parameters.Add( x => x.UseInternalEditing, false );
            parameters.Add( x => x.AddChildItemClicked, e => capturedContext = e );
        } );

        comp.Find( "#btnAddChild-1" ).Click();

        comp.WaitForAssertion( () => Assert.NotNull( capturedContext ) );

        Assert.Equal( GanttCommandType.AddChild, capturedContext.CommandType );
        Assert.NotNull( capturedContext.Item );
        Assert.NotNull( capturedContext.ParentItem );
        Assert.Equal( "1", capturedContext.ParentItem.Id );
        Assert.True( capturedContext.Item.End > capturedContext.Item.Start );
    }

    [Fact]
    public void UseInternalEditing_False_Should_Invoke_EditItemClicked()
    {
        GanttItemClickedEventArgs<GanttComponent.TaskItem> capturedEventArgs = null;

        var comp = Render<GanttComponent>( parameters =>
        {
            parameters.Add( x => x.UseInternalEditing, false );
            parameters.Add( x => x.EditItemClicked, e => capturedEventArgs = e );
        } );

        comp.Find( "#btnEdit-2" ).Click();

        comp.WaitForAssertion( () => Assert.NotNull( capturedEventArgs ) );

        Assert.Equal( "2", capturedEventArgs.Item.Id );
        Assert.Equal( "Planning", capturedEventArgs.Item.Title );
    }

    [Fact]
    public void UseInternalEditing_False_Should_Invoke_DeleteItemClicked_WithoutMutatingData()
    {
        var data = GanttComponent.CreateTasks();
        GanttItemClickedEventArgs<GanttComponent.TaskItem> capturedEventArgs = null;

        var comp = Render<GanttComponent>( parameters =>
        {
            parameters.Add( x => x.UseInternalEditing, false );
            parameters.Add( x => x.Data, data );
            parameters.Add( x => x.DeleteItemClicked, e => capturedEventArgs = e );
        } );

        comp.Find( "#btnDelete-2" ).Click();

        comp.WaitForAssertion( () => Assert.NotNull( capturedEventArgs ) );

        Assert.Equal( "2", capturedEventArgs.Item.Id );
        Assert.Equal( 3, data.Count );
    }

    public class SearchTask : GanttComponent.TaskItem
    {
        public List<SearchTask> Items { get; set; } = new();
    }
}