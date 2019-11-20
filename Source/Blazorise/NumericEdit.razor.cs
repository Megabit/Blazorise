#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise
{
    /// <summary>
    /// This is needed to set the value from javascript because calling generic component directly is not supported by Blazor.
    /// </summary>
    public interface INumericEdit
    {
        Task SetValue( string value );
    }

    public abstract class BaseNumericEdit<TValue> : BaseTextInput<TValue>, INumericEdit
    {
        #region Members

        // taken from https://github.com/aspnet/AspNetCore/issues/11159
        private DotNetObjectReference<NumericEditAdapter> dotNetObjectRef;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentValidation != null )
            {
                ParentValidation.InitializeInputExpression( ValueExpression );
            }

            base.OnInitialized();
        }

        protected override async Task OnFirstAfterRenderAsync()
        {
            dotNetObjectRef ??= JSRunner.CreateDotNetObjectRef( new NumericEditAdapter( this ) );
            await JSRunner.InitializeNumericEdit( dotNetObjectRef, ElementId, ElementRef, Decimals, DecimalsSeparator, Step );

            await base.OnFirstAfterRenderAsync();
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                JSRunner.DestroyNumericEdit( ElementId, ElementRef );
                JSRunner.DisposeDotNetObjectRef( dotNetObjectRef );
            }

            base.Dispose( disposing );
        }

        public Task SetValue( string value )
        {
            return CurrentValueHandler( value );
        }

        protected override Task OnInternalValueChanged( TValue value )
        {
            return ValueChanged.InvokeAsync( value );
        }

        protected override Task<ParseValue<TValue>> ParseValueFromStringAsync( string value )
        {
            if ( Converters.TryChangeType<TValue>( value, out var result ) )
            {
                return Task.FromResult( new ParseValue<TValue>( true, result, null ) );
            }
            else
            {
                return Task.FromResult( ParseValue<TValue>.Empty );
            }
        }

        #endregion

        #region Properties

        protected override TValue InternalValue { get => Value; set => Value = value; }

        /// <summary>
        /// Gets or sets the value inside the input field.
        /// </summary>
        [Parameter] public TValue Value { get; set; }

        /// <summary>
        /// Occurs after the value has changed.
        /// </summary>
        /// <remarks>
        /// This will be converted to EventCallback once the Blazor team fix the error for generic components. see https://github.com/aspnet/AspNetCore/issues/8385
        /// </remarks>
        [Parameter] public EventCallback<TValue> ValueChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the value.
        /// </summary>
        [Parameter] public Expression<Func<TValue>> ValueExpression { get; set; }

        /// <summary>
        /// Specifies the interval between valid values.
        /// </summary>
        [Parameter] public decimal? Step { get; set; }

        /// <summary>
        /// Maximum number of decimal places after the decimal separator.
        /// </summary>
        [Parameter] public int Decimals { get; set; } = 2;

        /// <summary>
        /// String to use as the decimal separator in numeric values.
        /// </summary>
        [Parameter] public string DecimalsSeparator { get; set; } = ".";

        ///// <summary>
        ///// The minimum value to accept for this input.
        ///// </summary>
        //[Parameter] public TValue? Min { get; set; }

        ///// <summary>
        ///// The maximum value to accept for this input.
        ///// </summary>
        //[Parameter] public TValue? Max { get; set; }

        #endregion
    }
}
