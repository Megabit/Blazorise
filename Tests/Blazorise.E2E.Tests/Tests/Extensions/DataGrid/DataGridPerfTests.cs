namespace Blazorise.E2E.Tests.Tests.Extensions.DataGrid;

public class DataGridPerfTests : BlazorisePageTest
{
    [SetUp]
    public async Task Init()
    {

    }


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

}
