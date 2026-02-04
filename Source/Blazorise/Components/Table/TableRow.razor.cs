#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Component defines a row of cells in a table. The row's cells can then be established using a mix of <see cref="TableRowCell"/> (data cell) components.
/// </summary>
public partial class TableRow : BaseDraggableComponent
{
    #region Members

    private string elementHashCode;

    private List<TableHeaderCell> tableHeaderCells;

    private List<TableRowHeader> tableRowHeaders;

    private List<TableRowCell> tableRowCells;

    private Color color = Color.Default;

    private bool selected;

    private Cursor hoverCursor;

    private double fixedStartCellPosition;

    /// <summary>
    /// Triggers when the width of the cell with TableColumnFixedPosition.End changes.
    /// </summary>
    public event EventHandler<TableRowCellFixedPositionEndAddedEventArgs> TableRowCellFixedPositionEndAdded;

    private List<EventHandler<TableRowCellFixedPositionEndAddedEventArgs>> tableRowCellFixedPositionEndAddedHandlers;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if ( ParentTable.FixedColumnsPositionSync )
        {
            tableHeaderCells = new();
            tableRowHeaders = new();
            tableRowCells = new();
        }
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TableRow( ParentTable.Striped, ParentTable.Hoverable ) );
        builder.Append( ClassProvider.TableRowColor( Color ) );
        builder.Append( ClassProvider.TableRowIsSelected( Selected ) );
        builder.Append( ClassProvider.TableRowHoverCursor( HoverCursor ) );

        base.BuildClasses( builder );
    }

    /// <inheritdoc/>
    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        builder
            .OpenComponent<CascadingValue<TableRow>>()
            .Attribute( "Value", this )
            .Attribute( "IsFixed", true );

        builder.Attribute( "ChildContent", (RenderFragment)( ( builder2 ) =>
        {
            builder2
               .OpenElement( "tr" )
               .Key( elementHashCode )
               .Id( ElementId )
               .Class( ClassNames )
               .Style( StyleNames )
               .Draggable( DraggableString );

            builder2.OnClick( this, EventCallback.Factory.Create<MouseEventArgs>( this, OnClickHandler ) );

            if ( MouseOver.HasDelegate )
            {
                builder2.OnMouseOver( this, EventCallback.Factory.Create<MouseEventArgs>( this, OnMouseOverHandler ) );
                builder2.OnMouseOverStopPropagation( true );
            }

            if ( MouseLeave.HasDelegate )
            {
                builder2.OnMouseLeave( this, EventCallback.Factory.Create<MouseEventArgs>( this, OnMouseLeaveHandler ) );
                builder2.OnMouseLeaveStopPropagation( true );
            }

            // build drag-and-drop related events
            BuildDraggableEventsRenderTree( builder2 );

            if ( Attributes is not null )
                builder2.Attributes( Attributes );

            builder2.ElementReferenceCapture( capturedRef => ElementRef = capturedRef );

            if ( ChildContent is not null )
                builder2.Content( ChildContent );

            builder2.CloseElement(); // </tr>
        } ) );

        builder.CloseComponent(); // </CascadingValue>

        base.BuildRenderTree( builder );
    }

    /// <summary>
    /// Handles the row clicked event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnClickHandler( MouseEventArgs eventArgs )
    {
        // https://stackoverflow.com/questions/5497073/how-to-differentiate-single-click-event-and-double-click-event
        // works good enough. Click is still called before the double click, but it is advise to not use both events anyway.
        // We'll be treating any Detail higher then 2 as the user constantly clicking, therefore triggering Single Click.
        if ( eventArgs.Detail == 1 || eventArgs.Detail > 2 )
            return Clicked.InvokeAsync( eventArgs );
        else if ( eventArgs.Detail == 2 )
            return DoubleClicked.InvokeAsync( eventArgs );
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the row mouse leave event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnMouseLeaveHandler( MouseEventArgs eventArgs )
    {
        return MouseLeave.InvokeAsync( eventArgs );
    }

    /// <summary>
    /// Handles the row mouseover event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnMouseOverHandler( MouseEventArgs eventArgs )
    {
        return MouseOver.InvokeAsync( eventArgs );
    }

    internal void AddTableHeaderCell( TableHeaderCell tableHeaderCell )
    {
        if ( ParentTable.FixedColumnsPositionSync )
        {
            tableHeaderCells?.Add( tableHeaderCell );
            RecalculateHashCode();
        }
        else
        {
            SetFixedCellPosition( tableHeaderCell.Width, tableHeaderCell.FixedPosition, tableHeaderCell.SetFixedPositionStartOffset, tableHeaderCell.IncreaseFixedPositionEndOffset );
        }
    }

    internal void RemoveTableHeaderCell( TableHeaderCell tableHeaderCell )
    {
        tableHeaderCells?.Remove( tableHeaderCell );
    }

    internal void AddTableRowHeader( TableRowHeader tableRowHeader )
    {
        if ( ParentTable.FixedColumnsPositionSync )
        {
            tableRowHeaders?.Add( tableRowHeader );
            RecalculateHashCode();
        }
        else
        {
            SetFixedCellPosition( tableRowHeader.Width, tableRowHeader.FixedPosition, tableRowHeader.SetFixedPositionStartOffset, tableRowHeader.IncreaseFixedPositionEndOffset );
        }
    }

    internal void RemoveTableRowHeader( TableRowHeader tableRowHeader )
    {
        tableRowHeaders?.Remove( tableRowHeader );
    }

    internal void AddTableRowCell( TableRowCell tableRowCell )
    {
        if ( ParentTable.FixedColumnsPositionSync )
        {
            tableRowCells?.Add( tableRowCell );
            RecalculateHashCode();
        }
        else
        {
            SetFixedCellPosition( tableRowCell.Width, tableRowCell.FixedPosition, tableRowCell.SetFixedPositionStartOffset, tableRowCell.IncreaseFixedPositionEndOffset );
        }
    }

    internal void RemoveTableRowCell( TableRowCell tableRowCell )
    {
        tableRowCells?.Remove( tableRowCell );
    }

    private void RecalculateHashCode()
    {
        elementHashCode = ElementId + tableHeaderCells.GetListHash() + tableRowCells.GetListHash() + tableRowHeaders.GetListHash();
    }

    internal double? GetTableHeaderCellFixedPositionStartOffset( TableHeaderCell tableHeaderCell )
    {
        double? fixedStartCellPosition = 0;
        foreach ( var headerCell in tableHeaderCells )
        {
            if ( tableHeaderCell == headerCell )
            {
                break;
            }
            if ( headerCell.FixedPosition == TableColumnFixedPosition.Start )
                fixedStartCellPosition += headerCell.Width?.FixedSize ?? 0;
        }
        return fixedStartCellPosition;
    }

    internal double? GetTableHeaderCellFixedPositionEndOffset( TableHeaderCell tableHeaderCell )
    {
        double? fixedEndCellPosition = 0;

        for ( int i = tableHeaderCells.Count - 1; i >= 0; i-- )
        {
            var headerCell = tableHeaderCells[i];

            if ( tableHeaderCell == headerCell )
            {
                break;
            }

            if ( headerCell.FixedPosition == TableColumnFixedPosition.End )
                fixedEndCellPosition += headerCell.Width?.FixedSize ?? 0;
        }

        return fixedEndCellPosition;
    }

    internal double? GetTableRowHeaderFixedPositionStartOffset( TableRowHeader tableRowHeader )
    {
        double? fixedStartCellPosition = 0;

        foreach ( var rowHeader in tableRowHeaders )
        {
            if ( tableRowHeader == rowHeader )
            {
                break;
            }

            if ( rowHeader.FixedPosition == TableColumnFixedPosition.Start )
                fixedStartCellPosition += rowHeader.Width?.FixedSize ?? 0;
        }

        return fixedStartCellPosition;
    }

    internal double? GetTableRowHeaderFixedPositionEndOffset( TableRowHeader tableRowHeader )
    {
        double? fixedEndCellPosition = 0;

        for ( int i = tableRowHeaders.Count - 1; i >= 0; i-- )
        {
            var rowHeader = tableRowHeaders[i];

            if ( tableRowHeader == rowHeader )
            {
                break;
            }

            if ( rowHeader.FixedPosition == TableColumnFixedPosition.End )
                fixedEndCellPosition += rowHeader.Width?.FixedSize ?? 0;
        }

        return fixedEndCellPosition;
    }

    internal double? GetTableRowCellFixedPositionStartOffset( TableRowCell tableRowCell )
    {
        double? fixedStartCellPosition = 0;

        foreach ( var rowCell in tableRowCells )
        {
            if ( tableRowCell == rowCell )
            {
                break;
            }

            if ( rowCell.FixedPosition == TableColumnFixedPosition.Start )
                fixedStartCellPosition += rowCell.Width?.FixedSize ?? 0;
        }

        return fixedStartCellPosition;
    }

    internal double? GetTableRowCellFixedPositionEndOffset( TableRowCell tableRowCell )
    {
        double? fixedEndCellPosition = 0;

        for ( int i = tableRowCells.Count - 1; i >= 0; i-- )
        {
            var rowCell = tableRowCells[i];

            if ( tableRowCell == rowCell )
            {
                break;
            }

            if ( rowCell.FixedPosition == TableColumnFixedPosition.End )
                fixedEndCellPosition += rowCell.Width?.FixedSize ?? 0;
        }

        return fixedEndCellPosition;
    }

    private void SetFixedCellPosition( IFluentSizing width, TableColumnFixedPosition fixedPosition, Action<double> cellFixedPositionStartUpdate, Action<double> cellFixedPositionEndUpdate )
    {
        var fixedWidth = width?.FixedSize ?? 0d;

        if ( fixedPosition == TableColumnFixedPosition.Start )
        {
            cellFixedPositionStartUpdate( fixedStartCellPosition );
            fixedStartCellPosition += fixedWidth;
        }

        if ( fixedPosition == TableColumnFixedPosition.End )
        {
            tableRowCellFixedPositionEndAddedHandlers ??= new();
            EventHandler<TableRowCellFixedPositionEndAddedEventArgs> handler = ( sender, args ) => cellFixedPositionEndUpdate( args.Width );

            TableRowCellFixedPositionEndAdded?.Invoke( this, new TableRowCellFixedPositionEndAddedEventArgs() { Width = fixedWidth } );
            TableRowCellFixedPositionEndAdded += handler;

            tableRowCellFixedPositionEndAddedHandlers.Add( handler );
        }
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing )
        {
            DisposeEventHandlers();
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            DisposeEventHandlers();
        }

        await base.DisposeAsync( disposing );
    }

    private void DisposeEventHandlers()
    {
        if ( !tableRowCellFixedPositionEndAddedHandlers.IsNullOrEmpty() && TableRowCellFixedPositionEndAdded is not null )
        {
            foreach ( var handler in tableRowCellFixedPositionEndAddedHandlers )
            {
                TableRowCellFixedPositionEndAdded -= handler;
            }

            tableRowCellFixedPositionEndAddedHandlers = null;
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the cascaded parent table component.
    /// </summary>
    [CascadingParameter] protected Table ParentTable { get; set; }

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    /// <summary>
    /// Gets or sets the row variant color.
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
    /// Gets or sets the row variant intent.
    /// </summary>
    [Parameter]
    public Intent Intent
    {
        get => Color.ToIntent();
        set => Color = value.ToColor();
    }

    /// <summary>
    /// Sets a table row as selected by appending "selected" modifier on a tr element.
    /// </summary>
    [Parameter]
    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Gets or sets the applied cursor when the row is hovered over.
    /// </summary>
    [Parameter]
    public Cursor HoverCursor
    {
        get => hoverCursor;
        set
        {
            hoverCursor = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Occurs when the row is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Occurs when the row is double clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> DoubleClicked { get; set; }

    /// <summary>
    /// Occurs when the row is mouse overed.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> MouseOver { get; set; }

    /// <summary>
    /// Occurs when the row is mouse leaved.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> MouseLeave { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="TableRow"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}