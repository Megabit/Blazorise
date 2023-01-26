#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// The <see cref="TableRowGroup"/> component is used to group rows in a table.
/// </summary>
public partial class TableRowGroup : BaseDraggableComponent
{
    #region Members

    private bool expanded;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="TableRowGroup"/> constructor.
    /// </summary>
    public TableRowGroup()
    {
        RowCellClassBuilder = new( BuildRowCellClasses );
        RowIndentCellClassBuilder = new( BuildRowIndentCellClasses );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TableRowGroup( Expanded ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds the classnames for a cell.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    private void BuildRowCellClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TableRowGroupCell() );
    }

    /// <summary>
    /// Builds the classnames for an indent cell.
    /// </summary>
    /// <param name="builder">Class builder used to append the classnames.</param>
    private void BuildRowIndentCellClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.TableRowGroupIndentCell() );
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        RowCellClassBuilder.Dirty();
        RowIndentCellClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <summary>
    /// Handles the row clicked event.
    /// </summary>
    /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task OnClickHandler( MouseEventArgs eventArgs )
    {
        // https://stackoverflow.com/questions/5497073/how-to-differentiate-single-click-event-and-double-click-event
        // works good enough. Click is still called before the double click, but it is advise to not use both events anyway.
        // We'll be treating any Detail higher then 2 as the user constantly clicking, therefore triggering Single Click.
        if ( eventArgs.Detail == 1 || eventArgs.Detail > 2 )
        {
            if ( Toggleable )
            {
                Expanded = !Expanded;
                await ExpandedChanged.InvokeAsync( Expanded );
            }

            await Clicked.InvokeAsync( eventArgs );
        }
        else if ( eventArgs.Detail == 2 )
            await DoubleClicked.InvokeAsync( eventArgs );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Row cell class builder.
    /// </summary>
    protected ClassBuilder RowCellClassBuilder { get; private set; }

    /// <summary>
    /// Row indent cell class builder.
    /// </summary>
    protected ClassBuilder RowIndentCellClassBuilder { get; private set; }

    /// <summary>
    /// Gets or sets the cascaded parent table component.
    /// </summary>
    [CascadingParameter] protected Table ParentTable { get; set; }

    /// <summary>
    /// Defines if the group is expanded.
    /// </summary>
    [Parameter]
    public bool Expanded
    {
        get => expanded;
        set
        {
            if ( expanded == value )
                return;

            expanded = value;

            DirtyClasses();
        }
    }

    /// <summary>
    /// Defines if the group is expanded.
    /// </summary>
    [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }

    /// <summary>
    /// Defines if the group <see cref="Expanded"/> property can be toggled by clicking. It is still possible to toggle it programatically.
    /// </summary>
    [Parameter]
    public bool Toggleable { get; set; } = true;

    /// <summary>
    /// Determines the column span for the inner table cell.
    /// </summary>
    [Parameter] public int ColumnSpan { get; set; } = 1000;

    /// <summary>
    /// Occurs when the row is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

    /// <summary>
    /// Occurs when the row is double clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> DoubleClicked { get; set; }

    /// <summary>
    /// Specifies the title to be rendered inside this <see cref="TableRowGroup"/>.
    /// </summary>
    [Parameter] public string Title { get; set; }

    /// <summary>
    /// Specifies the custom title content to be rendered inside this <see cref="TableRowGroup"/>. It has higher priority over the <see cref="Title"/> parameter.
    /// </summary>
    [Parameter] public RenderFragment TitleTemplate { get; set; }

    /// <summary>
    /// Specifies the content to be rendered inside this <see cref="TableRow"/>.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Specifies a number of Table Cells to Indent on the Group Row.
    /// </summary>
    [Parameter] public int IndentTableCells { get; set; }

    /// <summary>
    /// Specifies the custom content inside an Indented Table Cell on the Group Row. A contextual index is provided according to the Indentation.
    /// </summary>
    [Parameter] public RenderFragment<int> IndentTableCellTemplate { get; set; }

    #endregion
}