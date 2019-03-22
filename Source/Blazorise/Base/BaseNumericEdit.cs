#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Base
{
    public abstract class BaseNumericEdit<TValue> : BaseTextInput<TValue>
    //where TValue : struct 
    {
        #region Members

        #endregion

        #region Methods

        protected override void HandleValue( object value )
        {
            if ( Converters.TryChangeType<TValue>( value, out var result ) )
            {
                // TODO: disabled until Blazor implements constraints for generic components!!!!
                //if ( Max != null && Comparers.Compare( result, Max ) > 0 )
                //    result = Max ?? default;
                //else if ( Min != null && Comparers.Compare( result, Min ) < 0 )
                //    result = Min ?? default;

                Value = result;
            }
            else
                Value = default;

            ValueChanged?.Invoke( Value );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value inside the input field.
        /// </summary>
        [Parameter] protected TValue Value { get => InternalValue; set => InternalValue = value; }

        /// <summary>
        /// Occurs after the value has changed.
        /// </summary>
        /// <remarks>
        /// This will be converted to EventCallback once the Blazor team fix the error for generic components. see https://github.com/aspnet/AspNetCore/issues/8385
        /// </remarks>
        [Parameter] protected Action<TValue> ValueChanged { get; set; }

        /// <summary>
        /// Specifies the interval between valid values.
        /// </summary>
        [Parameter] protected int? Step { get; set; }

        ///// <summary>
        ///// The minimum value to accept for this input.
        ///// </summary>
        //[Parameter] protected TValue? Min { get; set; }

        ///// <summary>
        ///// The maximum value to accept for this input.
        ///// </summary>
        //[Parameter] protected TValue? Max { get; set; }

        #endregion
    }
}
