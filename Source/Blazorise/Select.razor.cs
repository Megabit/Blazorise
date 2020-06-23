#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public partial class Select<TValue> : BaseInputComponent<IReadOnlyList<TValue>>
    {
        #region Members

        private bool multiple;

        #endregion

        #region Methods

        protected override void OnInitialized()
        {
            if ( ParentValidation != null )
            {
                if ( Multiple )
                {
                    ParentValidation.InitializeInputExpression( SelectedValuesExpression );
                }
                else
                {
                    ParentValidation.InitializeInputExpression( SelectedValueExpression );
                }
            }

            base.OnInitialized();
        }

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Select() );
            builder.Append( ClassProvider.SelectMultiple(), Multiple );
            builder.Append( ClassProvider.SelectSize( Size ), Size != Size.None );
            builder.Append( ClassProvider.SelectValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        protected Task OnChangeHandler( ChangeEventArgs e )
        {
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        protected override Task OnInternalValueChanged( IReadOnlyList<TValue> value )
        {
            if ( Multiple )
                return SelectedValuesChanged.InvokeAsync( value );
            else
                return SelectedValueChanged.InvokeAsync( value == null ? default : value.FirstOrDefault() );
        }

        protected override object PrepareValueForValidation( IReadOnlyList<TValue> value )
        {
            if ( Multiple )
                return value;
            else
                return value == null ? default : value.FirstOrDefault();
        }

        protected override async Task<ParseValue<IReadOnlyList<TValue>>> ParseValueFromStringAsync( string value )
        {
            if ( string.IsNullOrEmpty( value ) )
                return ParseValue<IReadOnlyList<TValue>>.Empty;

            if ( Multiple )
            {
                // when multiple selection is enabled we need to use javascript to get the list of selected items
                var multipleValues = await JSRunner.GetSelectedOptions<TValue>( ElementId );

                return new ParseValue<IReadOnlyList<TValue>>( true, multipleValues, null );
            }
            else
            {
                if ( Converters.TryChangeType<TValue>( value, out var result ) )
                {
                    return new ParseValue<IReadOnlyList<TValue>>( true, new TValue[] { result }, null );
                }
                else
                {
                    return ParseValue<IReadOnlyList<TValue>>.Empty;
                }
            }
        }

        protected override string FormatValueAsString( IReadOnlyList<TValue> value )
        {
            if ( value == null || value.Count == 0 )
                return string.Empty;

            if ( Multiple )
            {
                return string.Empty;
            }
            else
            {
                if ( value[0] == null )
                    return string.Empty;

                return value[0].ToString();
            }
        }

        public bool ContainsValue( TValue value )
        {
            var currentValue = CurrentValue;

            if ( currentValue != null )
            {
                var result = currentValue.Any( x => EqualityComparer<TValue>.Default.Equals( x, value ) );

                return result;
            }

            return false;
        }

        #endregion

        #region Properties

        public override object ValidationValue
        {
            get
            {
                if ( Multiple )
                    return InternalValue;
                else
                    return InternalValue == null ? default : InternalValue.FirstOrDefault();
            }
        }

        protected override IReadOnlyList<TValue> InternalValue
        {
            get => Multiple ? SelectedValues : new TValue[] { SelectedValue };
            set
            {
                if ( Multiple )
                {
                    SelectedValues = value;
                }
                else
                {
                    SelectedValue = value == null ? default : value.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Specifies that multiple items can be selected.
        /// </summary>
        [Parameter]
        public bool Multiple
        {
            get => multiple;
            set
            {
                multiple = value;

                DirtyClasses();
            }
        }

        /// <summary>
        /// Gets or sets the selected item value.
        /// </summary>
        [Parameter]
        public TValue SelectedValue { get; set; }

        /// <summary>
        /// Gets or sets the multiple selected item values.
        /// </summary>
        [Parameter]
        public IReadOnlyList<TValue> SelectedValues { get; set; }

        /// <summary>
        /// Occurs when the selected item value has changed.
        /// </summary>
        [Parameter] public EventCallback<TValue> SelectedValueChanged { get; set; }

        /// <summary>
        /// Occurs when the selected items value has changed (only when <see cref="Multiple"/>==true).
        /// </summary>
        [Parameter] public EventCallback<IReadOnlyList<TValue>> SelectedValuesChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the selected value.
        /// </summary>
        [Parameter] public Expression<Func<TValue>> SelectedValueExpression { get; set; }

        /// <summary>
        /// Specifies how many options should be shown at once.
        /// </summary>
        [Parameter] public int? MaxVisibleItems { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the selected value.
        /// </summary>
        [Parameter] public Expression<Func<IReadOnlyList<TValue>>> SelectedValuesExpression { get; set; }

        #endregion
    }
}
