namespace Blazorise.E2E.Tests.Tests.Extensions.DataGrid;

public class DataGridScrollToRowTests : BlazorisePageTest
{
    [TestCase( false, false, false, 5 )]
    [TestCase( true, false, false, 5 )]
    [TestCase( false, true, false, 40 )]
    [TestCase( false, true, false, 500 )]
    [TestCase( true, true, false, 500 )]
    [TestCase( true, true, true, 500 )]
    public async Task ScrollToRow_ShouldKeepRequestedRowBelowHeaderAndInsideContainer( bool groupHeaders, bool virtualize, bool manualRead, int firstIndex )
    {
        await SelectTestComponent<DataGridScrollToRowComponent>();

        if ( groupHeaders )
        {
            await Page.Locator( "#groupHeaders" ).CheckAsync();
        }

        if ( virtualize )
        {
            await Page.Locator( "#virtualize" ).CheckAsync();
        }

        if ( manualRead )
        {
            await Page.Locator( "#manualRead" ).CheckAsync();
        }

        await Expect( Page.Locator( "#scrollGrid > thead > tr" ) ).ToHaveCountAsync( groupHeaders ? 2 : 1 );
        await Expect( Page.Locator( "#scrollGrid > tbody > tr > td:first-child" ).First ).ToHaveTextAsync( "0" );

        if ( virtualize )
        {
            await Expect( Page.Locator( "#scrollGrid" ).GetByText( "500", new() { Exact = true } ) ).ToHaveCountAsync( 0 );
        }
        else
        {
            await Expect( Page.Locator( "#scrollGrid > tbody > tr" ) ).ToHaveCountAsync( 30 );
        }

        var indices = virtualize ? new[] { firstIndex, 999, 40, 0 } : new[] { firstIndex, 6, 20, 10, 29, 0 };

        foreach ( var index in indices )
        {
            await Page.Locator( $"#scrollTo{index}" ).ClickAsync();

            try
            {
                // Wait for rendering and scrolling to bring the requested item into the unobscured viewport.
                await Page.WaitForFunctionAsync( """
                    index => {
                        const table = document.getElementById('scrollGrid');
                        const row = Array.from(table.querySelectorAll(':scope > tbody > tr'))
                            .find(row => row.cells[0]?.textContent.trim() === String(index));
                        if (!row) {
                            return false;
                        }
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
            catch ( TimeoutException )
            {
                var diagnostics = await Page.EvaluateAsync<string>( """
                    () => {
                        const table = document.getElementById('scrollGrid');
                        const container = table.parentElement;
                        return JSON.stringify({
                            scrollTop: container.scrollTop,
                            container: container.getBoundingClientRect().toJSON(),
                            headers: Array.from(table.querySelectorAll(':scope > thead > tr > th'))
                                .map(cell => cell.getBoundingClientRect().bottom),
                            rows: Array.from(table.querySelectorAll(':scope > tbody > tr')).map(row => ({
                                text: row.cells[0]?.textContent.trim(),
                                index: row.dataset.virtualRowIndex,
                                top: row.getBoundingClientRect().top,
                                bottom: row.getBoundingClientRect().bottom
                            }))
                        });
                    }
                    """ );

                Assert.Fail( $"ScrollToRow({index}) did not bring the requested row into view. {diagnostics}" );
            }
        }
    }
}