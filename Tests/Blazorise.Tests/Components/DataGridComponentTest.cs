#region Using directives
using System.Collections.Generic;
using BasicTestApp.Client;
using Blazorise.DataGrid;
using Blazorise.Tests.Extensions;
using Blazorise.Tests.Helpers;
using Bunit;
using Xunit;
using static BasicTestApp.Client.DataGridComponent;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridComponentTest : TestContext
{
    public DataGridComponentTest()
    {
        BlazoriseConfig.AddBootstrapProviders( Services );
        BlazoriseConfig.JSInterop.AddDataGrid( this.JSInterop );
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
        var expectedOrderedValues = new[] { "1/8", "1/4", "1/2", "3/4" };

        // test
        var comp = RenderComponent<DataGridComponent>();
        var rows = comp.FindAll( "tbody tr td:nth-child(2)" );

        // validate
        var count = 0;
        foreach ( var item in rows )
        {
            Assert.Equal( item.TextContent, expectedOrderedValues[count] );
            count++;
        }
    }

    [Theory]
    [InlineData( DataGridEditMode.Form )]
    [InlineData( DataGridEditMode.Inline )]
    [InlineData( DataGridEditMode.Popup )]
    public void New_Should_AddNewItem( DataGridEditMode editMode )
    {
        // setup
        var comp = RenderComponent<DataGridComponent>( parameters =>
            parameters.Add( x => x.DataGridEditMode, editMode ) );
        var startingDataCount = comp.Instance.Data.Count;

        // test
        comp.Click( "#btnNew" );
        comp.Click( "#btnSave" );
        var currentDataCount = comp.Instance.Data.Count;

        // validate
        var expectedResult = startingDataCount + 1;
        comp.WaitForAssertion( () => Assert.Equal( expectedResult, comp.Instance.Data.Count ), System.TimeSpan.FromSeconds( 3 ) );
    }

    [Theory]
    [InlineData( DataGridEditMode.Form )]
    [InlineData( DataGridEditMode.Inline )]
    [InlineData( DataGridEditMode.Popup )]
    public void New_Should_Invoke_RowInsert_Callbacks( DataGridEditMode editMode )
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

        var startingDataCount = comp.Instance.Data.Count;

        // test
        comp.Click( "#btnNew" );
        comp.Input( "input", newName, ( firstInput ) => firstInput.SetAttribute( "value", newName ) );
        comp.Click( "#btnSave" );
        var currentDataCount = comp.Instance.Data.Count;

        // validate
        var expectedCountResult = startingDataCount + 1;
        comp.WaitForAssertion( () => Assert.Equal( expectedCountResult, comp.Instance.Data.Count ), System.TimeSpan.FromSeconds( 3 ) );

        Assert.Equal( newName, comp.Instance.Data[expectedCountResult - 1].Name );

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
    public void Edit_Should_Invoke_RowUpdate_Callbacks( DataGridEditMode editMode )
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
        comp.Click( "#btnEdit" );

        comp.Input( "input", updatedName,
            ( firstInput ) => firstInput.SetAttribute( "value", updatedName ) );

        comp.Click( "#btnSave" );
        comp.SetParametersAndRender( x => x.Add( param => param.Data, data ) );

        var currentName = comp.Find( "tbody tr.table-row-selectable:first-child td:nth-child(3)" ).TextContent;

        // validate
        comp.WaitForAssertion( () => Assert.Contains( comp.Instance.Data, x => x.Name == updatedName ), System.TimeSpan.FromSeconds( 3 ) );
        comp.WaitForAssertion( () => Assert.Equal( updatedName, currentName ), System.TimeSpan.FromSeconds( 3 ) );

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
    public void Remove_Should_Invoke_RowRemove_Callbacks( DataGridEditMode editMode )
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

        var startingDataCount = comp.Instance.Data.Count;

        // test
        comp.Click( "#btnDelete" );

        var currentDataCount = comp.Instance.Data.Count;

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
    public void Edit_Should_UpdateItem( DataGridEditMode editMode )
    {
        // setup
        var updatedName = "RaulFromEdit";
        var comp = RenderComponent<DataGridComponent>( parameters =>
            parameters.Add( x => x.DataGridEditMode, editMode ) );

        // test
        comp.Find( "#btnEdit" ).Click();

        comp.Input( "input", updatedName,
            ( firstInput ) => firstInput.SetAttribute( "value", updatedName ) );

        comp.Click( "#btnSave" );

        var currentName = comp.Instance.Data[0].Name;

        // validate
        comp.WaitForAssertion( () => Assert.Contains( comp.Instance.Data, x => x.Name == updatedName ), System.TimeSpan.FromSeconds( 3 ) );
    }

    [Theory]
    [InlineData( DataGridEditMode.Form )]
    [InlineData( DataGridEditMode.Inline )]
    [InlineData( DataGridEditMode.Popup )]
    public void Delete_Should_DeleteItem( DataGridEditMode editMode )
    {
        // setup
        var comp = RenderComponent<DataGridComponent>( parameters =>
            parameters.Add( x => x.DataGridEditMode, editMode ) );
        var startingDataCount = comp.Instance.Data.Count;

        // test
        comp.Click( "#btnDelete" );

        var currentDataCount = comp.Instance.Data.Count;

        // validate
        var expectedResult = startingDataCount - 1;
        comp.WaitForAssertion( () => Assert.Equal( expectedResult, currentDataCount ), System.TimeSpan.FromSeconds( 3 ) );
    }

}