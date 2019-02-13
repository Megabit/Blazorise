#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseValidations : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        public void Validate()
        {
            ManualValidation?.Invoke();
        }

        #endregion

        #region Properties        

        [Parameter] protected internal ValidationMode Mode { get; set; }

        public event ManualValidationEventHandler ManualValidation;

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
