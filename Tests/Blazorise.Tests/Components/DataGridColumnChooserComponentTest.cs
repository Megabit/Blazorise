#region Using directives
using System;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.DataGrid;
using Bunit;
using Xunit;
using Employee = BasicTestApp.Client.DataGridColumnChooserComponent.Employee;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridColumnChooserComponentTest : TestContext
{
    public DataGridColumnChooserComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDataGrid();
    }

    [Fact]
    public async Task Column_With_Displaying_False_Should_BeHidden_Initially_And_Toggleable_From_ColumnChooser()
    {
        // setup
        var comp = RenderComponent<DataGridColumnChooserComponent>();
        var dataGrid = comp.FindComponent<DataGrid<Employee>>();
        var salaryColumn = dataGrid
            .FindComponents<DataGridColumn<Employee>>()
            .Single( x => x.Instance.Field == nameof( Employee.Salary ) );

        // validate initial state
        salaryColumn.Instance.Displaying.Should().BeFalse();
        comp.FindAll( "thead tr th" )
            .Select( x => x.TextContent.Trim() )
            .Should()
            .NotContain( "Salary" );
        comp.FindAll( "a.dropdown-item" )
            .Select( x => x.TextContent.Trim() )
            .Should()
            .Contain( "Salary" );

        // test
        await comp.FindAll( "a.dropdown-item" )
            .Single( x => x.TextContent.Contains( "Salary", StringComparison.Ordinal ) )
            .ClickAsync();

        // validate
        comp.WaitForAssertion( () =>
        {
            salaryColumn.Instance.Displaying.Should().BeTrue();
            comp.FindAll( "thead tr th" )
                .Select( x => x.TextContent.Trim() )
                .Should()
                .Contain( "Salary" );
        }, TimeSpan.FromSeconds( 3 ) );
    }

    [Fact]
    public async Task LoadState_Without_ColumnDisplayingStates_Should_Reset_To_Displaying_Parameter_Default()
    {
        // setup
        var comp = RenderComponent<DataGridColumnChooserComponent>();
        var dataGrid = comp.FindComponent<DataGrid<Employee>>();
        var salaryColumn = dataGrid
            .FindComponents<DataGridColumn<Employee>>()
            .Single( x => x.Instance.Field == nameof( Employee.Salary ) );

        // test
        await salaryColumn.Instance.SetDisplaying( true );
        await dataGrid.Instance.LoadState( new DataGridState<Employee>() );

        // validate
        comp.WaitForAssertion( () =>
        {
            salaryColumn.Instance.Displaying.Should().BeFalse();
            comp.FindAll( "thead tr th" )
                .Select( x => x.TextContent.Trim() )
                .Should()
                .NotContain( "Salary" );
        }, TimeSpan.FromSeconds( 3 ) );
    }
}