#region Using directives
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Defines a cell of a table that contains data.
    /// </summary>
    public partial class TableRowCell : BaseDraggableComponent
    {
        #region Members

        private Color color = Color.None;

        private Background background = Background.None;

        private TextColor textColor = TextColor.None;

        private TextAlignment textAlignment = TextAlignment.None;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableRowCell() );
            builder.Append( ClassProvider.TableRowCellColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.TableRowCellBackground( Background ), Background != Background.None );
            builder.Append( ClassProvider.TableRowCellTextColor( TextColor ), TextColor != TextColor.None );
            builder.Append( ClassProvider.TableRowCellTextAlignment( TextAlignment ), TextAlignment != TextAlignment.None );

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
        /// Gets or sets the cell background color.
        /// </summary>
        [Parameter]
        public Background Background
        {
            get => background;
            set
            {
                background = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the cell text color.
        /// </summary>
        [Parameter]
        public TextColor TextColor
        {
            get => textColor;
            set
            {
                textColor = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the cell text alignment.
        /// </summary>
        [Parameter]
        public TextAlignment TextAlignment
        {
            get => textAlignment;
            set
            {
                textAlignment = value;

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
        /// Occurs when the row cell is clicked.
        /// </summary>
        [Parameter] public EventCallback<BLMouseEventArgs> Clicked { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="TableRowCell"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
