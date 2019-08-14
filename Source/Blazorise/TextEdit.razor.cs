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
    public abstract class BaseTextEdit : BaseTextInput<string>
    {
        #region Members

        #endregion

        #region Methods

        // implementation according to the response on https://github.com/aspnet/AspNetCore/issues/7898#issuecomment-479863699
        protected override void OnInitialized()
        {
            internalValue = Text;

            base.OnInitialized();
        }

        protected async override Task OnFirstAfterRenderAsync()
        {
            await JSRunner.InitializeTextEdit( ElementId, ElementRef, MaskType.ToMaskTypeString(), EditMask );

            await base.OnFirstAfterRenderAsync();
        }

        public override void Dispose()
        {
            JSRunner.DestroyTextEdit( ElementId, ElementRef );

            base.Dispose();
        }

        public override Task SetParametersAsync( ParameterView parameters )
        {
            // This is needed for the two-way binding to work properly.
            // Otherwise the internal value would not be set.
            if ( parameters.TryGetValue<string>( nameof( Text ), out var newText ) )
            {
                internalValue = newText;
            }

            return base.SetParametersAsync( parameters );
        }

        protected override Task HandleValue( object value )
        {
            InternalValue = value?.ToString();
            return TextChanged.InvokeAsync( InternalValue );
        }

        #endregion

        #region Properties

        protected string Type => Role.ToTextRoleString();

        protected string Mode => InputMode.ToTextInputMode();

        /// <summary>
        /// Sets the role of the input text.
        /// </summary>
        [Parameter] protected TextRole Role { get; set; } = TextRole.Text;

        [Parameter] protected TextInputMode InputMode { get; set; } = TextInputMode.None;

        /// <summary>
        /// Gets or sets the text inside the input field.
        /// </summary>
        [Parameter] protected string Text { get; set; }

        /// <summary>
        /// Occurs after text has changed.
        /// </summary>
        [Parameter] protected EventCallback<string> TextChanged { get; set; }

        /// <summary>
        /// Occurs when a key is pressed down while the control has focus.
        /// </summary>
        [Parameter] protected EventCallback<UIKeyboardEventArgs> KeyDown { get; set; }

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        [Parameter] protected EventCallback<UIKeyboardEventArgs> KeyPress { get; set; }

        /// <summary>
        /// Occurs when a key is released while the control has focus.
        /// </summary>
        [Parameter] protected EventCallback<UIKeyboardEventArgs> KeyUp { get; set; }

        /// <summary>
        /// A string representing a edit mask expression.
        /// </summary>
        [Parameter] protected string EditMask { get; set; }

        /// <summary>
        /// Specify the mask type used by the editor.
        /// </summary>
        [Parameter] protected MaskType MaskType { get; set; }

        /// <summary>
        /// Specifies the maximum number of characters allowed in the input element.
        /// </summary>
        [Parameter] protected int? MaxLength { get; set; }

        #endregion
    }
}
