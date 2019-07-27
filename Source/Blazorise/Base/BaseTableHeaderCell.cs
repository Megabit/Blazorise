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
    public abstract class BaseTableHeaderCell : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.TableHeaderCell() );

            base.RegisterClasses();
        }

        protected void HandleClick( UIMouseEventArgs e )
        {
            Clicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( e ) );
        }

        #endregion

        #region Properties

        [Parameter] protected int? RowSpan { get; set; }

        [Parameter] protected int? ColumnSpan { get; set; }

        /// <summary>
        /// Occurs when the header cell is clicked.
        /// </summary>
        [Parameter] protected EventCallback<MouseEventArgs> Clicked { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
