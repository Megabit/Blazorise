#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
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

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.TableRowCell() )
                .If( () => ClassProvider.TableRowCellColor( Color ), () => Color != Color.None )
                .If( () => ClassProvider.TableRowCellBackground( Background ), () => Background != Background.None )
                .If( () => ClassProvider.TableRowCellTextColor( TextColor ), () => TextColor != TextColor.None );

            base.RegisterClasses();
        }

        protected void HandleClick( UIMouseEventArgs e )
        {
            Clicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( e ) );
        }

        #endregion

        #region Properties

        [Parameter]
        protected Color Color
        {
            get => color;
            set
            {
                color = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected Background Background
        {
            get => background;
            set
            {
                background = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter]
        protected TextColor TextColor
        {
            get => textColor;
            set
            {
                textColor = value;

                ClassMapper.Dirty();
            }
        }

        [Parameter] protected int? RowSpan { get; set; }

        [Parameter] protected int? ColumnSpan { get; set; }

        /// <summary>
        /// Occurs when the row cell is clicked.
        /// </summary>
        [Parameter] protected EventCallback<MouseEventArgs> Clicked { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
