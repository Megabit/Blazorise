#region Using directives
using System.Collections.Generic;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// A wrapper that represents a row in a flexbox grid.
/// </summary>
public partial class Row : BaseComponent
{
    #region Members

    private IFluentRowColumns rowColumns;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.Row() );
        builder.Append( ClassProvider.RowNoGutters(), NoGutters );

        if ( RowColumns != null && RowColumns.HasSizes )
            builder.Append( RowColumns.Class( ClassProvider ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.RowGutter( GutterState ), GutterState != default );

        base.BuildStyles( builder );
    }

    private List<BaseColumnableComponent> columns = new List<BaseColumnableComponent>();

    internal void NotifyColumnInitialized( BaseColumnableComponent column )
    {
        if ( !columns.Contains( column ) )
        {
            columns.Add( column );
        }
    }

    internal void NotifyColumnDestroyed( BaseColumnableComponent column )
    {
        if ( columns.Contains( column ) )
        {
            columns.Remove( column );
        }
    }

    public int ColumnIndex( BaseColumnableComponent column ) => columns.IndexOf( column );

    private int usedSpace = 0;

    public void ResetUsedSpace( BaseColumnableComponent column )
    {
        if ( column is not null && ColumnIndex( column ) <= 0 )
            usedSpace = 0;
    }

    public void IncreaseUsedSpace( int space )
    {
        usedSpace += space;
    }

    public int TotalUsedSpace => usedSpace;

    private static int ToColumnWidthIndex( ColumnWidth columnWidth )
    {
        return columnWidth switch
        {
            Blazorise.ColumnWidth.Is1 => 1,
            Blazorise.ColumnWidth.Is2 => 2,
            Blazorise.ColumnWidth.Is3 or Blazorise.ColumnWidth.Quarter => 3,
            Blazorise.ColumnWidth.Is4 or Blazorise.ColumnWidth.Third => 4,
            Blazorise.ColumnWidth.Is5 => 5,
            Blazorise.ColumnWidth.Is6 or Blazorise.ColumnWidth.Half => 6,
            Blazorise.ColumnWidth.Is7 => 7,
            Blazorise.ColumnWidth.Is8 => 8,
            Blazorise.ColumnWidth.Is9 => 9,
            Blazorise.ColumnWidth.Is10 => 10,
            Blazorise.ColumnWidth.Is11 => 11,
            Blazorise.ColumnWidth.Is12 or Blazorise.ColumnWidth.Full => 12,
            Blazorise.ColumnWidth.Auto => 0,
            _ => 0,
        };
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

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="Row"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}