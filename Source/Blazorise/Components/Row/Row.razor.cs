#region Using directives
using System.Collections.Generic;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A wrapper that represents a row in a flexbox grid.
/// </summary>
public partial class Row : BaseRowComponent
{
    #region Members

    private IFluentRowColumns rowColumns;

    private IFluentGutter gutter;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Row() );

        if ( RowColumns is not null && RowColumns.HasSizes )
            builder.Append( RowColumns.Class( ClassProvider ) );

        if ( gutter is not null )
            builder.Append( Gutter.Class( ClassProvider ) );

        base.BuildClasses( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Defines the number of columns to show in a row.
    /// </summary>
    [Parameter]
    public IFluentRowColumns RowColumns
    {
        get => rowColumns;
        set
        {
            rowColumns = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines the padding between your columns, used to responsively space and align content in the Blazorise grid system.
    /// </summary>
    [Parameter]
    public IFluentGutter Gutter
    {
        get => gutter;
        set
        {
            if ( gutter == value )
                return;

            gutter = value;

            DirtyClasses();
        }
    }

    #endregion
}