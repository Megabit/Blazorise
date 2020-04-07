#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// RadioGroup is a helpful wrapper used to group Radio components.
    /// </summary>
    public partial class RadioGroup<TValue> : BaseInputComponent<TValue>
    {
        #region Members

        private bool inline;

        private bool buttons;

        public event EventHandler<RadioCheckedChangedEventArgs<TValue>> RadioCheckedChanged;

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.RadioGroup( Buttons ) );
            builder.Append( ClassProvider.RadioGroupInline(), Inline );

            base.BuildClasses( builder );
        }

        protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
        {
            if ( string.IsNullOrEmpty( value ) )
                return Task.FromResult( ParseValue<TValue>.Empty );

            if ( Converters.TryChangeType<TValue>( value, out var result ) )
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
            // notify child radios they need to update their states
            RadioCheckedChanged.Invoke( this, new RadioCheckedChangedEventArgs<TValue>( value ) );

            return CheckedValueChanged.InvokeAsync( value );
        }

        internal async Task NotifyRadioChanged( Radio<TValue> radio )
        {
            await CurrentValueHandler( radio.Value?.ToString() );

            StateHasChanged();
        }

        #endregion

        #region Properties

        protected override TValue InternalValue { get => CheckedValue; set => CheckedValue = value; }

        /// <summary>
        /// Radio group name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// Group radios on the same horizontal row.
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
        /// The combination of radio button style.
        /// </summary>
        [Parameter]
        public bool Buttons
        {
            get => buttons;
            set
            {
                buttons = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the checked value.
        /// </summary>
        [Parameter] public TValue CheckedValue { get; set; }

        /// <summary>
        /// Occurs when the checked value is changed.
        /// </summary>
        [Parameter] public EventCallback<TValue> CheckedValueChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the checked value.
        /// </summary>
        [Parameter] public Expression<Func<TValue>> CheckedExpression { get; set; }

        #endregion
    }
}
