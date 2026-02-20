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
    public PageButtonContext( int pageNumber, bool active )
    {
        PageNumber = pageNumber;
        Active = active;
    }

    /// <summary>
    /// Gets the page number.
    /// </summary>
    public int PageNumber { get; private set; }

    /// <summary>
    /// Get the flag that indicates if the page is active.
    /// </summary>
    public bool Active { get; }
}