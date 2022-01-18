#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components
{
    /// <summary>
    /// Dynamically builds select component and it's items based in the supplied data.
    /// </summary>
    /// <typeparam name="TItem">Data item type.</typeparam>
    /// <typeparam name="TValue">Type if the value inside of <see cref="SelectList{TItem, TValue}"/>.</typeparam>
    public partial class SelectList<TItem, TValue> : ComponentBase
    {
        #region Members

        /// <summary>
        /// Reference to the <see cref="Select{TValue}"/> component.
        /// </summary>
        private Select<TValue> selectRef;

        private TValue selectedValue;

        #endregion

        #region Methods

        protected Task HandleSelectedValueChanged( TValue value )
        {
            SelectedValue = value;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets focus on the input element, if it can be focused.
        /// </summary>
        /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Focus( bool scrollToElement = true )
        {
            return selectRef.Focus( scrollToElement );
        }

        internal TValue GetValue( TItem item )
        {
            if ( ValueField == null )
            {
                // use index
                if ( Data is IList list && typeof(TValue) == typeof(int))
                {
                    return (TValue) (list.IndexOf( item ) as object);
                }

                return default;
            }
            else
            {
                return ValueField.Invoke( item );
            }
        }

        internal string GetKey( TItem item )
        {
            if ( ValueField == null )
                return item.GetHashCode().ToString();
            else
                return ValueField.Invoke( item ).ToString();
        }

        internal string GetText( TItem item )
        {
            return TextField != null ? TextField.Invoke( item ) : item.ToString();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the select element id.
        /// </summary>
        [Parameter] public string ElementId { get; set; }

        /// <summary>
        /// Gets or sets the select data-source.
        /// </summary>
        [Parameter] public IEnumerable<TItem> Data { get; set; }

        /// <summary>
        /// Method used to get the display field from the supplied data source.
        /// </summary>
        [Parameter] public Func<TItem, string> TextField { get; set; }

        /// <summary>
        /// Method used to get the value field from the supplied data source.
        /// </summary>
        [Parameter] public Func<TItem, TValue> ValueField { get; set; }

        /// <summary>
        /// Currently selected item value.
        /// </summary>
        [Parameter]
        public TValue SelectedValue
        {
            get => selectedValue;
            set
            {
                if ( !selectedValue.Equals( value ) )
                {
                    selectedValue = value;
                    SelectedValueChanged.InvokeAsync( selectedValue ).ConfigureAwait( false );
                    if ( SelectedItemSupported )
                    {
                        SelectedItemChanged.InvokeAsync( SelectedItem ).ConfigureAwait( false );
                    }
                }
            }
        }

        /// <summary>
        /// Occurs after the selected value has changed.
        /// </summary>
        [Parameter] public EventCallback<TValue> SelectedValueChanged { get; set; }

        /// <summary>
        /// Gets or sets an expression that identifies the selected value.
        /// </summary>
        [Parameter] public Expression<Func<TValue>> SelectedValueExpression { get; set; }

        /// <summary>
        /// Currently selected item in case <see cref="Data"/> is of type <see cref="IList"/> and no <see cref="ValueField"/> is given. 
        /// </summary>
        [Parameter]
        public TItem SelectedItem
        {
            get
            {
                if ( SelectedItemSupported )
                {
                    var list = (IList) Data;
                    int index = SelectedValue is int value ? value : -1;

                    if ( index < 0 || index >= list.Count )
                        return default;

                    return (TItem)list[index];
                }

                return default;
            }
            set
            {
                if ( SelectedItemSupported )
                {
                    var list = (IList) Data;
                    var index = list.IndexOf( value );
                    TValue val = (TValue)( index as object );
                    SelectedValue = val;
                }
            }
        }

        /// <summary>
        /// Occurs after the selected item has changed.
        /// </summary>
        [Parameter] public EventCallback<TItem> SelectedItemChanged { get; set; }

        /// <summary>
        /// Display text of the default select item.
        /// </summary>
        [Parameter] public string DefaultItemText { get; set; }

        /// <summary>
        /// Value of the default select item.
        /// </summary>
        [Parameter] public TValue DefaultItemValue { get; set; }

        /// <summary>
        /// If true, disables the default item.
        /// </summary>
        [Parameter] public bool DefaultItemDisabled { get; set; } = false;

        /// <summary>
        /// If true, disables the default item.
        /// </summary>
        [Parameter] public bool DefaultItemHidden { get; set; } = false;

        /// <summary>
        /// Custom css class-names.
        /// </summary>
        [Parameter] public string Class { get; set; }

        /// <summary>
        /// Custom styles.
        /// </summary>
        [Parameter] public string Style { get; set; }

        /// <summary>
        /// Size of a select field.
        /// </summary>
        [Parameter] public Size? Size { get; set; }

        /// <summary>
        /// Specifies how many options should be shown at once.
        /// </summary>
        [Parameter] public int? MaxVisibleItems { get; set; }

        /// <summary>
        /// If defined, indicates that its element can be focused and can participates in sequential keyboard navigation.
        /// </summary>
        [Parameter] public int? TabIndex { get; set; }

        /// <summary>
        /// Add the disabled boolean attribute on an select to prevent user interactions and make it appear lighter.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Captures all the custom attribute that are not part of Blazorise component.
        /// </summary>
        [Parameter( CaptureUnmatchedValues = true )]
        public Dictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Placeholder for validation messages.
        /// </summary>
        [Parameter] public RenderFragment Feedback { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="SelectList{TItem, TValue}"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }


        /// <summary>
        /// Returns true if SelectedItem property is supported.
        /// </summary>
        public bool SelectedItemSupported =>
            ValueField == null && Data is IList && typeof(TValue) == typeof(int);

        #endregion
    }
}
