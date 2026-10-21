namespace Blazorise.DataGrid;

internal struct VirtualizeState
{
    /// <summary>
    /// Index of the first item in the loaded virtualized data window, or -1 while a new window is loading.
    /// </summary>
    public int StartIndex { get; set; }

    /// <summary>
    /// When Virtualize is de-activated the grid should refresh.
    /// </summary>
    public bool WasActive { get; set; }

    /// <summary>
    /// Records last know edit scroll position, to scroll back to.
    /// </summary>
    public int? EditLastKnownScroll { get; set; }
}