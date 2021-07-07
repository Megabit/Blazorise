﻿#region Using directives
using System.Collections.Generic;
using System.Linq;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Placeholder for the list of <see cref="Validation"/> error messages.
    /// </summary>
    public partial class ValidationSummary : BaseComponent
    {
        #region Members

        private Validations previousParentValidations;

        private readonly ValidationsStatusChangedEventHandler validationsStatusChangedEventHandler;

        private IReadOnlyCollection<string> internalErrorMessages;

        #endregion

        #region Constructors

        /// <summary>
        /// A default <see cref="ValidationSummary"/> constructor.
        /// </summary>
        public ValidationSummary()
        {
            ErrorClassBuilder = new( BuildErrorClasses );

            validationsStatusChangedEventHandler += async ( eventArgs ) =>
            {
                OnValidationsStatusChanged( eventArgs );
                await InvokeAsync( StateHasChanged );
            };
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ValidationSummary() );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Builds the classnames for a summary placeholder.
        /// </summary>
        /// <param name="builder">Class builder used to append the classnames.</param>
        private void BuildErrorClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ValidationSummaryError() );
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                DetachAllListener();
            }

            base.Dispose( disposing );
        }

        /// <inheritdoc/>
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
            internalErrorMessages = eventArgs.Messages;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Summary placeholder class builder.
        /// </summary>
        protected ClassBuilder ErrorClassBuilder { get; private set; }

        /// <summary>
        /// Gets the classnames for the summary placeholder.
        /// </summary>
        protected string ErrorClassNames => ErrorClassBuilder.Class;

        /// <summary>
        /// True if any error message has received.
        /// </summary>
        protected bool HasErrorMessages
            => internalErrorMessages?.Count > 0 || Errors?.Count() > 0;

        /// <summary>
        /// Gets the list of error messages.
        /// </summary>
        protected IEnumerable<string> ErrorMessages
            => ( internalErrorMessages ?? Enumerable.Empty<string>().ToList() ).Concat( Errors ?? Enumerable.Empty<string>() );

        /// <summary>
        /// Label showed before the error messages.
        /// </summary>
        [Parameter] public string Label { get; set; }

        /// <summary>
        /// List of custom error messages for the validations summary.
        /// </summary>
        [Parameter] public string[] Errors { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="ValidationSummary"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the reference to the parent <see cref="Validations"/> component.
        /// </summary>
        [CascadingParameter] protected Validations ParentValidations { get; set; }

        #endregion
    }
}
