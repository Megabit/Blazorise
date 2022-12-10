#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <inheritdoc/>
public class RowContext : IRowContext
{
    #region Members

    private int spaceUsedByColumnables = 0;

    private List<IColumnableComponent> columnables;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public void NotifyColumnableInitialized( IColumnableComponent column )
    {
        columnables ??= new();

        if ( columnables is not null && !columnables.Contains( column ) )
        {
            columnables.Add( column );
        }
    }

    /// <inheritdoc/>
    public void NotifyColumnableRemoved( IColumnableComponent column )
    {
        if ( columnables is not null && columnables.Contains( column ) )
        {
            columnables.Remove( column );
        }
    }

    /// <inheritdoc/>
    public void ResetUsedSpace( IColumnableComponent column )
    {
        if ( column is not null && columnables is not null && columnables.IndexOf( column ) <= 0 )
            spaceUsedByColumnables = 0;
    }

    /// <inheritdoc/>
    public void IncreaseUsedSpace( int space )
    {
        spaceUsedByColumnables += space;

        if ( spaceUsedByColumnables > 12 )
            spaceUsedByColumnables = 12;
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public int TotalUsedSpace => spaceUsedByColumnables;

    #endregion
}
