#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Primitives;
#endregion

namespace Blazorise
{
    public partial class ValidationSummary : BaseComponent
    {
        #region Members

        private Validations previousParentValidations;

        private readonly ValidationsStatusChangedEventHandler validationsStatusChangedEventHandler;

        private IReadOnlyCollection<string> errorMessages;

        #endregion

        #region Constructors

        public ValidationSummary()
        {
            ErrorClassBuilder = new ClassBuilder( BuildErrorClasses );

            validationsStatusChangedEventHandler += ( eventArgs ) =>
            {
                OnValidationsStatusChanged( eventArgs );
                StateHasChanged();
            };
        }

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ValidationSummary() );

            base.BuildClasses( builder );
        }

        private void BuildErrorClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ValidationSummaryError() );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                DetachAllListener();
            }

            base.Dispose( disposing );
        }

        protected override void OnParametersSet()
        {
            if ( ParentValidations != previousParentValidations )
            {
                DetachAllListener();

                ParentValidations._StatusChanged += validationsStatusChangedEventHandler;

                previousParentValidations = ParentValidations;
            }
        }

        private void DetachAllListener()
        {
            if ( previousParentValidations != null )
            {
                previousParentValidations._StatusChanged -= validationsStatusChangedEventHandler;
            }
        }

        private void OnValidationsStatusChanged( ValidationsStatusChangedEventArgs eventArgs )
        {
            errorMessages = eventArgs.Messages;
        }

        #endregion

        #region Properties

        protected ClassBuilder ErrorClassBuilder { get; private set; }

        protected string ErrorClassNames => ErrorClassBuilder.Class;

        protected bool HasErrorMessages
            => errorMessages?.Count > 0;

        /// <summary>
        /// Gets the list of error messages.
        /// </summary>
        protected IReadOnlyCollection<string> ErrorMessages
            => errorMessages ?? Enumerable.Empty<string>().ToList();

        /// <summary>
        /// Label showed before the error messages.
        /// </summary>
        [Parameter] public string Label { get; set; }

        [CascadingParameter] protected Validations ParentValidations { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
