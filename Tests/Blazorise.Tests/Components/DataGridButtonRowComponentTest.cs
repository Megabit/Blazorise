#region Using directives
using System.Threading.Tasks;
using Blazorise.DataGrid;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridButtonRowComponentTest : TestContext
{
    public DataGridButtonRowComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDataGrid();
    }

    [Theory]
    [InlineData( DataGridEditMode.Form )]
    [InlineData( DataGridEditMode.Inline )]
    [InlineData( DataGridEditMode.Popup )]
    public async Task New_Should_AddNewItem( DataGridEditMode editMode )
    {
        // setup
        var comp = RenderComponent<DataGridButtonRowComponent>( parameters =>
            parameters.Add( x => x.DataGridEditMode, editMode ) );

        var startingDataCount = comp.Instance.InMemoryData.Count;

        // test
        await comp.Click( "#btnNew" );
        await comp.Click( "#btnSave" );

        var currentDataCount = comp.Instance.InMemoryData.Count;

        // validate
        var expectedResult = startingDataCount + 1;
        comp.WaitForAssertion( () => Assert.Equal( expectedResult, comp.Instance.InMemoryData.Count ), System.TimeSpan.FromSeconds( 3 ) );
    }

    [Theory]
    [InlineData( DataGridEditMode.Form )]
    [InlineData( DataGridEditMode.Inline )]
    [InlineData( DataGridEditMode.Popup )]
    public async Task Edit_Should_UpdateItem( DataGridEditMode editMode )
    {
        // setup
        var updatedName = "RaulFromEdit";
        var comp = RenderComponent<DataGridButtonRowComponent>( parameters =>
            parameters.Add( x => x.DataGridEditMode, editMode ) );

        // test
        await comp.Click( "tr.table-row-selectable" );
        await comp.Click( "#btnEdit" );

        await comp.Input( "input", updatedName,
            ( firstInput ) => firstInput.SetAttribute( "value", updatedName ) );

        await comp.Click( "#btnSave" );

        var currentName = comp.Instance.InMemoryData[0].Name;

        // validate
        comp.WaitForAssertion( () => Assert.Contains( comp.Instance.InMemoryData, x => x.Name == updatedName ), System.TimeSpan.FromSeconds( 3 ) );
    }

    [Theory]
    [InlineData( DataGridEditMode.Form )]
    [InlineData( DataGridEditMode.Inline )]
    [InlineData( DataGridEditMode.Popup )]
    public async Task Delete_Should_DeleteItem( DataGridEditMode editMode )
    {
        // setup
        var comp = RenderComponent<DataGridButtonRowComponent>( parameters =>
            parameters.Add( x => x.DataGridEditMode, editMode ) );
        var startingDataCount = comp.Instance.InMemoryData.Count;

        // test
        await comp.Click( "tr.table-row-selectable" );
        await comp.Click( "#btnDelete" );

        var currentDataCount = comp.Instance.InMemoryData.Count;

        // validate
        var expectedResult = startingDataCount - 1;
        comp.WaitForAssertion( () => Assert.Equal( expectedResult, currentDataCount ), System.TimeSpan.FromSeconds( 3 ) );
    }
}