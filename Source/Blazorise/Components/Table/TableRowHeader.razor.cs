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
        if ( FixedPosition == TableColumnFixedPosition.Start && fixedPositionStartOffset.HasValue )
        {
            builder.Append( $"left:{fixedPositionStartOffset:G29}px" );
        }

        if ( FixedPosition == TableColumnFixedPosition.End && fixedPositionEndOffset.HasValue )
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

    #endregion

    #region Properties

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
    /// When the <see cref="Table.ResponsiveMode"/> is set to <see cref="TableResponsiveMode.Mobile"/>, this title will be used for the row.
    /// </summary>
    [Parameter] public string MobileModeCaption { get; set; }

    #endregion
}