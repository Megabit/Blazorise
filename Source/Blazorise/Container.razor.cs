#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseContainer : BaseComponent
    {
        #region Members

        private bool isFluid;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( IsFluid )
                builder.Append( ClassProvider.ContainerFluid() );
            else
                builder.Append( ClassProvider.Container() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public bool IsFluid
        {
            get => isFluid;
            set
            {
                isFluid = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
