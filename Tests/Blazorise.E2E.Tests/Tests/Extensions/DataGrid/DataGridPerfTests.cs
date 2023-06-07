namespace Blazorise.E2E.Tests.Tests.Extensions.DataGrid;

public class DataGridPerfTests : BlazorisePageTest
{

    [Test]
    public async Task SelectRow_Should_RenderOnce()
    {
        await SelectTestComponent<DataGridPerfComponent>();

        var perfRenderCalls = Page.Locator( "#PerfRenderCalls" );
        var perfReset = Page.Locator( "#PerfReset" );
        var row = Page.Locator( "tbody tr.table-row-selectable:first-child" );

        await perfReset.ClickAsync();
        await row.ClickAsync();

        var callsAfterSelection = await perfRenderCalls.InnerTextAsync();

        Assert.AreEqual( 1, int.Parse( callsAfterSelection ) );
    }

    [Test]
    public async Task SelectRow_Should_RenderOnce_When_BindSelectedRowIsSet()
    {
        await SelectTestComponent<DataGridPerfBindSelectedComponent>();

        var perfRenderCalls = Page.Locator( "#PerfRenderCalls" );
        var perfReset = Page.Locator( "#PerfReset" );
        var row = Page.Locator( "tbody tr.table-row-selectable:first-child" );

        await perfReset.ClickAsync();
        await row.ClickAsync();

        var callsAfterSelection = await perfRenderCalls.InnerTextAsync();

        Assert.AreEqual( 1, int.Parse( callsAfterSelection ) );
    }

    [Test]
    public async Task SelectRow_Multiple_Should_RenderOnce()
    {
        await SelectTestComponent<DataGridPerfComponent>();

        var selectionMode = Page.Locator( "#dataGridSelectionMode" );
        await selectionMode.SelectOptionAsync( new[] { "Multiple" } );

        var perfRenderCalls = Page.Locator( "#PerfRenderCalls" );
        var perfReset = Page.Locator( "#PerfReset" );
        var row = Page.Locator( "tbody tr.table-row-selectable:first-child" );

        await perfReset.ClickAsync();
        await row.ClickAsync();

        var callsAfterSelection = await perfRenderCalls.InnerTextAsync();

        Assert.AreEqual( 1, int.Parse( callsAfterSelection ) );
    }

    [Test]
    public async Task SelectRow_Multiple_Should_RenderOnce_When_BindSelectedRowsIsSet()
    {
        await SelectTestComponent<DataGridPerfBindSelectedComponent>();

        var selectionMode = Page.Locator( "#dataGridSelectionMode" );
        await selectionMode.SelectOptionAsync( new[] { "Multiple" } );

        var perfRenderCalls = Page.Locator( "#PerfRenderCalls" );
        var perfReset = Page.Locator( "#PerfReset" );
        var row = Page.Locator( "tbody tr.table-row-selectable:first-child" );

        await perfReset.ClickAsync();
        await row.ClickAsync();

        var callsAfterSelection = await perfRenderCalls.InnerTextAsync();

        Assert.AreEqual( 1, int.Parse( callsAfterSelection ) );
    }

}
