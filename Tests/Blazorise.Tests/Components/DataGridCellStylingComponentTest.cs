#region Using directives
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridCellStylingComponentTest : TestContext
{
    public DataGridCellStylingComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDataGrid();
    }

    [Fact]
    public void CellStyling_Should_Apply_Background_Class_To_TableCells()
    {
        var comp = RenderComponent<DataGridCellStylingComponent>();

        var inactiveRowCells = comp.FindAll( "tbody tr:nth-child(2) td" );

        Assert.NotEmpty( inactiveRowCells );

        foreach ( var cell in inactiveRowCells )
        {
            Assert.Contains( "text-muted", cell.ClassList );
            Assert.Contains( "bg-secondary", cell.ClassList );
        }
    }
}