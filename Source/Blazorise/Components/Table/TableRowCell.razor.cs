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
/// Defines a cell of a table that contains data.
/// </summary>
public partial class TableRowCell : BaseDraggableComponent, IDisposable
{
    #region Members

    private Color color = Color.Default;

    private double? fixedPositionStartOffset;

    private double? fixedPositionEndOffset;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentTableRow?.AddTableRowCell( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TableRowCell() );
        builder.Append( ClassProvider.TableRowCellColor( Color ) );
        builder.Append( ClassProvider.TableRowCellFixed( FixedPosition ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( FixedPosition == TableColumnFixedPosition.Start )
        {
            if ( ParentTable.FixedColumnsPositionSync )
            {
                var startOffset = ParentTableRow.GetTableRowCellFixedPositionStartOffset( this );
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
                var endOffset = ParentTableRow.GetTableRowCellFixedPositionEndOffset( this );
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
        return Clicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( eventArgs ) );
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
            ParentTableRow?.RemoveTableRowCell( this );
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            ParentTableRow?.RemoveTableRowCell( this );
        }

        return base.DisposeAsync( disposing );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the cascaded parent table component.
    /// </summary>
    [CascadingParameter] protected Table ParentTable { get; set; }

    /// <summary>
    /// Gets or sets the cascaded parent table row component.
    /// </summary>
    [CascadingParameter] protected TableRow ParentTableRow { get; set; }

    /// <summary>
    /// Gets or sets the cell variant color.
    /// </summary>
    [Parameter]
    public Color Color
    {
        get => color;
        set
        {
            color = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Number of rows a cell should span.
    /// </summary>
    [Parameter] public int? RowSpan { get; set; }

    /// <summary>
    /// Number of columns a cell should span.
    /// </summary>
    [Parameter] public int? ColumnSpan { get; set; }

    /// <summary>
    /// Defines the fixed position of the row cell within the table.
    /// </summary>
    [Parameter] public TableColumnFixedPosition FixedPosition { get; set; }

    /// <summary>
    /// Occurs when the row cell is clicked.
    /// </summary>
    [Parameter] public EventCallback<BLMouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="TableRowCell"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Used to prevent the default action for an <see cref="OnClickHandler"/> event.
    /// </summary>
    [Parameter] public bool ClickPreventDefault { get; set; }

    /// <summary>
    /// Used to stop progation of the click action event.
    /// </summary>
    [Parameter] public bool ClickStopPropagation { get; set; }

    /// <summary>
    /// When the <see cref="Table.ResponsiveMode"/> is set to <see cref="TableResponsiveMode.Mobile"/>, this caption will be used for the row.
    /// </summary>
    [Parameter] public string MobileModeCaption { get; set; }

    #endregion
}