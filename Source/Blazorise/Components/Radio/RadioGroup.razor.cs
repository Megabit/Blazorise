#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utilities;
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

        /// <summary>
        /// Flag which indicates that radios will appear as button.
        /// </summary>
        private bool buttons;

        /// <summary>
        /// Defines the orientation of the radio elements.
        /// </summary>
        private Orientation orientation = Orientation.Horizontal;

        /// <summary>
        /// Defines the color or radio buttons(only when <see cref="Buttons"/> is true).
        /// </summary>
        private Color color = Color.Secondary;

        /// <summary>
        /// Flag needed for radiogroup to work property. Since the group is notified of it's state
        /// from child radio component we need to skip calling event callback when we get the new
        /// state through the param from outside. And it happens as a consequence of calling the
        /// infamous StateHasChanged().
        /// </summary>
        private bool skipCheckedValueChangedCallback = false;

        /// <summary>
        /// An event raised after the internal radio group value has changed.
        /// </summary>
        public event EventHandler<RadioCheckedChangedEventArgs<TValue>> RadioCheckedChanged;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            try
            {
                skipCheckedValueChangedCallback = true;

                if ( parameters.TryGetValue<TValue>( nameof( CheckedValue ), out var result ) )
                {
                    await CurrentValueHandler( result?.ToString() );
                }

                await base.SetParametersAsync( parameters );

                if ( ParentValidation != null )
                {
                    if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( CheckedValueExpression ), out var expression ) )
                        ParentValidation.InitializeInputExpression( expression );

                    InitializeValidation();
                }
            }
            finally
            {
                skipCheckedValueChangedCallback = false;
            }
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.RadioGroup( Buttons, Orientation ) );
            builder.Append( ClassProvider.RadioGroupValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        protected override async Task OnInternalValueChanged( TValue value )
        {
            // notify child radios they need to update their states
            RadioCheckedChanged?.Invoke( this, new RadioCheckedChangedEventArgs<TValue>( value ) );

            if ( !skipCheckedValueChangedCallback )
            {
                await CheckedValueChanged.InvokeAsync( value );
            }
        }

        /// <summary>
        /// Notifies radio group that one of it's radios have changed.
        /// </summary>
        /// <param name="radio">Radio from which change was received.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        internal async Task NotifyRadioChanged( Radio<TValue> radio )
        {
            await CurrentValueHandler( radio.Value?.ToString() );

            await InvokeAsync( StateHasChanged );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override TValue InternalValue { get => CheckedValue; set => CheckedValue = value; }

        /// <summary>
        /// True of radio elements should be inlined.
        /// </summary>
        public bool Inline => Orientation == Orientation.Horizontal;

        /// <summary>
        /// Radio group name.
        /// </summary>
        [Parameter] public string Name { get; set; }

        /// <summary>
        /// Flag which indicates that radios will appear as button.
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
        /// Defines the orientation of the radio elements.
        /// </summary>
        [Parameter]
        public Orientation Orientation
        {
            get => orientation;
            set
            {
                orientation = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Defines the color or radio buttons(only when <see cref="Buttons"/> is true).
        /// </summary>
        [Parameter]
        public Color Color
        {
            get => color;
            set
            {
                color = value;

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
        [Parameter] public Expression<Func<TValue>> CheckedValueExpression { get; set; }

        #endregion
    }
}
