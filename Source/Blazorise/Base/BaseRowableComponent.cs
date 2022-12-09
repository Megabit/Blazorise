#region Using directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

public interface IRowableComponent
{
    void NotifyColumnInitialized( IColumnableComponent column );

    void NotifyColumnDestroyed( IColumnableComponent column );

    void ResetUsedSpace( IColumnableComponent column );

    void IncreaseUsedSpace( int space );

    /// <summary>
    /// Gets the total space used by the columns placed inside of the row component.
    /// </summary>
    int TotalUsedSpace { get; }
}

/// <summary>
/// Base class for components that are containers for other components.
/// </summary>
public abstract class BaseRowableComponent : BaseComponent, IRowableComponent
{
    #region Members

    private int spaceUsedByColumnables = 0;

    private List<IColumnableComponent> columnables = new();

    #endregion

    #region Methods

    public void NotifyColumnInitialized( IColumnableComponent column )
    {
        if ( !columnables.Contains( column ) )
        {
            columnables.Add( column );
        }
    }

    public void NotifyColumnDestroyed( IColumnableComponent column )
    {
        if ( columnables.Contains( column ) )
        {
            columnables.Remove( column );
        }
    }

    public void ResetUsedSpace( IColumnableComponent column )
    {
        if ( column is not null && columnables.IndexOf( column ) <= 0 )
            spaceUsedByColumnables = 0;
    }

    public void IncreaseUsedSpace( int space )
    {
        spaceUsedByColumnables += space;
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    public int TotalUsedSpace => spaceUsedByColumnables;

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="BaseRowableComponent"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}