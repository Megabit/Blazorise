﻿#region Using directives
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Base class for all check-able components.
    /// </summary>
    /// <typeparam name="TValue">Checked value type.</typeparam>
    public abstract class BaseCheckComponent<TValue> : BaseInputComponent<TValue>
    {
        #region Members

        private bool inline;

        private Cursor cursor;

        #endregion

        #region Methods

        protected virtual Task OnChangeHandler( ChangeEventArgs e )
        {
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
        {
            var parsedValue = ( value?.ToLowerInvariant() == TrueValueName?.ToLowerInvariant() ).ToString();

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
        /// Gets the string value that represents the checked state.
        /// </summary>
        protected abstract string TrueValueName { get; }

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
        /// Group checkboxes or radios on the same horizontal row.
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
