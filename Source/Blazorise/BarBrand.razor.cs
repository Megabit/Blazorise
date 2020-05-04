#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class BarBrand : BaseComponent
    {
        #region Members

        private BarMode mode;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.BarBrand( Mode ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [CascadingParameter( Name = "Mode" )] protected BarMode Mode
        {
            get => mode;
            set
            {
                mode = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
