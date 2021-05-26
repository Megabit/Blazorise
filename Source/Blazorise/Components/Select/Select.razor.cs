﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise
{
    /// <summary>
    /// The browser built-in select dropdown.
    /// </summary>
    /// <typeparam name="TValue">The type of the <see cref="SelectedValue"/>.</typeparam>
    public partial class Select<TValue> : BaseInputComponent<IReadOnlyList<TValue>>
    {
        #region Members

        private bool multiple;

        private bool loading;

        private readonly List<ISelectItem<TValue>> selectItems = new();

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            await base.SetParametersAsync( parameters );

            if ( Multiple && parameters.TryGetValue<IReadOnlyList<TValue>>( nameof( SelectedValues ), out var selectedValues ) )
            {
                // For Multiple mode we need to update select options DOM through the javascript or otherwise
                // the newly selected values would not be re-rendered and not visible on the UI.
                ExecuteAfterRender( async () => await JSRunner.SetSelectedOptions( ElementId, selectedValues ) );
            }

            if ( ParentValidation != null )
            {
                if ( Multiple )
                {
                    if ( parameters.TryGetValue<Expression<Func<IReadOnlyList<TValue>>>>( nameof( SelectedValuesExpression ), out var expression ) )
                        await ParentValidation.InitializeInputExpression( expression );
                }
                else
                {
                    if ( parameters.TryGetValue<Expression<Func<TValue>>>( nameof( SelectedValueExpression ), out var expression ) )
                        await ParentValidation.InitializeInputExpression( expression );
                }

                await InitializeValidation();
            }
        }

        /// <inheritdoc/>
        protected override void BuildClasses( ClassBuilder builder )
        {
            builder.Append( ClassProvider.Select() );
            builder.Append( ClassProvider.SelectMultiple(), Multiple );
            builder.Append( ClassProvider.SelectSize( Size ), Size != Size.None );
            builder.Append( ClassProvider.SelectValidation( ParentValidation?.Status ?? ValidationStatus.None ), ParentValidation?.Status != ValidationStatus.None );

            base.BuildClasses( builder );
        }

        /// <summary>
        /// Handles the select onchange event.
        /// </summary>
        /// <param name="eventArgs">Supplies information about an change event that is being raised.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual Task OnChangeHandler( ChangeEventArgs eventArgs )
        {
            return CurrentValueHandler( eventArgs?.Value?.ToString() );
        }

        /// <inheritdoc/>
        protected override Task OnInternalValueChanged( IReadOnlyList<TValue> value )
        {
            if ( Multiple )
                return SelectedValuesChanged.InvokeAsync( value );
            else
                return SelectedValueChanged.InvokeAsync( value == null ? default : value.FirstOrDefault() );
        }

        /// <inheritdoc/>
        protected override object PrepareValueForValidation( IReadOnlyList<TValue> value )
        {
            if ( Multiple )
                return value;
            else
                return value == null ? default : value.FirstOrDefault();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <summary>
        /// Indicates if <see cref="Select{TValue}"/> contains the provided item value.
        /// </summary>
        /// <param name="value">Item value.</param>
        /// <returns>True if value is found.</returns>
        public bool ContainsValue( TValue value )
        {
            var currentValue = CurrentValue;

            if ( currentValue != null )
            {
                var result = currentValue.Any( x => x.IsEqual( value ) );

                return result;
            }

            return false;
        }

        internal void NotifySelectItemInitialized( ISelectItem<TValue> selectItem )
        {
            if ( selectItem == null )
                return;

            if ( !selectItems.Contains( selectItem ) )
                selectItems.Add( selectItem );
        }

        internal void NotifySelectItemRemoved( ISelectItem<TValue> selectItem )
        {
            if ( selectItem == null )
                return;

            if ( selectItems.Contains( selectItem ) )
                selectItems.Remove( selectItem );
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
        /// Gets the list of all select items inside of this select component.
        /// </summary>
        protected IEnumerable<ISelectItem<TValue>> SelectItems => selectItems;

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
        [Parameter] public TValue SelectedValue { get; set; }

        /// <summary>
        /// Gets or sets the multiple selected item values.
        /// </summary>
        [Parameter] public IReadOnlyList<TValue> SelectedValues { get; set; }

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

        /// <summary>
        /// Gets or sets loading property.
        /// </summary>
        [Parameter]
        public bool Loading
        {
            get => loading;
            set
            {
                loading = value;
                Disabled = value;
            }
        }

        #endregion
    }
}
