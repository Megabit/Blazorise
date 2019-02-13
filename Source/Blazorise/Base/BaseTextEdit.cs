#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseTextEdit : BaseInputComponent
    {
        #region Members

        private Color color;

        //private string text;

        #endregion

        #region Methods

        protected override void RegisterClasses()
        {
            ClassMapper
                .Add( () => ClassProvider.Text( IsPlaintext ) )
                .If( () => ClassProvider.TextColor( Color ), () => Color != Color.None )
                .If( () => ClassProvider.TextSize( Size ), () => Size != Size.None )
                .If( () => ClassProvider.TextValidation( ParentValidation?.Status ?? ValidationStatus.None ), () => ParentValidation?.Status != ValidationStatus.None );

            base.RegisterClasses();
        }

        protected override void OnInit()
        {
            // link to the parent component
            ParentValidation?.Hook( this, Text );

            base.OnInit();
        }

        protected internal override void Dirty()
        {
            ClassMapper.Dirty();

            base.Dirty();
        }

        protected void HandleOnChange( UIChangeEventArgs e )
        {
            Text = e?.Value?.ToString();
            TextChanged?.Invoke( Text );

            ParentValidation?.InputValueChanged( Text );
        }

        //protected void HandleOnInput( UIChangeEventArgs e )
        //{
        //    Console.WriteLine( $"oninput: {e.Value}" );

        //    var newText = e?.Value?.ToString();

        //    if ( TryChangeText( newText ) && TryEditMask( newText ) )
        //    {
        //        text = newText;
        //        TextChanged?.Invoke( text );
        //    }
        //    else
        //    {
        //        // currently there is no other way to prevent onchange value without js :(
        //        JSRunner.SetTextValue( ElementRef, text );
        //    }

        //    //Console.WriteLine( $"text: {Text}; new: {newValue}" );
        //}

        //private bool TryEditMask( string newValue )
        //{
        //    if ( MaskType == MaskType.None || string.IsNullOrEmpty( newValue ) )
        //        return true;

        //    if ( MaskType == MaskType.Numeric )
        //    {
        //        return decimal.TryParse( newValue, out var n );
        //    }
        //    else if ( MaskType == MaskType.DateTime )
        //    {
        //        return DateTime.TryParse( newValue, out var dt );
        //    }

        //    return false;
        //}

        ///// <summary>
        ///// Finds if any of the subscribed events has requested cancel.
        ///// </summary>
        ///// <param name="newValue">New value entered in the field.</param>
        ///// <returns></returns>
        //private bool TryChangeText( string newValue )
        //{
        //    var handler = TextChanging;

        //    if ( handler != null )
        //    {
        //        var args = new ChangingEventArgs( text, newValue, false );

        //        foreach ( Action<ChangingEventArgs> subHandler in handler?.GetInvocationList() )
        //        {
        //            subHandler( args );

        //            if ( args.Cancel )
        //            {
        //                return false;
        //            }
        //        }
        //    }

        //    return true;
        //}

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text inside the input field.
        /// </summary>
        [Parameter]
        protected string Text { get; set; }
        //protected string Text
        //{
        //    get { return text; }
        //    set
        //    {
        //        if ( text != value && TryChangeText( value ) )
        //        {
        //            text = value;
        //        }
        //    }
        //}

        /// <summary>
        /// Occurs after text has changed.
        /// </summary>
        [Parameter] protected Action<string> TextChanged { get; set; }

        //[Parameter] protected Action<ChangingEventArgs> TextChanging { get; set; }

        //[Parameter] protected MaskType MaskType { get; set; } = MaskType.None;

        //[Parameter] protected string MaskFormat { get; set; }

        protected string Type => Role.ToTextRoleString();

        /// <summary>
        /// Sets the role of the input text.
        /// </summary>
        [Parameter] protected TextRole Role { get; set; } = TextRole.Text;

        [Parameter] protected int? Step { get; set; }

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

        [CascadingParameter] protected BaseValidation ParentValidation { get; set; }

        #endregion
    }
}
