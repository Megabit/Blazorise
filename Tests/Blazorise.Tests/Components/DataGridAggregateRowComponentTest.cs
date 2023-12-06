#region Using directives
using Blazorise.DataGrid;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridAggregateRowComponentTest : TestContext
{
    public DataGridAggregateRowComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDataGrid();
    }

    [Fact]
    public void AggregateRow_DisplaysCorrectAggregation()
    {
        // setup
        var comp = RenderComponent<DataGridAggregateRowComponent>();

        // test
        var aggregateRow = comp.Find( "thead tr:nth-child(2) td:nth-child(2)" );

        // validate
        Assert.Equal( "Total employees: 4", aggregateRow.InnerHtml );
    }

    [Fact]
    public void AggregateRow_RendersTop()
    {
        // setup
        var comp = RenderComponent<DataGridAggregateRowComponent>(
            parameters => parameters.Add( x => x.AggregateRowPosition, DataGridAggregateRowPosition.Top ) );

        // test
        var aggregateRow = comp.Find( "thead tr:nth-child(2) td:nth-child(2)" );

        // validate
        Assert.Equal( "Total employees: 4", aggregateRow.InnerHtml );
    }

    [Fact]
    public void AggregateRow_RendersBottom()
    {
        // setup
        var comp = RenderComponent<DataGridAggregateRowComponent>(
            parameters => parameters.Add( x => x.AggregateRowPosition, DataGridAggregateRowPosition.Bottom ) );

        // test
        var aggregateRowBottom = comp.Find( "tfoot tr td:nth-child(2)" );

        // validate
        Assert.Throws<Bunit.ElementNotFoundException>( () => comp.Find( "thead tr:nth-child(2) td:nth-child(2)" ) );
        Assert.Equal( "Total employees: 4", aggregateRowBottom.InnerHtml );
    }

    [Fact]
    public void AggregateRow_RendersTopAndBottom()
    {
        // setup
        var comp = RenderComponent<DataGridAggregateRowComponent>(
            parameters => parameters.Add( x => x.AggregateRowPosition, DataGridAggregateRowPosition.TopAndBottom ) );

        // test
        var AggregateRowTop = comp.Find( "thead tr:nth-child(2) td:nth-child(2)" );
        var aggregateRowBottom = comp.Find( "tfoot tr td:nth-child(2)" );

        // validate
        Assert.Equal( "Total employees: 4", AggregateRowTop.InnerHtml );
        Assert.Equal( "Total employees: 4", aggregateRowBottom.InnerHtml );
    }

}