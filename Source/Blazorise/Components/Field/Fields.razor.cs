#region Using directives
using System;
using System.Collections.Generic;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Container for multiple <see cref="Field"/> component that needs to be placed in a flexbox grid row.
/// </summary>
public partial class Fields : BaseColumnableComponent, IRowableComponent, IDisposable
{
    #region Members

    private string label;

    private string help;

    private int spaceUsedByColumnables = 0;

    private List<IColumnableComponent> columnables = new();

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Fields() );

        if ( ColumnSize != null )
        {
            builder.Append( ClassProvider.FieldsColumn() );
        }

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            if ( columnables is not null )
            {
                columnables.Clear();
                columnables = null;
            }
        }

        base.Dispose( disposing );
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
    /// Sets the field label.
    /// </summary>
    [Parameter]
    public string Label
    {
        get => label;
        set
        {
            label = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Sets the field help-text positioned bellow the field.
    /// </summary>
    [Parameter]
    public string Help
    {
        get => help;
        set
        {
            help = value;

            DirtyClasses();
        }
    }

    #endregion
}