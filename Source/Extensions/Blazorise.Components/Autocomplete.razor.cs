#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Components
{
    public partial class Autocomplete<TItem> : ComponentBase
    {
        #region Members

        /// <summary>
        /// Original data-source.
        /// </summary>
        private IEnumerable<TItem> data;

        /// <summary>
        /// Holds the filtered data based on the filter.
        /// </summary>
        private List<TItem> filteredData = new List<TItem>();

        /// <summary>
        /// Marks the autocomplete to reload entire data source based on the current filter settings.
        /// </summary>
        private bool dirtyFilter = true;

        private object selectedValue;

        #endregion

        #region Methods

        protected async Task HandleTextChanged( string text )
        {
            CurrentSearch = text ?? string.Empty;
            SelectedText = CurrentSearch;
            dirtyFilter = true;

            if ( text?.Length >= MinLength && FilteredData.Any() )
                dropdownRef.Show();
            else
                dropdownRef.Hide();

            //If input field is empty, clear current SelectedValue.
            if ( string.IsNullOrEmpty( text ) )
                await Clear();

            await SearchChanged.InvokeAsync( CurrentSearch );
        }

        protected async Task HandleTextKeyDown( KeyboardEventArgs e )
        {
            if ( !DropdownVisible )
                return;

            // make sure everything is filtered
            if ( dirtyFilter )
                FilterData();

            var activeItemIndex = ActiveItemIndex;

            if ( ( e.Code == "Enter" || e.Code == "NumpadEnter" || e.Code == "Tab" ) )
            {
                var item = FilteredData.ElementAtOrDefault( activeItemIndex );

                if ( item != null )
                    await HandleDropdownItemClicked( ValueField.Invoke( item ) );
            }
            else if ( e.Code == "Escape" )
            {
                await Clear();
            }
            else if ( e.Code == "ArrowUp" )
            {
                UpdateActiveFilterIndex( --activeItemIndex );
            }
            else if ( e.Code == "ArrowDown" )
            {
                UpdateActiveFilterIndex( ++activeItemIndex );
            }
        }

        protected async Task HandleDropdownItemClicked( object value )
        {
            CurrentSearch = null;
            dropdownRef.Hide();

            var item = Data.FirstOrDefault( x => EqualityComparer<object>.Default.Equals( ValueField( x ), value ) );

            SelectedText = item != null ? TextField?.Invoke( item ) : string.Empty;
            SelectedValue = value;

            await SelectedValueChanged.InvokeAsync( SelectedValue );
            await SearchChanged.InvokeAsync( CurrentSearch );
        }

        private void FilterData()
        {
            FilterData( Data?.AsQueryable() );
        }

        private void FilterData( IQueryable<TItem> query )
        {
            if ( query == null )
            {
                filteredData.Clear();
                return;
            }

            if ( TextField == null )
                return;

            if ( Filter == AutocompleteFilter.Contains )
            {
                query = from q in query
                        let text = TextField.Invoke( q )
                        where text.IndexOf( CurrentSearch, 0, System.StringComparison.CurrentCultureIgnoreCase ) >= 0
                        select q;
            }
            else
            {
                query = from q in query
                        let text = TextField.Invoke( q )
                        where text.StartsWith( CurrentSearch, StringComparison.OrdinalIgnoreCase )
                        select q;
            }

            filteredData = query.ToList();
            ActiveItemIndex = 0;

            dirtyFilter = false;
        }

        /// <summary>
        /// Clears the selected value and the search field.
        /// </summary>
        public async Task Clear()
        {
            CurrentSearch = null;
            dropdownRef.Hide();

            SelectedText = string.Empty;
            SelectedValue = null;

            await SelectedValueChanged.InvokeAsync( selectedValue );
            await SearchChanged.InvokeAsync( CurrentSearch );
        }

        private void UpdateActiveFilterIndex( int activeItemIndex )
        {
            if ( activeItemIndex < 0 )
                activeItemIndex = 0;

            if ( activeItemIndex > ( FilteredData.Count - 1 ) )
                activeItemIndex = FilteredData.Count - 1;

            ActiveItemIndex = activeItemIndex;
        }

        /// <summary>
        /// Sets focus on the input element, if it can be focused.
        /// </summary>
        /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
        public void Focus( bool scrollToElement = true )
        {
            textEdit.Focus( scrollToElement );
        }

        #endregion

        #region Properties

        protected Dropdown dropdownRef;

        protected TextEdit textEdit;

        protected string CurrentSearch { get; set; } = string.Empty;

        protected string SelectedText { get; set; } = string.Empty;

        protected int ActiveItemIndex { get; set; }

        protected bool DropdownVisible => Data != null && TextField != null && CurrentSearch?.Length >= MinLength;

        protected string DropdownClassNames
            => $"{Class} b-is-autocomplete";

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

        /// <summary>
        /// Size of an search field.
        /// </summary>
        [Parameter] public Size Size { get; set; }

        /// <summary>
        /// Prevents a user from entering a value to the search field.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets the autocomplete data-source.
        /// </summary>
        [Parameter]
        public IEnumerable<TItem> Data
        {
            get { return data; }
            set
            {
                data = value;

                // make sure everything is recalculated
                dirtyFilter = true;
            }
        }

        /// <summary>
        /// Gets the data after all of the filters have being applied.
        /// </summary>
        protected IReadOnlyList<TItem> FilteredData
        {
            get
            {
                if ( dirtyFilter )
                    FilterData();

                return filteredData;
            }
        }

        /// <summary>
        /// Method used to get the display field from the supplied data source.
        /// </summary>
        [Parameter] public Func<TItem, string> TextField { get; set; }

        /// <summary>
        /// Method used to get the value field from the supplied data source.
        /// </summary>
        [Parameter] public Func<TItem, object> ValueField { get; set; }

        /// <summary>
        /// Currently selected item value.
        /// </summary>
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
                    ? Data.FirstOrDefault( x => EqualityComparer<object>.Default.Equals( ValueField( x ), value ) )
                    : default;

                SelectedText = item != null
                    ? TextField?.Invoke( item )
                    : string.Empty;
            }
        }

        /// <summary>
        /// Occurs after the selected value has changed.
        /// </summary>
        [Parameter] public EventCallback<object> SelectedValueChanged { get; set; }

        /// <summary>
        /// Occurs on every search text change.
        /// </summary>
        [Parameter] public EventCallback<string> SearchChanged { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public string Style { get; set; }

        [Parameter( CaptureUnmatchedValues = true )]
        public Dictionary<string, object> Attributes { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
