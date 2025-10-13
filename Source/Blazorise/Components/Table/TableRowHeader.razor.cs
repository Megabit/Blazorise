#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Defines a cell as header of a group of table cells.
/// </summary>
public partial class TableRowHeader : BaseDraggableComponent, IDisposable
{
    #region Members

    private double? fixedPositionStartOffset;

    private double? fixedPositionEndOffset;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        ParentTableRow?.AddTableRowHeader( this );

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TableRowHeader() );
        builder.Append( ClassProvider.TableRowHeaderFixed( FixedPosition ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( FixedPosition == TableColumnFixedPosition.Start )
        {
            if ( ParentTable.FixedColumnsPositionSync )
            {
                var startOffset = ParentTableRow.GetTableRowHeaderFixedPositionStartOffset( this );
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
                var endOffset = ParentTableRow.GetTableRowHeaderFixedPositionEndOffset( this );
                builder.Append( $"right:{endOffset.ToPreciseString()}px" );
            }
            else if ( fixedPositionEndOffset.HasValue )
            {
                builder.Append( $"right:{fixedPositionEndOffset.ToPreciseString()}px" );
            }
        }

        base.BuildStyles( builder );
    }

    /// <inheritdoc/>
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        builder
            .OpenElement( "th" )
            .Id( ElementId )
            .Class( ClassNames )
            .Style( StyleNames )
            .Draggable( DraggableString )
            .Scope( "row" )
            .ColSpan( ColumnSpan )
            .RowSpan( RowSpan );

        if ( Clicked.HasDelegate )
            builder.OnClick( this, EventCallback.Factory.Create<MouseEventArgs>( this, OnClickHandler ) );

        // build drag-and-drop related events
        BuildDraggableEventsRenderTree( builder );

        builder
            .Data( "caption", MobileModeCaption );

        if ( Attributes is not null )
            builder.Attributes( Attributes );

        builder.ElementReferenceCapture( capturedRef => ElementRef = capturedRef );

        if ( ChildContent is not null )
            builder.Content( ChildContent );

        builder.CloseElement(); // </th>

        base.BuildRenderTree( builder );
    }

    /// <summary>
    /// Handles the row onclick event.
    /// </summary>
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
            ParentTableRow?.RemoveTableRowHeader( this );
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            ParentTableRow?.RemoveTableRowHeader( this );
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
    /// Number of rows a cell should span.
    /// </summary>
    [Parameter] public int? RowSpan { get; set; }

    /// <summary>
    /// Number of columns a cell should span.
    /// </summary>
    [Parameter] public int? ColumnSpan { get; set; }

    /// <summary>
    /// Defines the fixed position of the row header within the table.
    /// </summary>
    [Parameter] public TableColumnFixedPosition FixedPosition { get; set; }

    /// <summary>
    /// Occurs when the row header is clicked.
    /// </summary>
    [Parameter] public EventCallback<BLMouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="TableRowHeader"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// When the <see cref="Table.ResponsiveMode"/> is set to <see cref="TableResponsiveMode.Mobile"/>, this caption will be used for the row.
    /// </summary>
    [Parameter] public string MobileModeCaption { get; set; }

    #endregion
}