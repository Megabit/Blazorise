#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseValidationSuccess : BaseComponent
    {
        #region Members

        private bool isTooltip;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .If( () => ClassProvider.ValidationSuccess(), () => !IsTooltip )
                .If( () => ClassProvider.ValidationSuccessTooltip(), () => IsTooltip );

            base.RegisterClasses();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Shows the tooltip instead of label.
        /// </summary>
        [Parameter]
        protected bool IsTooltip
        {
            get => isTooltip;
            set
            {
                isTooltip = value;

                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] protected BaseValidation ParentValidation { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
