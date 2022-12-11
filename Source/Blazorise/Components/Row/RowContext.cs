#region Using directives
using System.Collections.Generic;
#endregion

namespace Blazorise;

/// <inheritdoc/>
public class RowContext : IRowContext
{
    #region Members

    private List<IColumnComponent> columns;

    private Dictionary<Breakpoint, int> usedSpaces = new();

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
        {
            foreach ( var kv in usedSpaces )
                usedSpaces[kv.Key] = 0;
        }
    }

    /// <inheritdoc/>
    public int IncreaseUsedSpace( int space, Breakpoint breakpoint )
    {
        usedSpaces.TryGetValue( breakpoint, out var usedSpace );

        usedSpace += space;

        if ( usedSpace > 12 )
            usedSpace = 12;

        usedSpaces[breakpoint] = usedSpace;

        return usedSpace;
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public int TotalUsedSpace( Breakpoint breakpoint ) => usedSpaces[breakpoint];

    #endregion
}
