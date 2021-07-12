namespace Blazorise.DataGrid.Configuration
{
    internal struct VirtualizeState
    {
        /// <summary>
        /// When Virtualize is de-activated the grid should refresh.
        /// </summary>
        public bool WasActive { get; set; }

        /// <summary>
        /// Records last know edit scroll position, to scroll back to.
        /// </summary>
        public int? EditLastKnownScroll { get; set; }
    }
}