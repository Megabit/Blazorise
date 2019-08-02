#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise.Components.Base
{
    public abstract class BaseAutocomplete<TItem> : ComponentBase
    {
        #region Members

        private object selectedValue;

        #endregion

        #region Methods

        protected void HandleTextChanged( string text )
        {
            CurrentSearch = text ?? string.Empty;

            if ( text?.Length >= MinLength )
                dropdownRef.Open();
            else
                dropdownRef.Close();
        }

        protected void HandleDropdownItemClicked( object value )
        {
            CurrentSearch = null;
            dropdownRef.Close();

            var item = Data.FirstOrDefault( x => ValueField( x ) == value );

            SelectedText = item != null ? TextField?.Invoke( item ) : string.Empty;
            SelectedValue = value;
            SelectedValueChanged.InvokeAsync( SelectedValue );
        }

        protected bool SearchHandler( TItem item )
        {
            var text = TextField?.Invoke( item );

            switch ( Filter )
            {
                case AutocompleteFilter.Contains:
                    return text.IndexOf( CurrentSearch, 0, System.StringComparison.CurrentCultureIgnoreCase ) >= 0;
                default:
                    return text.StartsWith( CurrentSearch, StringComparison.OrdinalIgnoreCase );
            }
        }

        /// <summary>
        /// Clears the selected value and the search field.
        /// </summary>
        public void Clear()
        {
            SelectedText = string.Empty;
            selectedValue = null;
            SelectedValueChanged.InvokeAsync( selectedValue );
        }

        #endregion

        #region Properties

        protected Dropdown dropdownRef;

        protected string CurrentSearch { get; set; } = string.Empty;

        protected string SelectedText { get; set; } = string.Empty;

        /// <summary>
        /// Defines the method by which the search will be done.
        /// </summary>
        [Parameter] public AutocompleteFilter Filter { get; set; } = AutocompleteFilter.StartsWith;

        /// <summary>
        /// The minimum number of characters a user must type before a search is performed.
        /// </summary>
        [Parameter] public int MinLength { get; set; } = 1;

        /// <summary>
        /// Sets the placeholder for the empty search.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }

        [Parameter] public bool IsDisabled { get; set; }

        [Parameter] public IEnumerable<TItem> Data { get; set; }

        [Parameter] public Func<TItem, string> TextField { get; set; }

        [Parameter] public Func<TItem, object> ValueField { get; set; }

        [Parameter]
        public object SelectedValue
        {
            get { return selectedValue; }
            set
            {
                if ( selectedValue == value )
                    return;

                selectedValue = value;

                var item = Data != null
                    ? Data.FirstOrDefault( x => ValueField( x ) == value )
                    : default;

                SelectedText = item != null
                    ? TextField?.Invoke( item )
                    : string.Empty;
            }
        }

        [Parameter] public EventCallback<object> SelectedValueChanged { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
