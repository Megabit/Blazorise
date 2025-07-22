#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Defines a cell as header of a group of table cells.
/// </summary>
public partial class TableHeaderCell : BaseDraggableComponent, IDisposable
{
    #region Members

    private Cursor cursor;

    private double? fixedPositionStartOffset;

    private double? fixedPositionEndOffset;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentTableRow?.AddTableHeaderCell( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TableHeaderCell() );
        builder.Append( ClassProvider.TableHeaderCellCursor( Cursor ) );
        builder.Append( ClassProvider.TableHeaderCellFixed( FixedPosition ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( FixedPosition == TableColumnFixedPosition.Start )
        {
            if ( ParentTable.FixedColumnsPositionSync )
            {
                var startOffset = ParentTableRow.GetTableHeaderCellFixedPositionStartOffset( this );
                builder.Append( $"left:{startOffset.ToPreciseString()}px" );
            }
            else if ( fixedPositionStartOffset.HasValue )
            {
                builder.Append( $"left:{fixedPositionStartOffset.ToPreciseString()}px" );
            }
        }
        else if ( FixedPosition == TableColumnFixedPosition.End )
        {
            if ( ParentTable.FixedColumnsPositionSync )
            {
                var endOffset = ParentTableRow.GetTableHeaderCellFixedPositionEndOffset( this );
                builder.Append( $"right:{endOffset.ToPreciseString()}px" );
            }
            else if ( fixedPositionEndOffset.HasValue )
            {
                builder.Append( $"right:{fixedPositionEndOffset.ToPreciseString()}px" );
            }
        }

        base.BuildStyles( builder );
    }

    /// <summary>
    /// Handles the header cell clicked event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnClickHandler( MouseEventArgs eventArgs )
    {
        return Clicked.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Sets the fixed position start offset.
    /// </summary>
    /// <param name="width">Size in pixels.</param>
    internal void SetFixedPositionStartOffset( double width )
    {
        fixedPositionStartOffset = width;

        DirtyStyles();
    }

    /// <summary>
    /// Sets or increased the fixed position end offset by the provided width.
    /// </summary>
    /// <param name="width">Size in pixels.</param>
    internal void IncreaseFixedPositionEndOffset( double width )
    {
        if ( fixedPositionEndOffset.HasValue )
        {
            fixedPositionEndOffset += width;
        }
        else
        {
            fixedPositionEndOffset = width;
        }

        DirtyStyles();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            ParentTableRow?.RemoveTableHeaderCell( this );
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            ParentTableRow?.RemoveTableHeaderCell( this );
        }

        return base.DisposeAsync( disposing );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the fixed position data attribute.
    /// </summary>
    protected string FixedPositionDataAttribute => FixedPosition switch
    {
        TableColumnFixedPosition.Start => "start",
        TableColumnFixedPosition.End => "end",
        _ => null
    };

    /// <summary>
    /// Gets or sets the cascaded parent table component.
    /// </summary>
    [CascadingParameter] protected Table ParentTable { get; set; }

    /// <summary>
    /// Gets or sets the cascaded parent table row component.
    /// </summary>
    [CascadingParameter] protected TableRow ParentTableRow { get; set; }

    /// <summary>
    /// Number of rows a cell should span.
    /// </summary>
    [Parameter] public int? RowSpan { get; set; }

    /// <summary>
    /// Number of columns a cell should span.
    /// </summary>
    [Parameter] public int? ColumnSpan { get; set; }

    /// <summary>
    /// Defines the fixed position of the header cell within the table.
    /// </summary>
    [Parameter] public TableColumnFixedPosition FixedPosition { get; set; }

    /// <summary>
    /// Defines the mouse cursor based on the behaviour by the current css framework.
    /// </summary>
    [Parameter]
    public Cursor Cursor
    {
        get => cursor;
        set
        {
            if ( cursor == value )
                return;

            cursor = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Occurs when the header cell is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="TableHeaderCell"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}