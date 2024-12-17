#region Using directives
using System;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Context for pagination button.
/// </summary>
public class PageButtonContext
{
    /// <summary>
    /// Default context constructor.
    /// </summary>
    /// <param name="pageNumber">Button page number .</param>
    /// <param name="active">Indicates if page is active.</param>
    public PageButtonContext( long pageNumber, bool active )
    {
        PageNumber = pageNumber;
        Active = active;
    }

    /// <summary>
    /// Gets the page number.
    /// </summary>
    public long PageNumber { get; private set; }

    /// <summary>
    /// Gets the page number.
    /// </summary>
    [Obsolete( "PageNumer is deprecated and will be removed in future versions, please use PageNumber instead.", true )]
    public long PageNumer { get { return PageNumber; } set { PageNumber = value; } }

    /// <summary>
    /// Get the flag that indicates if the page is active.
    /// </summary>
    public bool Active { get; }
}