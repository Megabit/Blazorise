﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise
{
    public partial class ColorEdit : BaseInputComponent<string>
    {
        #region Members

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentValidation != null )
            {
                ParentValidation.InitializeInputExpression( ColorExpression );
            }

            base.OnInitialized();
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.ColorEdit() );

            base.BuildClasses( builder );
        }

        protected Task OnChangeHandler( ChangeEventArgs e )
        {
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        protected override Task OnInternalValueChanged( string value )
        {
            return ColorChanged.InvokeAsync( value );
        }

        protected override string FormatValueAsString( string value )
        {
            return value;
        }

        protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
        {
            return Task.FromResult( new ParseValue<string>( true, value, null ) );
        }

        #endregion

        #region Properties

        protected override string InternalValue { get => Color; set => Color = value; }

        /// <summary>
        /// Gets or sets the input color value.
        /// </summary>
        [Parameter]
        public string Color { get; set; }

        /// <summary>
        /// Occurs when the color has changed.
        /// </summary>
        [Parameter] public EventCallback<string> ColorChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the color value.
        /// </summary>
        [Parameter] public Expression<Func<string>> ColorExpression { get; set; }

        #endregion
    }
}
