#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Utils;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    public abstract class BaseSelect<TValue> : BaseInputComponent<IReadOnlyList<TValue>>
    {
        #region Members

        #endregion

        #region Methods

        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.SelectEdit() );
            builder.Append( ClassProvider.SelectEditSize( Size ), Size != Size.None );
            builder.Append( ClassProvider.SelectEditValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        protected Task OnChangeHandler( ChangeEventArgs e )
        {
            return CurrentValueHandler( e?.Value?.ToString() );
        }

        protected override void OnInternalValueChanged( IReadOnlyList<TValue> value )
        {
            if ( IsMultiple )
                SelectedValuesChanged.InvokeAsync( value );
            else
                SelectedValueChanged.InvokeAsync( value == null ? default : value.FirstOrDefault() );
        }

        protected override object PrepareValueForValidation( IReadOnlyList<TValue> value )
        {
            if ( IsMultiple )
                return value;
            else
                return value == null ? default : value.FirstOrDefault();
        }

        protected override async Task<ParseValue<IReadOnlyList<TValue>>> ParseValueFromStringAsync( string value )
        {
            if ( string.IsNullOrEmpty( value ) )
                return ParseValue<IReadOnlyList<TValue>>.Empty;

            if ( IsMultiple )
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

        public bool ContainsValue( TValue value )
        {
            if ( CurrentValue != null )
                return CurrentValue.Any( x => EqualityComparer<TValue>.Default.Equals( x, value ) );

            return false;
        }

        #endregion

        #region Properties

        public override object ValidationValue
        {
            get
            {
                if ( IsMultiple )
                    return InternalValue;
                else
                    return InternalValue == null ? default : InternalValue.FirstOrDefault();
            }
        }

        protected override IReadOnlyList<TValue> InternalValue
        {
            get => IsMultiple ? SelectedValues : new TValue[] { SelectedValue };
            set
            {
                if ( IsMultiple )
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
        [Parameter] public bool IsMultiple { get; set; }

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
        /// Occurs when the selected items value has changed (only when <see cref="IsMultiple"/>==true).
        /// </summary>
        [Parameter] public EventCallback<IReadOnlyList<TValue>> SelectedValuesChanged { get; set; }

        #endregion
    }
}
