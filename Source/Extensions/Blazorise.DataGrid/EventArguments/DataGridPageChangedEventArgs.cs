#region Using directives
using System;
#endregion

namespace Blazorise.DataGrid;

/// <summary>
/// Provides the data for datagrid page change.
/// </summary>
public class DataGridPageChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of page changed event argument.
    /// </summary>
    /// <param name="page">Page number at the moment of initialization.</param>
    /// <param name="pageSize">Maximum number of items per page.</param>
    public DataGridPageChangedEventArgs( long page, int pageSize )
    {
        Page = page;
        PageSize = pageSize;
    }

    /// <summary>
    /// Gets the requested page number.
    /// </summary>
    public long Page { get; }

    /// <summary>
    /// Gets the max number of items requested by page.
    /// </summary>
    public int PageSize { get; }
}