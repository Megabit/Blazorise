#region Using directives
using System.Linq;
using Blazorise.DataGrid;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridAggregateRowComponentTest : BunitContext
{
    public DataGridAggregateRowComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDataGrid();
    }

    [Theory]
    [InlineData( "Missing", "5", "10" )]
    [InlineData( "first", "5", "10" )]
    [InlineData( "First", "5", "Targeted total" )]
    [InlineData( null, "Targeted total", "10" )]
    [InlineData( "", "Targeted total", "10" )]
    public void AggregateRow_ResolvesColumnNamesBeforeFields( string columnName, string expectedAmount, string expectedFirst )
    {
        var comp = Render<DataGridNamedAggregateComponent>( parameters => parameters
            .Add( x => x.TargetColumnName, columnName ) );

        var cells = comp.FindAll( "tfoot tr:first-child td" );

        Assert.Equal( new[] { expectedAmount, "3", expectedFirst, "Custom total" }, cells.Select( cell => cell.TextContent.Trim() ) );
    }

    [Fact]
    public void AggregateRow_ResolvesColumnNamesWithinEachAggregateRow()
    {
        var comp = Render<DataGridNamedAggregateComponent>();

        var rows = comp.FindAll( "tfoot tr" );

        Assert.Equal( 2, rows.Count );
        Assert.Equal( new[] { "5", "3", "10", "Custom total" }, rows[0].QuerySelectorAll( "td" ).Select( cell => cell.TextContent.Trim() ) );
        Assert.Equal( new[] { "", "", "First summary", "Second summary" }, rows[1].QuerySelectorAll( "td" ).Select( cell => cell.TextContent.Trim() ) );
    }

    [Fact]
    public void AggregateRow_DisplaysCorrectAggregation()
    {
        // setup
        var comp = Render<DataGridAggregateRowComponent>();

        // test
        var aggregateRow = comp.Find( "thead tr:nth-child(2) td:nth-child(2)" );

        // validate
        Assert.Equal( "Total employees: 4", aggregateRow.InnerHtml );
    }

    [Fact]
    public void AggregateRow_RendersTop()
    {
        // setup
        var comp = Render<DataGridAggregateRowComponent>(
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
        var comp = Render<DataGridAggregateRowComponent>(
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
        var comp = Render<DataGridAggregateRowComponent>(
            parameters => parameters.Add( x => x.AggregateRowPosition, DataGridAggregateRowPosition.TopAndBottom ) );

        // test
        var AggregateRowTop = comp.Find( "thead tr:nth-child(2) td:nth-child(2)" );
        var aggregateRowBottom = comp.Find( "tfoot tr td:nth-child(2)" );

        // validate
        Assert.Equal( "Total employees: 4", AggregateRowTop.InnerHtml );
        Assert.Equal( "Total employees: 4", aggregateRowBottom.InnerHtml );
    }

}