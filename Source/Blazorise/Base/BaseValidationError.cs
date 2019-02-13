#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseValidationError : BaseComponent
    {
        #region Members

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.ValidationError() );

            base.RegisterClasses();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentValidation != null )
                {
                    ParentValidation.ValidationFailed -= OnValidationFailed;
                }
            }

            base.Dispose( disposing );
        }

        protected override void OnAfterRender()
        {
            if ( ParentValidation != null )
            {
                ParentValidation.ValidationFailed += OnValidationFailed;
            }

            base.OnAfterRender();
        }

        private void OnValidationFailed( ValidationFailedEventArgs e )
        {
            ErrorText = e.ErrorText;
        }

        #endregion

        #region Properties

        [CascadingParameter] protected BaseValidation ParentValidation { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Custom error type that will override default content.
        /// </summary>
        [Parameter] protected string ErrorText { get; set; }

        #endregion
    }
}
