#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Component that allows you to display and edit single-line text.
    /// </summary>
    public partial class TextEdit : BaseTextInput<string>
    {
        #region Methods

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            await base.SetParametersAsync( parameters );

            if ( ParentValidation != null )
            {
                if ( parameters.TryGetValue<Expression<Func<string>>>( nameof( TextExpression ), out var expression ) )
                    ParentValidation.InitializeInputExpression( expression );

                if ( parameters.TryGetValue<string>( nameof( Pattern ), out var pattern ) )
                {
                    // make sure we get the newest value
                    var value = parameters.TryGetValue<string>( nameof( Text ), out var inText )
                        ? inText
                        : InternalValue;

                    ParentValidation.InitializeInputPattern( pattern, value );
                }

                InitializeValidation();
            }
        }

        /// <inheritdoc/>
        protected async override Task OnFirstAfterRenderAsync()
        {
            await JSRunner.InitializeTextEdit( ElementRef, ElementId, MaskType.ToMaskTypeString(), EditMask );

            await base.OnFirstAfterRenderAsync();
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && Rendered )
            {
                JSRunner.DestroyTextEdit( ElementRef, ElementId );
            }

            base.Dispose( disposing );
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.TextEdit( Plaintext ) );
            builder.Append( ClassProvider.TextEditColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.TextEditSize( Size ), Size != Size.None );
            builder.Append( ClassProvider.TextEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override Task OnInternalValueChanged( string value )
        {
            return TextChanged.InvokeAsync( value );
        }

        /// <inheritdoc/>
        protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
        {
            return Task.FromResult( new ParseValue<string>( true, value, null ) );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the string representation of the input role.
        /// </summary>
        protected string Type => Role.ToTextRoleString();

        /// <summary>
        /// Gets the string representation of the input mode.
        /// </summary>
        protected string Mode => InputMode.ToTextInputMode();

        /// <inheritdoc/>
        protected override string InternalValue { get => Text; set => Text = value; }

        /// <inheritdoc/>
        protected override string DefaultValue => string.Empty;

        /// <summary>
        /// Defines the role of the input text.
        /// </summary>
        [Parameter] public TextRole Role { get; set; } = TextRole.Text;

        /// <summary>
        /// Hints at the type of data that might be entered by the user while editing the element or its contents.
        /// </summary>
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

        /// <summary>
        /// The size attribute specifies the visible width, in characters, of an <input> element.
        /// </summary>
        /// <see cref="https://www.w3schools.com/tags/att_input_size.asp"/>
        [Parameter] public int? VisibleCharacters { get; set; }

        #endregion
    }
}