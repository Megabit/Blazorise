#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <inheritdoc/>
public class RowContext : IRowContext
{
    #region Members

    private int spaceUsedByColumns = 0;

    private List<IColumnComponent> columns;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void NotifyColumnInitialized( IColumnComponent column )
    {
        columns ??= new();

        if ( columns is not null && !columns.Contains( column ) )
        {
            columns.Add( column );
        }
    }

    /// <inheritdoc/>
    public void NotifyColumnRemoved( IColumnComponent column )
    {
        if ( columns is not null && columns.Contains( column ) )
        {
            columns.Remove( column );
        }
    }

    /// <inheritdoc/>
    public void ResetUsedSpace( IColumnComponent column )
    {
        if ( column is not null && columns is not null && columns.IndexOf( column ) <= 0 )
            spaceUsedByColumns = 0;
    }

    /// <inheritdoc/>
    public void IncreaseUsedSpace( int space )
    {
        spaceUsedByColumns += space;

        if ( spaceUsedByColumns > 12 )
            spaceUsedByColumns = 12;
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public int TotalUsedSpace => spaceUsedByColumns;

    #endregion
}
