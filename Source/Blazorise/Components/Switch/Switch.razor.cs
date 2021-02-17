﻿#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Switches toggle the state of a single setting on or off.
    /// </summary>
    /// <typeparam name="TValue">Checked value type.</typeparam>
    public partial class Switch<TValue> : BaseCheckComponent<TValue>
    {
        #region Members

        private Color color = Color.None;

        #endregion

        #region Methods

        public override async Task SetParametersAsync( ParameterView parameters )
        {
            await base.SetParametersAsync( parameters );

            if ( ParentValidation != null )
            {
                if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( CheckedExpression ), out var expression ) )
                    ParentValidation.InitializeInputExpression( expression );

                InitializeValidation();
            }
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Switch() );
            builder.Append( ClassProvider.SwitchColor( Color ), Color != Color.None );
            builder.Append( ClassProvider.SwitchSize( Size ), Size != Size.None );
            builder.Append( ClassProvider.SwitchChecked( IsChecked ) );
            builder.Append( ClassProvider.SwitchCursor( Cursor ), Cursor != Cursor.Default );
            builder.Append( ClassProvider.SwitchValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override string TrueValueName => "true";

        /// <summary>
        /// Returns true id switch is in checked state.
        /// </summary>
        protected bool IsChecked => Checked?.ToString()?.ToLowerInvariant() == TrueValueName;

        /// <summary>
        /// Defines the switch named color.
        /// </summary>
        [Parameter]
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                DirtyClasses();
            }
        }

        #endregion
    }
}
