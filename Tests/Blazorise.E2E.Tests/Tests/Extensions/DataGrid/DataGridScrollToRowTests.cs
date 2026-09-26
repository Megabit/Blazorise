namespace Blazorise.E2E.Tests.Tests.Extensions.DataGrid;

public class DataGridScrollToRowTests : BlazorisePageTest
{
    [TestCase( false )]
    [TestCase( true )]
    public async Task ScrollToRow_ShouldKeepRequestedRowBelowHeaderAndInsideContainer( bool groupHeaders )
    {
        await SelectTestComponent<DataGridScrollToRowComponent>();

        if ( groupHeaders )
        {
            await Page.Locator( "#groupHeaders" ).CheckAsync();
        }

        await Expect( Page.Locator( "#scrollGrid > thead > tr" ) ).ToHaveCountAsync( groupHeaders ? 2 : 1 );
        await Expect( Page.Locator( "#scrollGrid > tbody > tr" ) ).ToHaveCountAsync( 30 );

        foreach ( int index in new[] { 5, 6, 20, 10, 29, 0 } )
        {
            await Page.Locator( $"#scrollTo{index}" ).ClickAsync();

            // Wait for smooth scrolling to bring the entire row into the unobscured viewport.
            await Page.WaitForFunctionAsync( """
                index => {
                    const table = document.getElementById('scrollGrid');
                    const row = table.querySelector(`:scope > tbody > tr[data-row-index='${index}']`);
                    const container = table.parentElement;
                    const containerRect = container.getBoundingClientRect();
                    const viewportTop = containerRect.top + container.clientTop;
                    const viewportBottom = viewportTop + container.clientHeight;
                    const headerBottom = Math.max(viewportTop, ...Array.from(
                        table.querySelectorAll(':scope > thead > tr > th'),
                        cell => cell.getBoundingClientRect().bottom));
                    const rowRect = row.getBoundingClientRect();

                    return rowRect.height > 0
                        && rowRect.top >= headerBottom - 1
                        && rowRect.bottom <= viewportBottom + 1;
                }
                """, index );
        }
    }
}