#region Using directives
using System.Threading.Tasks;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridMultiSelectComponentTest : TestContext
{
    public DataGridMultiSelectComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDataGrid();
    }

    [Fact]
    public async Task MultipleSelectAll_Should_Select_Unselect_AllRows()
    {
        var comp = RenderComponent<DataGridMultiSelectComponent>();
        await comp.Find( "input[type=checkbox]:first-child" ).ChangeAsync( "true" );
        var allCheckbox = comp.FindAll( "input[type=checkbox]" );
        foreach ( var checkbox in allCheckbox )
        {
            Assert.NotNull( checkbox.GetAttribute( "checked" ) );
        }

        await comp.Find( "input[type=checkbox]:first-child" ).ChangeAsync( "false" );
        allCheckbox = comp.FindAll( "input[type=checkbox]" );
        foreach ( var checkbox in allCheckbox )
        {
            Assert.Null( checkbox.GetAttribute( "checked" ) );
        }
    }
}