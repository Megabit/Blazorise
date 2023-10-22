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

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TableRowHeader() );
        builder.Append( ClassProvider.TableRowHeaderFixed( Fixed ) );

        base.BuildClasses( builder );
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
    /// Number of rows a cell should span.
    /// </summary>
    [Parameter] public int? RowSpan { get; set; }

    /// <summary>
    /// Number of columns a cell should span.
    /// </summary>
    [Parameter] public int? ColumnSpan { get; set; }

    /// <summary>
    /// Fixes the row header to the start of the table.
    /// </summary>
    [Parameter] public bool Fixed { get; set; }

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