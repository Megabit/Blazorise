#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseValidations : ComponentBase
    {
        #region Members

        /// <summary>
        /// Raises an intent to validate all validations inside of this container.
        /// </summary>
        public event ValidatingAllEventHandler ValidatingAll;

        public event ValidatedAllEventHandler ValidatedAll;

        public event ClearAllValidatinaEventHandler ClearingAll;

        #endregion

        #region Methods

        /// <summary>
        /// Runs the validation process for all validations and returns false if any is failed.
        /// </summary>
        public bool ValidateAll()
        {
            var result = TryValidateAll();

            if ( result )
            {
                ValidatedAll?.Invoke();
            }

            return result;
        }

        /// <summary>
        /// Clears all validation statuses.
        /// </summary>
        public void ClearAll()
        {
            ClearingAll?.Invoke();
        }

        private bool TryValidateAll()
        {
            var validated = true;

            var handler = ValidatingAll;

            if ( handler != null )
            {
                var args = new ValidatingAllEventArgs( false );

                foreach ( ValidatingAllEventHandler subHandler in handler?.GetInvocationList() )
                {
                    subHandler( args );

                    if ( args.Cancel )
                    {
                        validated = false;
                    }
                }
            }

            return validated;
        }

        #endregion

        #region Properties        

        /// <summary>
        /// Defines the validation mode for validations inside of this container.
        /// </summary>
        [Parameter] public ValidationMode Mode { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
