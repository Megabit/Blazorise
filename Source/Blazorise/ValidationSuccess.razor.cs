﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class ValidationSuccess : BaseValidationResult
    {
        #region Members

        private bool tooltip;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( !Tooltip )
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
        public bool Tooltip
        {
            get => tooltip;
            set
            {
                tooltip = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
