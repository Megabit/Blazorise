#region Using directives
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Blazorise.Bootstrap;
using Blazorise.DataGrid;
using Blazorise.Tests.TestServices;
using Bunit;
using Xunit;
using Employee = BasicTestApp.Client.DataGridFilterStateComponent.Employee;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridFilterStateComponentTest : BunitContext
{
    public DataGridFilterStateComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDataGrid();
    }

    [Fact]
    public async Task LoadState_And_GetState_Should_RoundTrip_ColumnFilterMethod()
    {
        // setup
        var comp = Render<BasicTestApp.Client.DataGridFilterStateComponent>();
        var dataGrid = comp.FindComponent<DataGrid<Employee>>();
        var state = new DataGridState<Employee>();
        state.AddFilterState( nameof( Employee.Name ), "oh", DataGridColumnFilterMethod.StartsWith );

        // test
        await dataGrid.Instance.LoadState( state );
        var loadedState = await dataGrid.Instance.GetState();

        // validate
        comp.WaitForAssertion( () =>
        {
            comp.FindAll( "tbody tr.table-row-selectable" ).Should().BeEmpty();
        }, TimeSpan.FromSeconds( 3 ) );

        var filterStates = loadedState.ColumnFilterStates;
        filterStates.Should().ContainSingle();

        var filterState = filterStates!.Single();
        filterState.FieldName.Should().Be( nameof( Employee.Name ) );
        filterState.SearchValue.Should().Be( "oh" );
        filterState.FilterMethod.Should().Be( DataGridColumnFilterMethod.StartsWith );
    }

    [Fact]
    public void DataGridColumnFilterState_Should_Deserialize_Legacy_Json_Without_FilterMethod()
    {
        // setup
        var json = "{\"FieldName\":\"Name\",\"SearchValue\":\"oh\"}";

        // test
        var filterState = JsonSerializer.Deserialize<DataGridColumnFilterState<Employee>>( json );

        // validate
        filterState.Should().NotBeNull();
        var actualFilterState = filterState!;
        actualFilterState.FieldName.Should().Be( nameof( Employee.Name ) );
        actualFilterState.SearchValue.Should().NotBeNull();
        actualFilterState.SearchValue.ToString().Should().Be( "oh" );
        actualFilterState.FilterMethod.Should().BeNull();
    }
}