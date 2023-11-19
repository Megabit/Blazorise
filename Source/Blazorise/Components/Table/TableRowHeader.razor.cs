#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Defines a cell as header of a group of table cells.
/// </summary>
public partial class TableRowHeader : BaseDraggableComponent
{
    #region Members

    private double? fixedPositionOffset;

    private double? fixedPositionEndOffset;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        if ( ParentTable is not null )
        {
            ParentTableRow.AddTableRowHeader( this );

            if ( FixedPosition != TableColumnFixedPosition.None )
            {
                fixedPositionOffset = ParentTableRow.GetFixedCellPosition();
            }
        }

        base.OnInitialized();
    }

    internal void IncreaseFixedPositionEndOff( double addPixels )
    {
        if ( fixedPositionEndOffset.HasValue )
            fixedPositionEndOffset += addPixels;
        else
            fixedPositionEndOffset = addPixels;
        DirtyStyles();
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
        if ( FixedPosition == TableColumnFixedPosition.Start && fixedPositionOffset != null )
        {
            builder.Append( $"left:{fixedPositionOffset:G29}px" );
        }

        if ( FixedPosition == TableColumnFixedPosition.End && fixedPositionEndOffset != null )
        {
            builder.Append( $"right:{fixedPositionEndOffset:G29}px" );
        }

        base.BuildStyles( builder );
    }

    /// <summary>
    /// Handles the row onclick event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task ClickHandler( MouseEventArgs eventArgs )
    {
        return Clicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( eventArgs ) );
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

    #endregion
}