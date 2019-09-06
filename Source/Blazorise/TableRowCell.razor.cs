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
    public abstract class BaseTableRowCell : BaseComponent
    {
        #region Members

        private Color color = Color.None;

        private Background background = Background.None;

        private TextColor textColor = TextColor.None;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableRowCell() );
            builder.Append( ClassProvider.TableRowCellColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.TableRowCellBackground( Background ), Background != Background.None );
            builder.Append( ClassProvider.TableRowCellTextColor( TextColor ), TextColor != TextColor.None );

            base.BuildClasses( builder );
        }

        protected void HandleClick( MouseEventArgs e )
        {
            Clicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( e ) );
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

                Dirty();
            }
        }

        [Parameter]
        public Background Background
        {
            get => background;
            set
            {
                background = value;

                Dirty();
            }
        }

        [Parameter]
        public TextColor TextColor
        {
            get => textColor;
            set
            {
                textColor = value;

                Dirty();
            }
        }

        [Parameter] public int? RowSpan { get; set; }

        [Parameter] public int? ColumnSpan { get; set; }

        /// <summary>
        /// Occurs when the row cell is clicked.
        /// </summary>
        [Parameter] public EventCallback<BLMouseEventArgs> Clicked { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
