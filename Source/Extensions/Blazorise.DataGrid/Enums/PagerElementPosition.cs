namespace Blazorise.DataGrid;

/// <summary>
/// Sets a Pager Element's position.
/// </summary>
public enum PagerElementPosition
{
    /// <summary>
    /// Positions the element at the default position. 
    /// If both ButtonRowPosition and PaginationPosition are set to default. Whether ButtonRow is shown or not will make it so the Pagination is positioned accordingly.
    /// </summary>
    Default,

    /// <summary>
    /// Positions the element at the start.
    /// </summary>
    Start,

    /// <summary>
    /// Positions the element at the center.
    /// </summary>
    Center,

    /// <summary>
    /// Positions the element at the end.
    /// </summary>
    End
}