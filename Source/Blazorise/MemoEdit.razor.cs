#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseMemo : BaseInputComponent<string>
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.MemoEdit() );
            builder.Append( ClassProvider.MemoEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        protected void HandleOnChange( UIChangeEventArgs e )
        {
            if ( !Options.ChangeTextOnKeyPress )
            {
                HandleText( e?.Value?.ToString() );
            }
        }

        protected void HandleOnInput( UIChangeEventArgs e )
        {
            if ( Options.ChangeTextOnKeyPress )
            {
                HandleText( e?.Value?.ToString() );
            }
        }

        protected void HandleText( string text )
        {
            Text = text;
            TextChanged?.Invoke( Text );
        }

        #endregion

        #region Properties

        [Inject] protected BlazoriseOptions Options { get; set; }

        /// <summary>
        /// Sets the placeholder for the empty text.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }

        /// <summary>
        /// Gets or sets the text inside the input field.
        /// </summary>
        [Parameter] public string Text { get => InternalValue; set => InternalValue = value; }

        /// <summary>
        /// Occurs after text has changed.
        /// </summary>
        [Parameter] public Action<string> TextChanged { get; set; }

        /// <summary>
        /// Specifies the maximum number of characters allowed in the input element.
        /// </summary>
        [Parameter] public int? MaxLength { get; set; }

        /// <summary>
        /// Specifies the number lines in the input element.
        /// </summary>
        [Parameter] public int? Rows { get; set; }

        #endregion
    }
}
