#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseValidations : ComponentBase
    {
        #region Members

        public event ValidatingAllEventHandler ValidatingAll;

        #endregion

        #region Methods

        /// <summary>
        /// Runs the validation process for all validations.
        /// </summary>
        public void ValidateAll()
        {
            ValidatingAll?.Invoke();
        }

        #endregion

        #region Properties        

        /// <summary>
        /// Defines the validation mode for validations inside of this container.
        /// </summary>
        [Parameter] protected internal ValidationMode Mode { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
