#region Using directives
using System.Collections.Generic;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A wrapper that represents a row in a css grid.
/// </summary>
public partial class Grid : BaseGridComponent
{
    #region Members

    private IFluentGridRows gridRows;

    private IFluentGridColumns gridColumns;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Grid( HasRowsSizes, HasColumnSizes ) );

        if ( Rows is not null && Rows.HasSizes )
            builder.Append( Rows.Class( ClassProvider ) );

        if ( HasColumnSizes )
            builder.Append( Columns.Class( ClassProvider ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Determines if the rows sizes are defined.
    /// </summary>
    protected bool HasRowsSizes => Rows is not null && Rows.HasSizes;

    /// <summary>
    /// Determines if the column sizes are defined.
    /// </summary>
    protected bool HasColumnSizes => Columns is not null && Columns.HasSizes;

    /// <summary>
    /// Defines the number of rows to show in a grid.
    /// </summary>
    [Parameter]
    public IFluentGridRows Rows
    {
        get => gridRows;
        set
        {
            gridRows = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the number of columns to show in a grid.
    /// </summary>
    [Parameter]
    public IFluentGridColumns Columns
    {
        get => gridColumns;
        set
        {
            gridColumns = value;

            DirtyClasses();
        }
    }

    #endregion
}