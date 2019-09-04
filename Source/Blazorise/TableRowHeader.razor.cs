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
    public abstract class BaseTableRowHeader : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.TableRowHeader() );

            base.RegisterClasses();
        }

        protected void HandleClick( MouseEventArgs e )
        {
            Clicked.InvokeAsync( EventArgsMapper.ToMouseEventArgs( e ) );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Occurs when the row header is clicked.
        /// </summary>
        [Parameter] public EventCallback<MyMouseEventArgs> Clicked { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
