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
    public partial class TableRowCell : BaseComponent
    {
        #region Members

        private Color color = Color.None;

        private Background background = Background.None;

        private TextColor textColor = TextColor.None;

        private TextAlignment textAlignment = TextAlignment.None;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableRowCell() );
            builder.Append( ClassProvider.TableRowCellColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.TableRowCellBackground( Background ), Background != Background.None );
            builder.Append( ClassProvider.TableRowCellTextColor( TextColor ), TextColor != TextColor.None );
            builder.Append( ClassProvider.TableRowCellTextAlignment( TextAlignment ), TextAlignment != TextAlignment.None );

            base.BuildClasses( builder );
        }

        protected Task ClickHandler( MouseEventArgs e )
        {
            return Clicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( e ) );
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

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
