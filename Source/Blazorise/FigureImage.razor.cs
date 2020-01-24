#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class FigureImage : BaseComponent
    {
        #region Members

        private bool isRounded;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.FigureImage() );
            builder.Append( ClassProvider.FigureImageRounded(), IsRounded );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        [Parameter] public string Source { get; set; }

        [Parameter] public string AlternateText { get; set; }

        [Parameter]
        public bool IsRounded
        {
            get => isRounded;
            set
            {
                isRounded = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
