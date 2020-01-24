#region Using directives
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class CheckEdit<TValue> : BaseInputComponent<TValue>
    {
        #region Members

        private string radioGroup;

        private bool isInline;

        private Cursor cursor;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentValidation != null )
            {
                if ( CheckedExpression != null )
                    ParentValidation.InitializeInputExpression( CheckedExpression );
            }

            base.OnInitialized();
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            if ( RadioGroup != null )
                builder.Append( ClassProvider.RadioEdit() );
            else
                builder.Append( ClassProvider.CheckEdit() );

            builder.Append( ClassProvider.CheckEditCursor( Cursor ), Cursor != Cursor.Default );
            builder.Append( ClassProvider.CheckEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        protected Task OnChangeHandler( ChangeEventArgs e )
        {
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
        {
            // radio and checkbox have diferent values so we need to convert all to "true" or "false"
            var parsedValue = ( value?.ToLowerInvariant() == ( RadioGroup != null ? "on" : "true" ) ).ToString();

            if ( Converters.TryChangeType<TValue>( parsedValue, out var result, CultureInfo.InvariantCulture ) )
            {
                return Task.FromResult( new ParseValue<TValue>( true, result, null ) );
            }
            else
            {
                return Task.FromResult( ParseValue<TValue>.Empty );
            }
        }

        protected override Task OnInternalValueChanged( TValue value )
        {
            return CheckedChanged.InvokeAsync( Checked );
        }

        #endregion

        #region Properties

        protected ControlRole Role => RadioGroup != null ? ControlRole.Radio : ControlRole.Check;

        protected string Type => RadioGroup != null ? "radio" : "checkbox";

        protected override TValue InternalValue { get => Checked; set => Checked = value; }

        /// <summary>
        /// Gets or sets the checked flag.
        /// </summary>
        [Parameter] public TValue Checked { get; set; }

        /// <summary>
        /// Occurs when the check state is changed.
        /// </summary>
        [Parameter] public EventCallback<TValue> CheckedChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the checked value.
        /// </summary>
        [Parameter] public Expression<Func<TValue>> CheckedExpression { get; set; }

        /// <summary>
        /// Sets the radio group name.
        /// </summary>
        [Parameter]
        public string RadioGroup
        {
            get => radioGroup;
            set
            {
                radioGroup = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Group checkboxes or radios on the same horizontal row.
        /// </summary>
        [Parameter]
        public bool IsInline
        {
            get => isInline;
            set
            {
                isInline = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the mouse cursor based on the behaviour by the current css framework.
        /// </summary>
        [Parameter]
        public Cursor Cursor
        {
            get => cursor;
            set
            {
                cursor = value;

                DirtyClasses();
            }
        }

        #endregion
    }
}
