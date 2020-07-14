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

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TableHeaderCell() );

            base.BuildClasses( builder );
        }

        protected Task ClickHandler( MouseEventArgs e )
        {
            return Clicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( e ) );
        }

        #endregion

        #region Properties

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

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
