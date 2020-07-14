#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class DisplayHeading : BaseComponent
    {
        #region Members

        private DisplayHeadingSize displayHeadingSize = DisplayHeadingSize.Is2;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.DisplayHeadingSize( Size ) );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter]
        public DisplayHeadingSize Size
        {
            get => displayHeadingSize;
            set
            {
                displayHeadingSize = value;

                DirtyClasses();
            }
        }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
