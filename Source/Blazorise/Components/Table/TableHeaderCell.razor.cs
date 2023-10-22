#region Using directives
using System.Threading.Tasks;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// Defines a cell as header of a group of table cells.
/// </summary>
public partial class TableHeaderCell : BaseDraggableComponent
{
    #region Members

    private Cursor cursor;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TableHeaderCell() );
        builder.Append( ClassProvider.TableHeaderCellCursor( Cursor ) );
        builder.Append( ClassProvider.TableHeaderCellFixed( Fixed ) );

        base.BuildClasses( builder );
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

    #endregion

    #region Properties

    /// <summary>
    /// Number of rows a cell should span.
    /// </summary>
    [Parameter] public int? RowSpan { get; set; }

    /// <summary>
    /// Number of columns a cell should span.
    /// </summary>
    [Parameter] public int? ColumnSpan { get; set; }

    /// <summary>
    /// Fixes the header cell to the start of the table.
    /// </summary>
    [Parameter] public bool Fixed { get; set; }

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
    [Parameter] public EventCallback<BLMouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="TableHeaderCell"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}