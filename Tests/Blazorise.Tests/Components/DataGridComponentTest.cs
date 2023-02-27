#region Using directives

using System.Collections.Generic;
using System.Linq;
using BasicTestApp.Client;
using Blazorise.DataGrid;
using Blazorise.Tests.Extensions;
using Blazorise.Tests.Helpers;
using Bunit;
using FluentAssertions;
using Xunit;
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

        // validate
        comp.FindAll( "tbody tr td:nth-child(2)" )
            .Select( x => x.TextContent )
            .Should()
            .ContainInOrder( expectedOrderedValues );
    }

    [Fact]
    public void SortByField_Should_CorrectlyReorderRows()
    {
        // setup
        var expectedOrderedValues = new[] { "3/4", "1/2", "1/4", "1/8"  };
        var comp = RenderComponent<DataGridComponent>();

        // test
        comp.Find( "thead tr th:nth-child(2)" )
            .Click(); // change sort order to descending
        
        // validate
        comp.FindAll( "tbody tr td:nth-child(2)" )
            .Select( x => x.TextContent )
            .Should()
            .ContainInOrder( expectedOrderedValues );
    }

    [Fact]
    public void SortByField_Should_RaiseSortChangedEvent()
    {
        // setup
        var sortChangedEventArgs = new List<DataGridSortChangedEventArgs>();
        var comp = RenderComponent<DataGridComponent>( parameters =>
        {
            parameters.Add( 
                parameterSelector: x => x.SortChanged, 
                callback: e => sortChangedEventArgs.Add( e ) );
        } );

        // test
        comp.Find( "thead tr th:nth-child(2)" )
            .Click(); // change sort order to descending
        
        // validate
        sortChangedEventArgs
            .Should().HaveCount( 1 )
            .And
            .OnlyContain( e => e.ColumnFieldName == "Fraction" &&
                               e.FieldName == "FractionValue" &&
                               e.SortDirection == SortDirection.Descending );
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