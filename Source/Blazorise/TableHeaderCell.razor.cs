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
    public partial class TableHeaderCell : BaseComponent
    {
        #region Members

        private TextAlignment textAlignment = TextAlignment.None;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableHeaderCell() );
            builder.Append( ClassProvider.TableHeaderCellTextAlignment( TextAlignment ), TextAlignment != TextAlignment.None );

            base.BuildClasses( builder );
        }

        protected Task ClickHandler( MouseEventArgs e )
        {
            return Clicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( e ) );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text alignment.
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
        /// Occurs when the header cell is clicked.
        /// </summary>
        [Parameter] public EventCallback<BLMouseEventArgs> Clicked { get; set; }

        /// <summary>
        /// Gets or sets the component child content.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
