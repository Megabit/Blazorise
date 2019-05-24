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

        #endregion

        #region Properties

        protected Dropdown dropdownRef;

        protected string CurrentSearch { get; set; } = string.Empty;

        protected string SelectedText { get; set; } = string.Empty;

        /// <summary>
        /// Defines the method by which the search will be done.
        /// </summary>
        [Parameter] protected AutocompleteFilter Filter { get; set; } = AutocompleteFilter.StartsWith;

        /// <summary>
        /// The minimum number of characters a user must type before a search is performed.
        /// </summary>
        [Parameter] protected int MinLength { get; set; } = 1;

        /// <summary>
        /// Sets the placeholder for the empty search.
        /// </summary>
        [Parameter] protected string Placeholder { get; set; }

        [Parameter] protected bool IsDisabled { get; set; }

        [Parameter] protected IEnumerable<TItem> Data { get; set; }

        [Parameter] protected Func<TItem, string> TextField { get; set; }

        [Parameter] protected Func<TItem, object> ValueField { get; set; }

        [Parameter] protected object SelectedValue { get; set; }

        [Parameter] protected EventCallback<object> SelectedValueChanged { get; set; }

        [Parameter] protected RenderFragment ChildContent { get; set; }

        #endregion
    }
}
