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
    public partial class Switch<TValue> : BaseInputComponent<TValue>
    {
        #region Members

        private TValue @checked;

        private bool inline;

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
            builder.Append( ClassProvider.Switch() );
            builder.Append( ClassProvider.SwitchChecked( Checked?.ToString() == bool.TrueString ) );
            builder.Append( ClassProvider.SwitchCursor( Cursor ), Cursor != Cursor.Default );
            builder.Append( ClassProvider.SwitchValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        protected Task OnChangeHandler( ChangeEventArgs e )
        {
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
        {
            var parsedValue = value?.ToLowerInvariant() == "true";

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

        protected override TValue InternalValue { get => Checked; set => Checked = value; }

        /// <summary>
        /// Gets or sets the checked flag.
        /// </summary>
        [Parameter]
        public TValue Checked
        {
            get => @checked;
            set
            {
                @checked = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Occurs when the check state is changed.
        /// </summary>
        [Parameter] public EventCallback<TValue> CheckedChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the checked value.
        /// </summary>
        [Parameter] public Expression<Func<TValue>> CheckedExpression { get; set; }

        /// <summary>
        /// Group switches on the same horizontal row.
        /// </summary>
        [Parameter]
        public bool Inline
        {
            get => inline;
            set
            {
                inline = value;

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
