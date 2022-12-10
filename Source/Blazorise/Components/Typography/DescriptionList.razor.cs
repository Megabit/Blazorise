#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A description list is a list of items with a description or definition of each item.
/// </summary>
public partial class DescriptionList : BaseTypographyComponent, IRowableComponent
{
    #region Members

    private bool row;

    private int spaceUsedByColumnables = 0;

    private List<IColumnableComponent> columnables = new();

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.DescriptionList() );

        if ( Row )
            builder.Append( ClassProvider.Row() );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    public void NotifyColumnInitialized( IColumnableComponent column )
    {
        if ( columnables is not null && !columnables.Contains( column ) )
        {
            columnables.Add( column );
        }
    }

    /// <inheritdoc/>
    public void NotifyColumnDestroyed( IColumnableComponent column )
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
    /// Specifies that description list will be arranged in a rows and columns.
    /// </summary>
    [Parameter]
    public bool Row
    {
        get => row;
        set
        {
            row = value;

            DirtyClasses();
        }
    }

    #endregion
}