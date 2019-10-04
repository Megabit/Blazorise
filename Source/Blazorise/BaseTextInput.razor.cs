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
    /// <summary>
    /// Base component for inputs that are text-based.
    /// </summary>
    /// <typeparam name="TValue">Editable value type.</typeparam>
    public abstract class BaseTextInput<TValue> : BaseInputComponent<TValue>
    {
        #region Members

        private Color color;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TextEdit( IsPlaintext ) );
            builder.Append( ClassProvider.TextEditColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.TextEditSize( Size ), Size != Size.None );
            builder.Append( ClassProvider.TextEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        protected override void OnInitialized()
        {
            if ( ParentValidation != null )
            {
                ParentValidation.InitInputPattern( Pattern );
            }

            base.OnInitialized();
        }

        protected Task OnChangeHandler( ChangeEventArgs e )
        {
            if ( !Options.ChangeTextOnKeyPress )
            {
                return CurrentValueHandler( e?.Value?.ToString() );
            }

            return Task.CompletedTask;
        }

        protected Task OnInputHandler( ChangeEventArgs e )
        {
            if ( Options.ChangeTextOnKeyPress )
            {
                return CurrentValueHandler( e?.Value?.ToString() );
            }

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        [Inject] protected BlazoriseOptions Options { get; set; }

        /// <summary>
        /// Sets the placeholder for the empty text.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }

        /// <summary>
        /// Sets the class to remove the default form field styling and preserve the correct margin and padding.
        /// </summary>
        [Parameter] public bool IsPlaintext { get; set; }

        /// <summary>
        /// Sets the input text color.
        /// </summary>
        [Parameter]
        public Color Color
        {
            get => color;
            set
            {
                color = value;

                DirtyClasses();
                DirtyClasses();
            }
        }

        /// <summary>
        /// The pattern attribute specifies a regular expression that the input element's value is checked against on form validation.
        /// </summary>
        [Parameter] public string Pattern { get; set; }

        #endregion
    }
}
