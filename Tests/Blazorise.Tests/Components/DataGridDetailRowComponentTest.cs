#region Using directives
using System.Threading.Tasks;
using Bunit;
using Xunit;
#endregion

namespace Blazorise.Tests.Components;

public class DataGridDetailRowComponentTest : TestContext
{
    public DataGridDetailRowComponentTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop.AddBlazoriseDataGrid();
    }

    [Fact]
    public void DetailRow_DetailRowStartsVisible_True_ShouldRender()
    {
        // setup
        var comp = RenderComponent<DataGridDetailRowComponent>();

        // test
        var rows = comp.FindAll( "#lblFraction" );
        var rowsFraction = comp.FindAll( "tbody tr.table-row-selectable td:first-child" );

        // validate
        for ( int i = 0; i < rowsFraction.Count; i++ )
            Assert.Equal( rowsFraction[i].TextContent, rows[i].TextContent );
    }

    [Fact]
    public void DetailRow_DetailRowStartsVisible_False_ShouldNotRender()
    {
        // setup
        var comp = RenderComponent<DataGridDetailRowComponent>(
            parameters => parameters.Add( x => x.DetailRowStartsVisible, false ) );

        // test
        var rows = comp.FindAll( "#lblFraction" );

        // validate
        Assert.Empty( rows );
    }

    [Fact]
    public async Task DetailRow_OnClick_ShouldHide()
    {
        // setup
        var comp = RenderComponent<DataGridDetailRowComponent>();

        // test
        var rowsFraction = comp.FindAll( "tbody tr.table-row-selectable" );
        foreach ( var row in rowsFraction )
        {
            await row.ClickAsync( new Microsoft.AspNetCore.Components.Web.MouseEventArgs { Detail = 1 } );
        }

        var rows = comp.FindAll( "#lblFraction" );

        // validate
        Assert.Empty( rows );
    }

    [Fact]
    public async Task DetailRow_OnToggleDetailRowTrigger_ShouldTrigger()
    {
        // setup
        var comp = RenderComponent<DataGridDetailRowComponent>();

        // test
        var rowsBefore = comp.FindAll( "#lblFraction" );
        foreach ( var item in comp.Instance.InMemoryData )
        {
            await comp.Instance.DataGridRef.ToggleDetailRow( item );
        }

        var rowsAfter = comp.FindAll( "#lblFraction" );

        foreach ( var item in comp.Instance.InMemoryData )
        {
            await comp.Instance.DataGridRef.ToggleDetailRow( item );
        }

        var rowsAfter2 = comp.FindAll( "#lblFraction" );

        // validate
        Assert.Equal( comp.Instance.InMemoryData.Count, rowsBefore.Count );
        Assert.Empty( rowsAfter );
        Assert.Equal( comp.Instance.InMemoryData.Count, rowsAfter2.Count );
    }


    [Fact]
    public async Task DetailRow_OnClick_Single_ToggleableFalse_ShouldTriggerOnlyOne()
    {
        // setup
        var comp = RenderComponent<DataGridDetailRowComponent>(
            parameters =>
            {
                parameters.Add( x => x.DetailRowStartsVisible, false );
                parameters.Add( x => x.DetailRowTrigger, ( context ) =>
                {
                    context.Single = true;
                    context.Toggleable = false;
                    return true;
                } );
            } );

        // test
        var rowsBefore = comp.FindAll( "#lblFraction" );
        Assert.Empty( rowsBefore );

        var selectableRows = comp.FindAll( "tr.table-row-selectable" );

        //Click First row , validate one detail row shows.
        await selectableRows[0].ClickAsync();
        var checkDetailRows = comp.FindAll( "#lblFraction" );
        Assert.Single( checkDetailRows );

        //Click second row , validate still only one detail row shows.
        await selectableRows[1].ClickAsync();
        checkDetailRows = comp.FindAll( "#lblFraction" );
        Assert.Single( checkDetailRows );

        //Click second row again , validate still only one detail row shows.
        await selectableRows[1].ClickAsync();
        checkDetailRows = comp.FindAll( "#lblFraction" );
        Assert.Single( checkDetailRows );
    }

    [Fact]
    public async Task DetailRow_OnClick_Single_ShouldTriggerOnlyOne()
    {
        // setup
        var comp = RenderComponent<DataGridDetailRowComponent>(
            parameters =>
            {
                parameters.Add( x => x.DetailRowStartsVisible, false );
                parameters.Add( x => x.DetailRowTrigger, ( context ) =>
                {
                    context.Single = true;
                    context.Toggleable = true;
                    return true;
                } );
            } );

        // test
        var rowsBefore = comp.FindAll( "#lblFraction" );
        Assert.Empty( rowsBefore );

        var selectableRows = comp.FindAll( "tr.table-row-selectable" );

        //Click First row , validate one detail row shows.
        await selectableRows[0].ClickAsync();
        var checkDetailRows = comp.FindAll( "#lblFraction" );
        Assert.Single( checkDetailRows );

        //Click second row , validate still only one detail row shows.
        await selectableRows[1].ClickAsync();
        checkDetailRows = comp.FindAll( "#lblFraction" );
        Assert.Single( checkDetailRows );

        //Click second row again , validate that no detail row shows as toggleable is set to true.
        await selectableRows[1].ClickAsync();
        checkDetailRows = comp.FindAll( "#lblFraction" );
        Assert.Empty( checkDetailRows );
    }


    [Fact]
    public async Task DetailRow_OnClick_ToggleableFalse_ShouldNotHide()
    {
        // setup
        var comp = RenderComponent<DataGridDetailRowComponent>(
            parameters =>
            {
                parameters.Add( x => x.DetailRowStartsVisible, false );
                parameters.Add( x => x.DetailRowTrigger, ( context ) =>
                {
                    context.Toggleable = false;
                    return true;
                } );
            } );

        // test
        var rowsBefore = comp.FindAll( "#lblFraction" );
        Assert.Empty( rowsBefore );

        var selectableRows = comp.FindAll( "tr.table-row-selectable" );

        //Validate 1 row detail
        await selectableRows[0].ClickAsync();
        var checkDetailRows = comp.FindAll( "#lblFraction" );
        Assert.Single( checkDetailRows );

        //Validate still 1 row detail 
        await selectableRows[0].ClickAsync();
        checkDetailRows = comp.FindAll( "#lblFraction" );
        Assert.Single( checkDetailRows );

        //Validate 2 row detail 
        await selectableRows[1].ClickAsync();
        checkDetailRows = comp.FindAll( "#lblFraction" );
        Assert.Collection( checkDetailRows, _ => { }, _ => { } );

        //Validate still 2 row detail 
        await selectableRows[1].ClickAsync();
        checkDetailRows = comp.FindAll( "#lblFraction" );
        Assert.Collection( checkDetailRows, _ => { }, _ => { } );
    }
}