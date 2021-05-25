namespace Blazorise.DataGrid.Configuration
{
    public class VirtualizeOptions
    {
        /// <summary>
        /// Sets the table height when <see cref="DataGrid.DataGrid{TItem}.Virtualize"/> feature is enabled (defaults to 500px).
        /// </summary>
        public string TableHeight { get; set; } = "500px";

        // Summary:
        //     Gets or sets a value that determines how many additional items will be rendered
        //     before and after the visible region. This help to reduce the frequency of rendering
        //     during scrolling. However, higher values mean that more elements will be present
        //     in the page.
        public int OverscanCount { get; set; } = 10;
    }
}