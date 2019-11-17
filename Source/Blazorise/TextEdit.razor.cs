#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    public abstract class BaseTextEdit : BaseTextInput<string>
    {
        #region Members

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentValidation != null )
            {
                ParentValidation.InitializeInputExpression( TextExpression );
            }

            base.OnInitialized();
        }

        protected async override Task OnFirstAfterRenderAsync()
        {
            await JSRunner.InitializeTextEdit( ElementId, ElementRef, MaskType.ToMaskTypeString(), EditMask );

            await base.OnFirstAfterRenderAsync();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                JSRunner.DestroyTextEdit( ElementId, ElementRef );
            }

            base.Dispose( disposing );
        }

        protected override void OnInternalValueChanged( string value )
        {
            TextChanged.InvokeAsync( value );
        }

        protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
        {
            return Task.FromResult( new ParseValue<string>( true, value, null ) );
        }

        #endregion

        #region Properties

        protected string Type => Role.ToTextRoleString();

        protected string Mode => InputMode.ToTextInputMode();

        protected override string InternalValue { get => Text; set => Text = value; }

        /// <summary>
        /// Sets the role of the input text.
        /// </summary>
        [Parameter] public TextRole Role { get; set; } = TextRole.Text;

        [Parameter] public TextInputMode InputMode { get; set; } = TextInputMode.None;

        /// <summary>
        /// Gets or sets the text inside the input field.
        /// </summary>
        [Parameter] public string Text { get; set; }

        /// <summary>
        /// Occurs after text has changed.
        /// </summary>
        [Parameter] public EventCallback<string> TextChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the text value.
        /// </summary>
        [Parameter] public Expression<Func<string>> TextExpression { get; set; }

        /// <summary>
        /// Occurs when a key is pressed down while the control has focus.
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> KeyDown { get; set; }

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> KeyPress { get; set; }

        /// <summary>
        /// Occurs when a key is released while the control has focus.
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> KeyUp { get; set; }

        /// <summary>
        /// A string representing a edit mask expression.
        /// </summary>
        [Parameter] public string EditMask { get; set; }

        /// <summary>
        /// Specify the mask type used by the editor.
        /// </summary>
        [Parameter] public MaskType MaskType { get; set; }

        /// <summary>
        /// Specifies the maximum number of characters allowed in the input element.
        /// </summary>
        [Parameter] public int? MaxLength { get; set; }

        #endregion
    }
}
