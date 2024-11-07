#region Using directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Bootstrap;
using Blazorise.DataGrid;
using Blazorise.Tests.TestServices;
using Bunit;
using Xunit;
using Employee = BasicTestApp.Client.DataGridComponent.Employee;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridComponentTest : TestContext
{
    public DataGridComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDataGrid();
    }


    [Fact]
    public async Task OnSelection_Should_Render_EmptyTemplate()
    {
        //Arrange
        var numRenders = 0;

        //Act
        var comp = RenderComponent<DataGridComponent>();

        comp.OnAfterRender += ( sender, args ) =>
        {
            numRenders++;
        };

        var rowsFraction = comp.FindAll( "tbody tr.table-row-selectable" );
        await rowsFraction[0].ClickAsync( new() { Detail = 1 } );
        //Assert
        Assert.Equal( 1, numRenders );
    }

    [Fact]
    public void NoData_Should_Render_EmptyTemplate()
    {
        // setup
        var expectedEmptyTemplate = "No Records...";

        // test
        var comp = RenderComponent<DataGridComponent>( parameters =>
            parameters.Add( x => x.Data, null ) );

        // validate
        var emptyTemplateValue = comp.Find( "tbody tr td" ).TextContent;

        Assert.Contains( expectedEmptyTemplate, emptyTemplateValue );
    }

    [Fact]
    public void SortByField_Should_CorrectlySortRows()
    {
        // setup
        var expectedOrderedValues = new[] { "1/8", "1/4", "1/3", "1/2", "3/4" };

        // test
        var comp = RenderComponent<DataGridComponent>();

        // validate
        comp.FindAll( "tbody tr td:nth-child(2)" )
            .Select( x => x.TextContent )
            .Should()
            .BeEquivalentTo( expectedOrderedValues );
    }

    [Fact]
    public async Task SortByField_Should_CorrectlyReorderRows()
    {
        // setup
        var expectedOrderedValues = new[] { "3/4", "1/2", "1/3", "1/4", "1/8" };
        var comp = RenderComponent<DataGridComponent>();

        // test
        await comp.Find( "thead tr th:nth-child(2)" )
            .ClickAsync(); // change sort order to descending

        // validate
        comp.FindAll( "tbody tr td:nth-child(2)" )
            .Select( x => x.TextContent )
            .Should()
            .BeEquivalentTo( expectedOrderedValues );
    }

    [Fact]
    public async Task SortByField_Should_RaiseSortChangedEvent()
    {
        // setup
        var sortChanged = new List<DataGridSortChangedEventArgs>();
        var comp = RenderComponent<DataGridComponent>( parameters =>
        {
            parameters.Add(
                parameterSelector: x => x.SortChanged,
                callback: e => sortChanged.Add( e ) );
        } );

        // test
        await Task.Factory.StartNew(
            // Note: event handling was implemented using Fire & Forget (async call without await keyword!)
            () => comp.Find( "thead tr th:nth-child(2)" ).ClickAsync(), // change sort order to descending
            cancellationToken: CancellationToken.None,
            creationOptions: TaskCreationOptions.None,
            scheduler: new CurrentThreadTaskScheduler()
        );

        // validate
        comp.WaitForAssertion( () =>
        {
            sortChanged
            .Should().HaveCount( 1 )
            .And
            .OnlyContain( e => e.ColumnFieldName == "Fraction" &&
                               e.FieldName == "FractionValue" &&
                               e.SortDirection == SortDirection.Descending );
        } );

    }

    [Fact]
    public async Task SortByField_Should_RaiseSortingChangedEvent()
    {
        // setup
        var sortingChanged = new List<DataGridSortChangedEventArgs>();
        var comp = RenderComponent<DataGridComponent>( parameters =>
        {
            parameters.Add(
                parameterSelector: x => x.SortChanged,
                callback: e => sortingChanged.Add( e ) );
        } );

        // test
        await Task.Factory.StartNew(
            // Note: event handling was implemented using Fire & Forget (async call without await keyword!)
            action: () => comp.Find( "thead tr th:nth-child(2)" ).ClickAsync(), // change sort order to descending
            cancellationToken: CancellationToken.None,
            creationOptions: TaskCreationOptions.None,
            scheduler: new CurrentThreadTaskScheduler()
        );

        // validate
        comp.WaitForAssertion( () =>
        {
            sortingChanged
            .Should().HaveCount( 1 )
            .And
            .ContainEquivalentOf( new DataGridSortChangedEventArgs(
                 fieldName: nameof( Employee.FractionValue ),
                 columnFieldName: nameof( Employee.Fraction ),
                 sortDirection: SortDirection.Descending
            ) );
        } );
    }

    [Fact]
    public async Task Sort_Should_CorrectlyReorderWhenUsingFieldName()
    {
        // setup
        var expectedOrderedValues = new[] { "3/4", "1/2", "1/3", "1/4", "1/8" };
        var comp = RenderComponent<DataGridComponent>();
        var dataGrid = comp.FindComponent<DataGrid<Employee>>();

        // test
        await Task.Factory.StartNew(
            // Note: event handling was implemented using Fire & Forget (async call without await keyword!)
            action: () => dataGrid.Instance.Sort( nameof( Employee.Fraction ), SortDirection.Descending )
                .GetAwaiter().GetResult(),
            cancellationToken: CancellationToken.None,
            creationOptions: TaskCreationOptions.None,
            scheduler: new CurrentThreadTaskScheduler()
        );

        // validate
        comp.WaitForAssertion( () =>
        {
            comp.FindAll( "tbody tr td:nth-child(2)" )
                .Select( x => x.TextContent )
                .Should()
                .BeEquivalentTo( expectedOrderedValues );
        } );
    }

    [Fact]
    public async Task ApplySorting_Multiple_Should_ReorderCorrectlyWhenSortingWithMultipleColumns()
    {
        // setup
        var expectedOrderedValues = new[] { "3/4", "1/3", "1/2", "1/8", "1/4" };
        var comp = RenderComponent<DataGridComponent>();
        var dataGrid = comp.FindComponent<DataGrid<Employee>>();

        // test
        await dataGrid.Instance.ApplySorting(
            new DataGridSortColumnInfo(
                field: nameof( Employee.Name ),
                sortDirection: SortDirection.Ascending ),
            new DataGridSortColumnInfo(
                field: nameof( Employee.Fraction ),
                sortDirection: SortDirection.Ascending )
        );

        // validate
        comp.WaitForAssertion( () =>
        {
            comp.FindAll( "tbody tr td:nth-child(2)" )
                .Select( x => x.TextContent )
                .Should()
                .BeEquivalentTo( expectedOrderedValues );
        } );
    }

    [Fact]
    public async Task ApplySorting_Multiple_Should_RaiseSortingChangedEvent()
    {
        // setup
        var sortingChanged = new List<DataGridSortChangedEventArgs>();
        var comp = RenderComponent<DataGridComponent>( parameters =>
        {
            parameters.Add(
                parameterSelector: x => x.SortChanged,
                callback: e => sortingChanged.Add( e ) );
        } );

        var dataGrid = comp.FindComponent<DataGrid<Employee>>();

        // test
        await dataGrid.Instance.ApplySorting(
            new DataGridSortColumnInfo(
                field: nameof( Employee.Name ),
                sortDirection: SortDirection.Ascending ),
            new DataGridSortColumnInfo(
                field: nameof( Employee.Fraction ),
                sortDirection: SortDirection.Descending )
        );

        // validate
        comp.WaitForAssertion( () =>
        {
            sortingChanged
                .Should().HaveCount( 2 )
                .And
                .ContainEquivalentOf( new DataGridSortChangedEventArgs(
                 fieldName: nameof( Employee.FractionValue ),
                 columnFieldName: nameof( Employee.Fraction ),
                 sortDirection: SortDirection.Descending
                ) );

            sortingChanged.Should().ContainEquivalentOf( new DataGridSortChangedEventArgs(
                             fieldName: nameof( Employee.Name ),
                             columnFieldName: nameof( Employee.Name ),
                             sortDirection: SortDirection.Ascending
                        ) );
        } );
    }

    [Fact]
    public async Task ApplySorting_Multiple_Should_RestoreTheDefaultOrderIfNoSortColumnsAreSpecified()
    {
        // setup
        var expectedOrderedValues = new[] { "1/2", "1/4", "1/8", "3/4", "1/3" };
        var comp = RenderComponent<DataGridComponent>();
        var dataGrid = comp.FindComponent<DataGrid<Employee>>();

        // test
        await dataGrid.Instance.ApplySorting( Array.Empty<DataGridSortColumnInfo>() );

        // validate
        comp.WaitForAssertion( () =>
        {
            comp.FindAll( "tbody tr td:nth-child(2)" )
                .Select( x => x.TextContent )
                .Should()
                .BeEquivalentTo( expectedOrderedValues );
        } );
    }

    [Fact]
    public async Task ApplySorting_Single_Should_ReorderCorrectlyWhenSortingASingleColumn()
    {
        // setup
        var expectedOrderedValues = new[] { "3/4", "1/2", "1/3", "1/8", "1/4" };
        var comp = RenderComponent<DataGridComponent>( p => p.Add( x => x.SortMode, DataGridSortMode.Single ) );
        var dataGrid = comp.FindComponent<DataGrid<Employee>>();

        // test
        await dataGrid.Instance.ApplySorting(
            new DataGridSortColumnInfo(
                field: nameof( Employee.Name ),
                sortDirection: SortDirection.Ascending )
        );

        // validate
        comp.WaitForAssertion( () =>
        {
            comp.FindAll( "tbody tr td:nth-child(2)" )
                .Select( x => x.TextContent )
                .Should()
                .BeEquivalentTo( expectedOrderedValues );
        } );
    }

    [Fact]
    public async Task ApplySorting_Single_Should_RestoreTheDefaultOrderIfNoSortColumnIsSpecified()
    {
        // setup
        var expectedOrderedValues = new[] { "1/2", "1/4", "1/8", "3/4", "1/3" };
        var comp = RenderComponent<DataGridComponent>( p => p.Add( x => x.SortMode, DataGridSortMode.Single ) );
        var dataGrid = comp.FindComponent<DataGrid<Employee>>();

        // test
        await dataGrid.Instance.ApplySorting( Array.Empty<DataGridSortColumnInfo>() );

        // validate
        comp.WaitForAssertion( () =>
        {
            comp.FindAll( "tbody tr td:nth-child(2)" )
                .Select( x => x.TextContent )
                .Should()
                .BeEquivalentTo( expectedOrderedValues );
        } );
    }

    [Fact]
    public async Task ApplySorting_Single_Should_RaiseSortingChangedEvent()
    {
        // setup
        var sortingChanged = new List<DataGridSortChangedEventArgs>();
        var comp = RenderComponent<DataGridComponent>( parameters =>
        {
            parameters.Add( x => x.SortMode, DataGridSortMode.Single );
            parameters.Add(
                parameterSelector: x => x.SortChanged,
                callback: e => sortingChanged.Add( e ) );
        } );

        var dataGrid = comp.FindComponent<DataGrid<Employee>>();

        // test
        await dataGrid.Instance.ApplySorting(
            new DataGridSortColumnInfo(
                field: nameof( Employee.Name ),
                sortDirection: SortDirection.Descending )
        );

        // validate
        comp.WaitForAssertion( () =>
        {
            sortingChanged
                .Should().HaveCount( 1 )
                .And
                .ContainEquivalentOf( new DataGridSortChangedEventArgs(
                     fieldName: nameof( Employee.Name ),
                     columnFieldName: nameof( Employee.Name ),
                     sortDirection: SortDirection.Descending
                ) );
        } );
    }

    [Fact]
    public async Task RemoveColumn_Should_RaiseSortingChanged()
    {
        // setup
        var sortingChanged = new List<DataGridSortChangedEventArgs>();
        var comp = RenderComponent<DataGridComponent>( parameters =>
        {
            parameters.Add(
                parameterSelector: x => x.SortChanged,
                callback: e => sortingChanged.Add( e ) );
        } );
        var dataGrid = comp.FindComponent<DataGrid<Employee>>();
        var column = dataGrid
            .FindComponents<DataGridColumn<Employee>>()
            .Single( x => x.Instance.Field == nameof( Employee.Fraction ) );

        // test
        await Task.Factory.StartNew(
            // Note: event handling was implemented using Fire & Forget (async call without await keyword!)
            function: () => dataGrid.Instance.RemoveColumn( column.Instance ),
            cancellationToken: CancellationToken.None,
            creationOptions: TaskCreationOptions.None,
            scheduler: new CurrentThreadTaskScheduler()
        );

        // validate
        comp.WaitForAssertion( () =>
        {
            sortingChanged
                .Should().HaveCount( 1 )
                .And
                .ContainEquivalentOf( new DataGridSortChangedEventArgs(
                     fieldName: nameof( Employee.FractionValue ),
                     columnFieldName: nameof( Employee.Fraction ),
                     sortDirection: SortDirection.Default
                ) );
        } );
    }

    [Theory]
    [InlineData( DataGridEditMode.Form )]
    [InlineData( DataGridEditMode.Inline )]
    [InlineData( DataGridEditMode.Popup )]
    public async Task New_Should_AddNewItem( DataGridEditMode editMode )
    {
        // setup
        var comp = RenderComponent<DataGridComponent>( parameters =>
            parameters.Add( x => x.DataGridEditMode, editMode ) );
        var startingDataCount = comp.Instance.Data.Count();

        // test
        await comp.Click( "#btnNew" );
        await comp.Click( "#btnSave" );

        var currentDataCount = comp.Instance.Data.Count();

        // validate
        var expectedResult = startingDataCount + 1;
        comp.WaitForAssertion( () => Assert.Equal( expectedResult, comp.Instance.Data.Count() ), System.TimeSpan.FromSeconds( 3 ) );
    }

    [Theory]
    [InlineData( DataGridEditMode.Form )]
    [InlineData( DataGridEditMode.Inline )]
    [InlineData( DataGridEditMode.Popup )]
    public async Task New_Should_Invoke_RowInsert_Callbacks( DataGridEditMode editMode )
    {
        // setup
        var newName = "RaulFromNew";
        var data = new List<Employee>();
        var rowInsertingCount = 0;
        var rowInsertedCount = 0;

        Employee EmployeeInsertingOld = null;
        Employee EmployeeInsertingNew = null;
        Employee EmployeeInsertedOld = null;
        Employee EmployeeInsertedNew = null;

        var comp = RenderComponent<DataGridComponent>( parameters =>
        {
            //Avoid mutating the original Item
            parameters.Add( x => x.UseInternalEditing, false );
            parameters.Add( x => x.Data, data );
            parameters.Add( x => x.DataGridEditMode, editMode );
            parameters.Add( x => x.RowInserted, ( e ) =>
            {
                rowInsertedCount++;
                EmployeeInsertedOld = e.OldItem;
                EmployeeInsertedNew = e.NewItem;

                data.Add( e.NewItem );
            } );
            parameters.Add( x => x.RowInserting, ( e ) =>
            {
                rowInsertingCount++;
                EmployeeInsertingOld = e.OldItem;
                EmployeeInsertingNew = e.NewItem;

                //We are asserting here, because the reference will be mutated with the newName value when inserted.
                Assert.Equal( default, EmployeeInsertingOld.Name );
            } );
        } );

        var startingDataCount = comp.Instance.Data.Count();

        // test
        await comp.Click( "#btnNew" );
        await comp.Input( "input", newName, ( firstInput ) => firstInput.SetAttribute( "value", newName ) );
        await comp.Click( "#btnSave" );
        var currentDataCount = comp.Instance.Data.Count();

        // validate
        var expectedCountResult = startingDataCount + 1;
        comp.WaitForAssertion( () => Assert.Equal( expectedCountResult, comp.Instance.Data.Count() ), System.TimeSpan.FromSeconds( 3 ) );

        Assert.Equal( newName, comp.Instance.Data.ElementAt( expectedCountResult - 1 ).Name );

        Assert.False( object.ReferenceEquals( EmployeeInsertingOld, EmployeeInsertingNew ) );
        Assert.False( object.ReferenceEquals( EmployeeInsertedOld, EmployeeInsertedNew ) );


        Assert.Equal( newName, EmployeeInsertingNew.Name );

        Assert.Equal( newName, EmployeeInsertedOld.Name );
        Assert.Equal( newName, EmployeeInsertedNew.Name );

        Assert.Equal( 1, rowInsertingCount );
        Assert.Equal( 1, rowInsertedCount );
    }

    [Theory]
    [InlineData( DataGridEditMode.Form )]
    [InlineData( DataGridEditMode.Inline )]
    [InlineData( DataGridEditMode.Popup )]
    public async Task Edit_Should_Invoke_RowUpdate_Callbacks( DataGridEditMode editMode )
    {
        // setup
        var updatedName = "RaulFromEdit";
        var data = new List<Employee>() {
            new Employee()
            {
                Name = "Paul"
            },
            new Employee()
            {
                Name = "John"
            },
        };

        var rowUpdatingCount = 0;
        var rowUpdatedCount = 0;

        Employee EmployeeUpdatingOld = null;
        Employee EmployeeUpdatingNew = null;
        Employee EmployeeUpdatedOld = null;
        Employee EmployeeUpdatedNew = null;

        var comp = RenderComponent<DataGridComponent>( parameters =>
        {
            //Avoid mutating the original Item
            parameters.Add( x => x.UseInternalEditing, false );
            parameters.Add( x => x.Data, data );
            parameters.Add( x => x.DataGridEditMode, editMode );
            parameters.Add( x => x.RowUpdated, ( e ) =>
            {
                rowUpdatedCount++;
                EmployeeUpdatedOld = e.OldItem;
                EmployeeUpdatedNew = e.NewItem;

                var idx = data.FindIndex( x => x == e.OldItem );
                data[idx] = e.NewItem;
            } );
            parameters.Add( x => x.RowUpdating, ( e ) =>
            {
                rowUpdatingCount++;
                EmployeeUpdatingOld = e.OldItem;
                EmployeeUpdatingNew = e.NewItem;
            } );
        } );

        // test
        await comp.Click( "#btnEdit" );

        await comp.Input( "input", updatedName,
            ( firstInput ) => firstInput.SetAttribute( "value", updatedName ) );

        await comp.Click( "#btnSave" );

        var dataGridRef = comp.FindComponent<DataGrid<Employee>>();
        await dataGridRef.Instance.Reload();


        // validate
        comp.WaitForAssertion( () => Assert.Contains( comp.Instance.Data, x => x.Name == updatedName ), System.TimeSpan.FromSeconds( 3 ) );

        var currentName = comp.Find( "tbody tr.table-row-selectable:first-child td:nth-child(3)" ).TextContent;
        Assert.Equal( updatedName, currentName );

        Assert.False( object.ReferenceEquals( EmployeeUpdatingOld, EmployeeUpdatingNew ) );
        Assert.False( object.ReferenceEquals( EmployeeUpdatedOld, EmployeeUpdatedNew ) );

        Assert.Equal( "Paul", EmployeeUpdatingOld.Name );
        Assert.Equal( updatedName, EmployeeUpdatingNew.Name );

        Assert.Equal( "Paul", EmployeeUpdatedOld.Name );
        Assert.Equal( updatedName, EmployeeUpdatedNew.Name );

        Assert.Equal( 1, rowUpdatingCount );
        Assert.Equal( 1, rowUpdatedCount );
    }

    [Theory]
    [InlineData( DataGridEditMode.Form )]
    [InlineData( DataGridEditMode.Inline )]
    [InlineData( DataGridEditMode.Popup )]
    public async Task Edit_Should_Update_ItemAndUI_WhenObservable( DataGridEditMode editMode )
    {
        // setup
        var updatedName = "RaulFromEdit";
        var data = new ObservableCollection<Employee>() {
        new Employee()
        {
            Name = "Paul"
        },
        new Employee()
        {
            Name = "John"
        },
    };

        var rowUpdatingCount = 0;
        var rowUpdatedCount = 0;

        Employee EmployeeUpdatingOld = null;
        Employee EmployeeUpdatingNew = null;
        Employee EmployeeUpdatedOld = null;
        Employee EmployeeUpdatedNew = null;

        var comp = RenderComponent<DataGridComponent>( parameters =>
        {
            //Avoid mutating the original Item
            parameters.Add( x => x.UseInternalEditing, false );
            parameters.Add( x => x.Data, data );
            parameters.Add( x => x.DataGridEditMode, editMode );
            parameters.Add( x => x.RowUpdated, ( e ) =>
            {
                rowUpdatedCount++;
                EmployeeUpdatedOld = e.OldItem;
                EmployeeUpdatedNew = e.NewItem;

                var idx = data.IndexOf( e.OldItem );
                data[idx] = e.NewItem;
            } );
            parameters.Add( x => x.RowUpdating, ( e ) =>
            {
                rowUpdatingCount++;
                EmployeeUpdatingOld = e.OldItem;
                EmployeeUpdatingNew = e.NewItem;
            } );
        } );

        // test
        await comp.Click( "#btnEdit" );

        await comp.Input( "input", updatedName,
            ( firstInput ) => firstInput.SetAttribute( "value", updatedName ) );

        await comp.Click( "#btnSave" );



        // validate
        comp.WaitForAssertion( () => Assert.Contains( comp.Instance.Data, x => x.Name == updatedName ), System.TimeSpan.FromSeconds( 3 ) );

        var currentName = comp.Find( "tbody tr.table-row-selectable:first-child td:nth-child(3)" ).TextContent;
        Assert.Equal( updatedName, currentName );

        Assert.False( object.ReferenceEquals( EmployeeUpdatingOld, EmployeeUpdatingNew ) );
        Assert.False( object.ReferenceEquals( EmployeeUpdatedOld, EmployeeUpdatedNew ) );

        Assert.Equal( "Paul", EmployeeUpdatingOld.Name );
        Assert.Equal( updatedName, EmployeeUpdatingNew.Name );

        Assert.Equal( "Paul", EmployeeUpdatedOld.Name );
        Assert.Equal( updatedName, EmployeeUpdatedNew.Name );

        Assert.Equal( 1, rowUpdatingCount );
        Assert.Equal( 1, rowUpdatedCount );
    }

    [Theory]
    [InlineData( DataGridEditMode.Form )]
    [InlineData( DataGridEditMode.Inline )]
    [InlineData( DataGridEditMode.Popup )]
    public async Task Remove_Should_Invoke_RowRemove_Callbacks( DataGridEditMode editMode )
    {
        // setup

        var rowRemovingCount = 0;
        var rowRemovedCount = 0;

        Employee EmployeeRemovingOld = null;
        Employee EmployeeRemovingNew = null;
        Employee EmployeeRemoved = null;

        var comp = RenderComponent<DataGridComponent>( parameters =>
        {
            parameters.Add( x => x.DataGridEditMode, editMode );
            parameters.Add( x => x.RowRemoved, ( e ) =>
            {
                rowRemovedCount++;
                EmployeeRemoved = e;
            } );
            parameters.Add( x => x.RowRemoving, ( e ) =>
            {
                rowRemovingCount++;
                EmployeeRemovingOld = e.OldItem;
                EmployeeRemovingNew = e.NewItem;
            } );
        } );

        var startingDataCount = comp.Instance.Data.Count();

        // test
        await comp.Click( "#btnDelete" );

        var currentDataCount = comp.Instance.Data.Count();

        // validate
        var expectedResult = startingDataCount - 1;
        comp.WaitForAssertion( () => Assert.Equal( expectedResult, currentDataCount ), System.TimeSpan.FromSeconds( 3 ) );

        Assert.DoesNotContain( EmployeeRemoved, comp.Instance.Data );
        Assert.True( object.ReferenceEquals( EmployeeRemovingNew, EmployeeRemovingOld ) );

        Assert.Equal( 1, rowRemovingCount );
        Assert.Equal( 1, rowRemovedCount );
    }

    [Theory]
    [InlineData( DataGridEditMode.Form )]
    [InlineData( DataGridEditMode.Inline )]
    [InlineData( DataGridEditMode.Popup )]
    public async Task Edit_Should_UpdateItem( DataGridEditMode editMode )
    {
        // setup
        var updatedName = "RaulFromEdit";
        var comp = RenderComponent<DataGridComponent>( parameters =>
            parameters.Add( x => x.DataGridEditMode, editMode ) );

        // test
        await comp.Find( "#btnEdit" ).ClickAsync();

        await comp.Input( "input", updatedName,
            ( firstInput ) => firstInput.SetAttribute( "value", updatedName ) );

        await comp.Click( "#btnSave" );

        var currentName = comp.Instance.Data.ElementAt( 0 ).Name;

        // validate
        comp.WaitForAssertion( () => Assert.Contains( comp.Instance.Data, x => x.Name == updatedName ), System.TimeSpan.FromSeconds( 3 ) );
    }

    [Theory]
    [InlineData( DataGridEditMode.Form )]
    [InlineData( DataGridEditMode.Inline )]
    [InlineData( DataGridEditMode.Popup )]
    public async Task Delete_Should_DeleteItem( DataGridEditMode editMode )
    {
        // setup
        var comp = RenderComponent<DataGridComponent>( parameters =>
            parameters.Add( x => x.DataGridEditMode, editMode ) );
        var startingDataCount = comp.Instance.Data.Count();

        // test
        await comp.Click( "#btnDelete" );

        var currentDataCount = comp.Instance.Data.Count();

        // validate
        var expectedResult = startingDataCount - 1;
        comp.WaitForAssertion( () => Assert.Equal( expectedResult, currentDataCount ), System.TimeSpan.FromSeconds( 3 ) );
    }
}