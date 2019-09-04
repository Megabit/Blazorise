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
    public abstract class BaseTableRow : BaseComponent
    {
        #region Members

        private Color color = Color.None;

        private Background background = Background.None;

        private TextColor textColor = TextColor.None;

        private bool selected;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableRow() );
            builder.Append( ClassProvider.TableRowColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.TableRowBackground( Background ), Background != Background.None );
            builder.Append( ClassProvider.TableRowTextColor( TextColor ), TextColor != TextColor.None );
            builder.Append( ClassProvider.TableRowIsSelected(), IsSelected );

            base.BuildClasses( builder );
        }

        protected void HandleClick( UIMouseEventArgs e )
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

        /// <summary>
        /// Sets a table row as selected by appending "selected" modifier on a <tr>.
        /// </summary>
        [Parameter]
        public bool IsSelected
        {
            get => selected;
            set
            {
                selected = value;

                Dirty();
            }
        }

        /// <summary>
        /// Occurs when the row is clicked.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> Clicked { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
