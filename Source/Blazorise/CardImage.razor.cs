#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseCardImage : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.CardImage() );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Image url.
        /// </summary>
        [Parameter] protected string Source { get; set; }

        /// <summary>
        /// Alternative image text.
        /// </summary>
        [Parameter] protected string Alt { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
