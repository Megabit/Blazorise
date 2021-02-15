﻿#region Using directives
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// Radio buttons allow the user to select one option from a set.
    /// </summary>
    /// <typeparam name="TValue">Checked value type.</typeparam>
    public partial class Radio<TValue> : BaseCheckComponent<bool>
    {
        #region Members

        private string group;

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            await base.SetParametersAsync( parameters );

            // Individual Radio can have validation ONLY of it's not placed inside
            // of a RadioGroup
            if ( ParentValidation != null && ParentRadioGroup == null )
            {
                if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( CheckedExpression ), out var expression ) )
                    ParentValidation.InitializeInputExpression( expression );

                InitializeValidation();
            }
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if ( ParentRadioGroup != null )
            {
                Checked = ParentRadioGroup.CheckedValue.IsEqual( Value );

                // TODO: possibly memory leak in Blazor server-side with prerendering mode!
                ParentRadioGroup.RadioCheckedChanged += OnRadioChanged;

                // Parent group name have higher priority!
                if ( string.IsNullOrEmpty( Group ) )
                {
                    Group = ParentRadioGroup.Name;
                }
            }

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Radio( AsButton ) );
            builder.Append( ClassProvider.RadioSize( AsButton, Size ), Size != Size.None );
            builder.Append( ClassProvider.RadioCursor( Cursor ), Cursor != Cursor.Default );
            builder.Append( ClassProvider.RadioValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( ParentRadioGroup != null )
                {
                    ParentRadioGroup.RadioCheckedChanged -= OnRadioChanged;
                }
            }

            base.Dispose( disposing );
        }

        /// <inheritdoc/>
        protected override Task OnChangeHandler( ChangeEventArgs e )
        {
            if ( ParentRadioGroup != null )
                return ParentRadioGroup.NotifyRadioChanged( this );

            // Radio should always be inside of RadioGroup or otherwise it's "checked" state will not
            // be activated like it should be. I will leave this just in case that users want to use it
            // but I will need to state in the documentation that it's generally not supported.
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        /// <inheritdoc/>
        protected override Task<ParseValue<bool>> ParseValueFromStringAsync( string value )
        {
            return base.ParseValueFromStringAsync( value );
        }

        /// <summary>
        /// Event that raises after one of other radios inside of group changes.
        /// </summary>
        /// <param name="sender">Reference to the object that raised the event.</param>
        /// <param name="eventArgs">Information about the currently checked radio.</param>
        private async void OnRadioChanged( object sender, RadioCheckedChangedEventArgs<TValue> eventArgs )
        {
            await CurrentValueHandler( eventArgs?.Value?.ToString() );

            // Some providers like AntDesign need additional changes on classes or styles.
            DirtyClasses();
            DirtyStyles();
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        protected override string TrueValueName => Value?.ToString();

        /// <summary>
        /// True if radio belongs to the <see cref="RadioGroup{TValue}"/>.
        /// </summary>
        protected bool ParentIsRadioGroup => ParentRadioGroup != null;

        /// <summary>
        /// True if radio should look as a regular button.
        /// </summary>
        protected bool AsButton => ParentRadioGroup?.Buttons == true;

        /// <summary>
        /// Returns the button color.
        /// </summary>
        protected Color ButtonColor => ParentRadioGroup?.Color ?? Color.Secondary;

        /// <summary>
        /// Sets the radio group name.
        /// </summary>
        [Parameter]
        public string Group
        {
            get => group;
            set
            {
                group = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the radio value.
        /// </summary>
        [Parameter] public TValue Value { get; set; }

        /// <summary>
        /// Radio group in which this radio is placed.
        /// </summary>
        [CascadingParameter] protected RadioGroup<TValue> ParentRadioGroup { get; set; }

        #endregion
    }
}
