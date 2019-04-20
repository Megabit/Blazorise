#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
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

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.TableRow() )
                .If( () => ClassProvider.TableRowColor( Color ), () => Color != Color.None )
                .If( () => ClassProvider.TableRowBackground( Background ), () => Background != Background.None )
                .If( () => ClassProvider.TableRowTextColor( TextColor ), () => TextColor != TextColor.None )
                .If( () => ClassProvider.TableRowIsSelected(), () => IsSelected );

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

        /// <summary>
        /// Sets a table row as selected by appending "selected" modifier on a <tr>.
        /// </summary>
        [Parameter]
        protected bool IsSelected
        {
            get => selected;
            set
            {
                selected = value;

                ClassMapper.Dirty();
            }
        }

        /// <summary>
        /// Occurs when the row is clicked.
        /// </summary>
        [Parameter] protected EventCallback<MouseEventArgs> Clicked { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
