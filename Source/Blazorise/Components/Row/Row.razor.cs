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

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Row() );
        builder.Append( ClassProvider.RowNoGutters( NoGutters ) );

        if ( RowColumns is not null && RowColumns.HasSizes )
            builder.Append( RowColumns.Class( ClassProvider ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.RowGutter( GutterState ), GutterState != default );

        base.BuildStyles( builder );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current gutter value based on the supplied parameters.
    /// </summary>
    protected (int Horizontal, int Vertical) GutterState
        => Gutter ?? (HorizontalGutter ?? 0, VerticalGutter ?? 0);

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
    /// Row grid spacing - we recommend setting Horizontal and/or Vertical it to (16 + 8n). (n stands for natural number.)
    /// </summary>
    [Parameter] public (int Horizontal, int Vertical)? Gutter { get; set; }

    /// <summary>
    /// Row grid Horizontal spacing. (n stands for natural number.)
    /// </summary>
    [Parameter] public int? HorizontalGutter { get; set; }

    /// <summary>
    /// Row grid Vertical spacing. (n stands for natural number.)
    /// </summary>
    [Parameter] public int? VerticalGutter { get; set; }

    /// <summary>
    /// Removes the negative margins from row and the horizontal padding from all immediate children columns.
    /// </summary>
    [Parameter] public bool NoGutters { get; set; }

    #endregion
}