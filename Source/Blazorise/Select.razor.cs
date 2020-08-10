#region Using directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
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
        private bool loading;
        private IReadOnlyList<TValue> selectedValues = new List<TValue>();
        private Dictionary<TValue, RenderFragment> items = new Dictionary<TValue, RenderFragment>();
        private Dictionary<TValue, RenderFragment> selectedItems = new Dictionary<TValue, RenderFragment>();
        private IEqualityComparer<TValue> equalityComparer;
        private IReadOnlyList<TValue> internalValues;
        private TValue internalValue;

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

        protected bool RemoveSelectedItem( TValue value )
        {
            return selectedItems.Remove( value );
        }

        public Task AddItemAsync( TValue value, RenderFragment context )
        {
            items.Add( value, context );

            return Task.CompletedTask;
        }

        public async Task SelectValue( TValue value )
        {
            if ( Multiple )
            {
                if ( value != null && !selectedItems.Remove( value ) )
                {
                    if ( items.ContainsKey( value ) )
                    {
                        selectedItems.Add( value, items[value] );
                    }
                }
            }
            else
            {
                selectedItems.Clear();

                if ( value != null && items.ContainsKey( value ) )
                {
                    selectedItems.Add( value, items[value] );
                }
            }

            //await OnChangeHandler( new ChangeEventArgs { Value = value } );
            
            // In case of object values value cannot be converted properly into string 
            // so validation cannot be executed and internal value cannot be set. 
            // Executing event invoking directly. 
            if ( SelectedValueChanged.HasDelegate )
            {
                if ( !multiple )
                {
                    await SelectedValueChanged.InvokeAsync( SelectedValue );
                }
                else
                {
                    await SelectedValuesChanged.InvokeAsync( selectedValues );
                }
            }

            StateHasChanged();
        }

        public async Task SelectValues( IEnumerable<TValue> values )
        {
            foreach ( var value in values )
            {
                await SelectValue( value );
            }
        }

        protected void ClearSelectedItems()
        {
            selectedItems.Clear();
        }

        private void SetComparer( IEqualityComparer<TValue> comparer )
        {
            var _items = new Dictionary<TValue, RenderFragment>( comparer );
            var _selectedItems = new Dictionary<TValue, RenderFragment>( comparer );

            foreach ( var item in items )
            {
                _items.TryAdd( item.Key, item.Value );
            }

            foreach ( var item in selectedItems )
            {
                _selectedItems.TryAdd( item.Key, item.Value );
            }

            items = _items;
            selectedItems = _selectedItems;
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
            get => Multiple ? internalValues : new TValue[] { internalValue };
            set
            {
                if ( Multiple )
                {
                    internalValues = value ?? new List<TValue>();
                }
                else
                {
                    internalValue = value == null ? default : value.FirstOrDefault();
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
        public TValue SelectedValue
        {
            get => selectedItems.Keys.FirstOrDefault();
            set
            {
                SelectValue( value );
            }
        }

        /// <summary>
        /// Gets or sets the multiple selected item values.
        /// </summary>
        [Parameter]
        public IReadOnlyList<TValue> SelectedValues
        {
            get => selectedValues;
            set
            {
                selectedItems.Clear();

                SelectValues( value ).RunSynchronously();
            }
        }

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

        /// <summary>
        /// Gets or sets loading property.
        /// </summary>
        [Parameter] public bool AllowClear { get; set; }

        /// <summary>
        /// Gets the selected item.
        /// </summary>
        public RenderFragment? SelectedItem => SelectedValue != null ? items.TryGetValue( SelectedValue, out RenderFragment content ) ? content : null : null;

        /// <summary>
        /// Gets the selected items.
        /// </summary>
        public IEnumerable<RenderFragment> SelectedItems
        {
            get
            {
                var list = new List<RenderFragment>();

                foreach ( var selectedValue in selectedValues )
                {
                    list.Add( items[selectedValue] );
                }

                return list;
            }
        }

        /// <summary>
        /// Gets component items.
        /// </summary>
        public Dictionary<TValue, RenderFragment> Items => items;

        /// <summary>
        /// Gets or sets values comparer.
        /// </summary>
        [Parameter]
        public IEqualityComparer<TValue> EqualityComparer
        {
            get => equalityComparer;
            set
            {
                equalityComparer = value;
                SetComparer( value );
            }
        }
        #endregion
    }
}
