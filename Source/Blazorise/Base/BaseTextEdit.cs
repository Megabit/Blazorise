#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseTextEdit : BaseInputComponent
    {
        #region Members

        private Color color;

        protected string text;

        protected object editValue;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Text( IsPlaintext ) )
                .If( () => ClassProvider.TextColor( Color ), () => Color != Color.None )
                .If( () => ClassProvider.TextSize( Size ), () => Size != Size.None );

            base.RegisterClasses();
        }

        protected void HandleOnChange( UIChangeEventArgs e )
        {
        }

        protected void HandleOnInput( UIChangeEventArgs e )
        {
            Console.WriteLine( "oninput" );

            var newValue = e?.Value?.ToString();

            //if ( !ParseMask( newValue ) )
            //{
            //    // currently there is no other way to prevent onchange value without js :(
            //    JSRunner.SetTextValue( ElementRef, Text );

            //    return;
            //}

            ParseText( newValue );
        }

        private bool ParseMask( string newValue )
        {
            if ( string.IsNullOrEmpty( newValue ) )
                return true;

            if ( !int.TryParse( newValue, out var number ) )
                return false;

            return true;
        }

        private void ParseText( string newValue )
        {
            //Console.WriteLine( $"2: {newValue}" );

            var canceled = CheckCanceled( newValue );

            if ( !canceled )
            {
                text = newValue;

                TextChanged?.Invoke( Text );
            }
            else
            {
                // currently there is no other way to prevent onchange value without js :(
                JSRunner.SetTextValue( ElementRef, Text );
            }

            //Console.WriteLine( $"text: {Text}; new: {newValue}" );
        }

        /// <summary>
        /// Finds if any of the subscribed events has requested cancel.
        /// </summary>
        /// <param name="newValue">New value entered in the field.</param>
        /// <returns></returns>
        private bool CheckCanceled( string newValue )
        {
            var handler = TextChanging;

            if ( handler != null )
            {
                var args = new ChangingEventArgs( Text, newValue, false );

                foreach ( Action<ChangingEventArgs> subHandler in handler?.GetInvocationList() )
                {
                    subHandler( args );

                    if ( args.Cancel )
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text inside the input field.
        /// </summary>
        [Parameter]
        protected string Text
        {
            get { return text; }
            set
            {
                if ( !CheckCanceled( value ) )
                {
                    text = value;
                }
            }
        }

        [Parameter]
        protected object EditValue
        {
            get { return editValue; }
            set
            {
                editValue = value;
                
                text = ( editValue as IFormattable )?.ToString( "C", new System.Globalization.NumberFormatInfo { CurrencySymbol = "kn" } );
            }
        }

        /// <summary>
        /// Occurs after text has changed.
        /// </summary>
        [Parameter] protected Action<string> TextChanged { get; set; }

        [Parameter] protected Action<ChangingEventArgs> TextChanging { get; set; }

        [Parameter] protected Action<object> EditValueChanged { get; set; }

        //[Parameter] protected Action<ConvertEditValueEventArgs> ParseEditValue { get; set; }

        //[Parameter] protected Action<ChangingEventArgs> EditValueChanging { get; set; }

        //[Parameter] protected FormatInfo DisplayFormat { get; set; } = new FormatInfo();

        //[Parameter] protected FormatInfo EditFormat { get; set; } = new FormatInfo();

        [Parameter] protected MaskType MaskType { get; set; } = MaskType.None;

        [Parameter] protected string MaskFormat { get; set; }

        protected string Type => Role.ToTextRoleString();

        /// <summary>
        /// Sets the role of the input text.
        /// </summary>
        [Parameter] protected TextRole Role { get; set; } = TextRole.Text;

        /// <summary>
        /// Sets the placeholder for the empty text.
        /// </summary>
        [Parameter] protected string Placeholder { get; set; }

        /// <summary>
        /// Specifies the maximum number of characters allowed in the input element.
        /// </summary>
        [Parameter] protected int? MaxLength { get; set; }

        /// <summary>
        /// Sets the class to remove the default form field styling and preserve the correct margin and padding.
        /// </summary>
        [Parameter] protected bool IsPlaintext { get; set; }

        [Parameter]
        protected Color Color
        {
            get => color;
            set
            {
                color = value;

                Dirty();
                ClassMapper.Dirty();
            }
        }

        [CascadingParameter] protected BaseAddons ParentAddons { get; set; }

        #endregion
    }
}
