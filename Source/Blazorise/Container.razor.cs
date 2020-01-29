#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Container : BaseComponent
    {
        #region Members

        private bool fluid;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( Fluid )
                builder.Append( ClassProvider.ContainerFluid() );
            else
                builder.Append( ClassProvider.Container() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public bool Fluid
        {
            get => fluid;
            set
            {
                fluid = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
