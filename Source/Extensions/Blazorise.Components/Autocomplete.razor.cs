#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise.Components
{
    /// <summary>
    /// The autocomplete is a normal text input enhanced by a panel of suggested options.
    /// </summary>
    /// <typeparam name="TItem">Type of an item filtered by the autocomplete component.</typeparam>
    /// <typeparam name="TValue">Type of an SelectedValue field.</typeparam>
    public partial class Autocomplete<TItem, TValue> : ComponentBase
    {
        #region Members

        /// <summary>
        /// Reference to the Dropdown component.
        /// </summary>
        private Dropdown dropdownRef;

        /// <summary>
        /// Reference to the TextEdit component.
        /// </summary>
        private TextEdit textEditRef;

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

        /// <summary>
        /// Holds internal selected value.
        /// </summary>
        private TValue selectedValue;

        #endregion

        #region Methods

        /// <summary>
        /// Handles the search field onchange or oninput event.
        /// </summary>
        /// <param name="text">Serach value.</param>
        /// <returns>Returns awaitable task</returns>
        protected async Task OnTextChangedHandler( string text )
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

        /// <summary>
        /// Handles the search field onfocusin event.
        /// </summary>
        /// <param name="eventArgs">Event arguments.</param>
        /// <returns>Returns awaitable task</returns>
        protected async Task OnTextKeyDownHandler( KeyboardEventArgs e )
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

                if ( item != null && ValueField != null )
                    await OnDropdownItemClicked( ValueField.Invoke( item ) );
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

        /// <summary>
        /// Handles the search field onfocusin event.
        /// </summary>
        /// <param name="eventArgs">Event arguments.</param>
        /// <returns>Returns awaitable task</returns>
        protected Task OnTextFocusInHandler( FocusEventArgs eventArgs )
        {
            TextFocused = true;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the search field onblur event.
        /// </summary>
        /// <param name="eventArgs">Event arguments.</param>
        /// <returns>Returns awaitable task</returns>
        protected async Task OnTextBlurHandler( FocusEventArgs eventArgs )
        {
            // Give enought time for other events to do their stuff before closing
            // the dropdown.
            await Task.Delay( 100 );

            TextFocused = false;
        }

        private async Task OnDropdownItemClicked( object value )
        {
            CurrentSearch = null;
            dropdownRef.Hide();

            var item = Data.FirstOrDefault( x => ValueField( x ).IsEqual( value ) );

            SelectedText = item != null ? TextField?.Invoke( item ) : string.Empty;
            SelectedValue = Converters.ChangeType<TValue>( value );

            await SelectedValueChanged.InvokeAsync( SelectedValue );
            await SearchChanged.InvokeAsync( CurrentSearch );

            textEditRef?.Revalidate();
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
                        where text.IndexOf( CurrentSearch ?? string.Empty, 0, StringComparison.CurrentCultureIgnoreCase ) >= 0
                        select q;
            }
            else
            {
                query = from q in query
                        let text = TextField.Invoke( q )
                        where text.StartsWith( CurrentSearch ?? string.Empty, StringComparison.OrdinalIgnoreCase )
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
            SelectedValue = default;

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

            // update search text with the currently focused item text
            if ( FilteredData.Count > 0 && ActiveItemIndex >= 0 && ActiveItemIndex <= ( FilteredData.Count - 1 ) )
            {
                var item = FilteredData[ActiveItemIndex];

                SelectedText = item != null ? TextField?.Invoke( item ) : string.Empty;
            }
        }

        /// <summary>
        /// Sets focus on the input element, if it can be focused.
        /// </summary>
        /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
        public void Focus( bool scrollToElement = true )
        {
            textEditRef.Focus( scrollToElement );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the dropdown element id.
        /// </summary>
        [Parameter] public string ElementId { get; set; }

        /// <summary>
        /// Gets or sets the current search value.
        /// </summary>
        protected string CurrentSearch { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the currently selected item text.
        /// </summary>
        protected string SelectedText { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the currently active item index.
        /// </summary>
        protected int ActiveItemIndex { get; set; }

        /// <summary>
        /// Gets or sets the serach field focus state.
        /// </summary>
        protected bool TextFocused { get; set; }

        /// <summary>
        /// True if the dropdown menu should be visible.
        /// </summary>
        protected bool DropdownVisible
            => FilteredData?.Count > 0 && TextField != null && CurrentSearch?.Length >= MinLength && TextFocused;

        /// <summary>
        /// Gets the custom classnames for dropdown element.
        /// </summary>
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
        [Parameter] public Func<TItem, TValue> ValueField { get; set; }

        /// <summary>
        /// Currently selected item value.
        /// </summary>
        [Parameter]
        public TValue SelectedValue
        {
            get { return selectedValue; }
            set
            {
                if ( selectedValue.IsEqual( value ) )
                    return;

                selectedValue = value;

                var item = Data != null
                    ? Data.FirstOrDefault( x => ValueField( x ).IsEqual( value ) )
                    : default;

                SelectedText = item != null
                    ? TextField?.Invoke( item )
                    : string.Empty;
            }
        }

        /// <summary>
        /// Occurs after the selected value has changed.
        /// </summary>
        [Parameter] public EventCallback<TValue> SelectedValueChanged { get; set; }

        /// <summary>
        /// Occurs on every search text change.
        /// </summary>
        [Parameter] public EventCallback<string> SearchChanged { get; set; }

        /// <summary>
        /// Custom classname for dropdown element.
        /// </summary>
        [Parameter] public string Class { get; set; }

        /// <summary>
        /// Custom styles for dropdown element.
        /// </summary>
        [Parameter] public string Style { get; set; }

        /// <summary>
        /// If true the text in will be changed after each key press.
        /// </summary>
        /// <remarks>
        /// Note that setting this will override global settings in <see cref="BlazoriseOptions.ChangeTextOnKeyPress"/>.
        /// </remarks>
        [Parameter] public bool? ChangeTextOnKeyPress { get; set; }

        /// <summary>
        /// If true the entered text will be slightly delayed before submiting it to the internal value.
        /// </summary>
        [Parameter] public bool? DelayTextOnKeyPress { get; set; }

        /// <summary>
        /// Interval in milliseconds that entered text will be delayed from submiting to the internal value.
        /// </summary>
        [Parameter] public int? DelayTextOnKeyPressInterval { get; set; }

        /// <summary>
        /// If defined, indicates that its element can be focused and can participates in sequential keyboard navigation.
        /// </summary>
        [Parameter] public int? TabIndex { get; set; }

        /// <summary>
        /// Validation handler used to validate selected value.
        /// </summary>
        [Parameter] public Action<ValidatorEventArgs> Validator { get; set; }

        /// <summary>
        /// Captures all the custom attribute that are not part of Blazorise component.
        /// </summary>
        [Parameter( CaptureUnmatchedValues = true )]
        public Dictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Specifies the content to be rendered inside this <see cref="Autocomplete{TItem, TValue}"/>.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
