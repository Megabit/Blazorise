#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    public partial class TableRow : BaseComponent
    {
        #region Members

        private Color color = Color.None;

        private Background background = Background.None;

        private TextColor textColor = TextColor.None;

        private bool selected;

        private Cursor hoverCursor;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableRow() );
            builder.Append( ClassProvider.TableRowColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.TableRowBackground( Background ), Background != Background.None );
            builder.Append( ClassProvider.TableRowTextColor( TextColor ), TextColor != TextColor.None );
            builder.Append( ClassProvider.TableRowIsSelected(), Selected );
            builder.Append( ClassProvider.TableRowHoverCursor(), HoverCursor != Cursor.Default );

            base.BuildClasses( builder );
        }

        protected async Task ClickHandler( MouseEventArgs e )
        {
            // https://stackoverflow.com/questions/5497073/how-to-differentiate-single-click-event-and-double-click-event
            // works good enough. Click is still called before the double click, but it is advise to not use both events anyway.
            if ( e.Detail == 1 )
                await Clicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( e ) );
            else if ( e.Detail == 2 )
                await DoubleClicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( e ) );
        }

        #endregion

        #region Properties

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
        /// Sets a table row as selected by appending "selected" modifier on a <tr>.
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

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
