#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseValidationSuccess : BaseValidationSummary
    {
        #region Members

        private bool isTooltip;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( !IsTooltip )
                builder.Append( ClassProvider.ValidationSuccess() );
            else
                builder.Append( ClassProvider.ValidationSuccessTooltip() );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Shows the tooltip instead of label.
        /// </summary>
        [Parameter]
        public bool IsTooltip
        {
            get => isTooltip;
            set
            {
                isTooltip = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
