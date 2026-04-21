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
}