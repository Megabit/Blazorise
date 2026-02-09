#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Bootstrap;
using Blazorise.DataGrid;
using Blazorise.Tests.TestServices;
using Bunit;
using Xunit;
using Employee = BasicTestApp.Client.DataGridGroupingComponent.Employee;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridGroupingComponentTest : TestContext
{
    public DataGridGroupingComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDataGrid();
    }

    [Fact]
    public async Task GroupingChanged_Should_RaiseEvents_When_GroupingAddedAndRemoved()
    {
        // setup
        var groupingChanged = new List<DataGridGroupingChangedEventArgs<Employee>>();
        var comp = RenderComponent<BasicTestApp.Client.DataGridGroupingComponent>( parameters =>
        {
            parameters.Add(
                parameterSelector: x => x.GroupingChanged,
                callback: e => groupingChanged.Add( e ) );
        } );

        var dataGrid = comp.FindComponent<DataGrid<Employee>>();
        var nameColumn = dataGrid
            .FindComponents<DataGridColumn<Employee>>()
            .Single( x => x.Instance.Field == nameof( Employee.Name ) );

        // test
        await Task.Factory.StartNew(
            action: () => dataGrid.Instance.AddGroupColumn( nameColumn.Instance ),
            cancellationToken: CancellationToken.None,
            creationOptions: TaskCreationOptions.None,
            scheduler: new CurrentThreadTaskScheduler() );

        await Task.Factory.StartNew(
            action: () => dataGrid.Instance.RemoveGroupColumn( nameColumn.Instance ),
            cancellationToken: CancellationToken.None,
            creationOptions: TaskCreationOptions.None,
            scheduler: new CurrentThreadTaskScheduler() );

        // validate
        comp.WaitForAssertion( () =>
        {
            groupingChanged.Should().HaveCount( 2 );

            groupingChanged[0].ChangeType.Should().Be( DataGridGroupingChangeType.Added );
            groupingChanged[0].AddedColumn?.Field.Should().Be( nameof( Employee.Name ) );
            groupingChanged[0].RemovedColumn.Should().BeNull();
            groupingChanged[0].PreviousGroupedColumns.Should().BeEmpty();
            groupingChanged[0].GroupedColumns.Select( x => x.Field ).Should().Equal( nameof( Employee.Name ) );

            groupingChanged[1].ChangeType.Should().Be( DataGridGroupingChangeType.Removed );
            groupingChanged[1].RemovedColumn?.Field.Should().Be( nameof( Employee.Name ) );
            groupingChanged[1].AddedColumn.Should().BeNull();
            groupingChanged[1].PreviousGroupedColumns.Select( x => x.Field ).Should().Equal( nameof( Employee.Name ) );
            groupingChanged[1].GroupedColumns.Should().BeEmpty();
        }, TimeSpan.FromSeconds( 3 ) );
    }

    [Fact]
    public async Task GetState_Should_Include_ColumnGroupingStates()
    {
        // setup
        var comp = RenderComponent<BasicTestApp.Client.DataGridGroupingComponent>();
        var dataGrid = comp.FindComponent<DataGrid<Employee>>();
        var columns = dataGrid.FindComponents<DataGridColumn<Employee>>();
        var nameColumn = columns.Single( x => x.Instance.Field == nameof( Employee.Name ) );
        var departmentColumn = columns.Single( x => x.Instance.Field == nameof( Employee.Department ) );

        // test
        dataGrid.Instance.AddGroupColumn( nameColumn.Instance );
        dataGrid.Instance.AddGroupColumn( departmentColumn.Instance );

        var state = await dataGrid.Instance.GetState();

        // validate
        state.ColumnGroupingStates.Should().NotBeNull();
        state.ColumnGroupingStates.Select( x => x.FieldName )
            .Should().Equal( nameof( Employee.Name ), nameof( Employee.Department ) );
    }

    [Fact]
    public async Task LoadState_Should_Restore_ColumnGroupingStates()
    {
        // setup
        var comp = RenderComponent<BasicTestApp.Client.DataGridGroupingComponent>();
        var dataGrid = comp.FindComponent<DataGrid<Employee>>();
        var columns = dataGrid.FindComponents<DataGridColumn<Employee>>();
        var nameColumn = columns.Single( x => x.Instance.Field == nameof( Employee.Name ) );
        var state = new DataGridState<Employee>();
        state.AddGroupingState( nameof( Employee.Department ) );

        dataGrid.Instance.AddGroupColumn( nameColumn.Instance );

        // test
        await dataGrid.Instance.LoadState( state );
        var loadedState = await dataGrid.Instance.GetState();

        // validate
        loadedState.ColumnGroupingStates.Should().NotBeNull();
        loadedState.ColumnGroupingStates.Select( x => x.FieldName )
            .Should().Equal( nameof( Employee.Department ) );
    }

    [Fact]
    public async Task LoadState_Should_Not_Raise_GroupingChanged_Event()
    {
        // setup
        var groupingChanged = new List<DataGridGroupingChangedEventArgs<Employee>>();
        var comp = RenderComponent<BasicTestApp.Client.DataGridGroupingComponent>( parameters =>
        {
            parameters.Add(
                parameterSelector: x => x.GroupingChanged,
                callback: e => groupingChanged.Add( e ) );
        } );

        var dataGrid = comp.FindComponent<DataGrid<Employee>>();
        var state = new DataGridState<Employee>();
        state.AddGroupingState( nameof( Employee.Department ) );

        // test
        await dataGrid.Instance.LoadState( state );

        // validate
        comp.WaitForAssertion( () =>
        {
            groupingChanged.Should().BeEmpty();
        }, TimeSpan.FromSeconds( 3 ) );
    }
}