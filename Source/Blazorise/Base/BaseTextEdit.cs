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
    public abstract class BaseTextEdit : BaseInputComponent<string>
    {
        #region Members

        private Color color;

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
        /// Gets or sets the text inside the input field.
        /// </summary>
        [Parameter]
        protected string Text { get => Value; set => Value = value; }

        /// <summary>
        /// Occurs after text has changed.
        /// </summary>
        [Parameter] protected Action<string> TextChanged { get; set; }

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

        #endregion
    }
}
