#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseCardLink : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.CardLink() );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Link url.
        /// </summary>
        [Parameter] protected string Source { get; set; }

        /// <summary>
        /// Alternative link text.
        /// </summary>
        [Parameter] protected string Alt { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
