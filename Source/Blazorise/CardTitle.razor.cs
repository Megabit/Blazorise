#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseCardTitle : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.CardTitle() );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Number from 1 to 6 that defines the title size where the smaller number means larger text.
        /// </summary>
        /// <remarks>
        /// todo: change to enum
        /// </remarks>
        [Parameter] public int? Size { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
