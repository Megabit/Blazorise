#region Using directives
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

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

    /// <inheritdoc/>
    public void NotifyColumnableInitialized( IColumnableComponent column )
    {
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
        if ( column is not null && columnables.IndexOf( column ) <= 0 )
            spaceUsedByColumnables = 0;
    }

    /// <inheritdoc/>
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