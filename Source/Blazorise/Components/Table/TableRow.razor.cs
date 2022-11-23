#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Component defines a row of cells in a table. The row's cells can then be established using a mix of <see cref="TableRowCell"/> (data cell) components.
    /// </summary>
    public partial class TableRow : BaseDraggableComponent
    {
        #region Members

        private Color color = Color.Default;

        private bool selected;

        private Cursor hoverCursor;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableRow() );
            builder.Append( ClassProvider.TableRowColor( Color ), Color != Color.Default );
            builder.Append( ClassProvider.TableRowIsSelected(), Selected );
            builder.Append( ClassProvider.TableRowHoverCursor(), HoverCursor != Cursor.Default );

            base.BuildClasses( builder );
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
                return Clicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( eventArgs ) );
            else if ( eventArgs.Detail == 2 )
                return DoubleClicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( eventArgs ) );
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the row mouse leave event.
        /// </summary>
        /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task OnMouseLeaveHandler(MouseEventArgs eventArgs )
        {
            return MouseLeave.InvokeAsync( EventArgsMapper.ToMouseEventArgs( eventArgs ) );
        }

        /// <summary>
        /// Handles the row mouseover event.
        /// </summary>
        /// <param name="eventArgs">Supplies information about a mouse event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task OnMouseOverHandler(MouseEventArgs eventArgs )
        {
            return MouseOver.InvokeAsync( EventArgsMapper.ToMouseEventArgs( eventArgs ) );
        }

        #endregion

        #region Properties

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
        [Parameter] public EventCallback<BLMouseEventArgs> Clicked { get; set; }

        /// <summary>
        /// Occurs when the row is double clicked.
        /// </summary>
        [Parameter] public EventCallback<BLMouseEventArgs> DoubleClicked { get; set; }

        /// <summary>
        /// Occurs when the row is mouse overed.
        /// </summary>
        [Parameter] public EventCallback<BLMouseEventArgs> MouseOver { get; set; }

        /// <summary>
        /// Occurs when the row is mouse leaved.
        /// </summary>
        [Parameter] public EventCallback<BLMouseEventArgs> MouseLeave { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="TableRow"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
