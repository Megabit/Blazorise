#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using static System.Net.Mime.MediaTypeNames;
#endregion

namespace Blazorise.Components
{
    /// <summary>
    /// The autocomplete is a normal text input enhanced by a panel of suggested options.
    /// </summary>
    /// <typeparam name="TItem">Type of an item filtered by the autocomplete component.</typeparam>
    /// <typeparam name="TValue">Type of an SelectedValue field.</typeparam>
    public partial class Autocomplete<TItem, TValue> : BaseAfterRenderComponent, ICloseActivator
    {
        #region Members

        /// <summary>
        /// Tells us that modal is tracked by the JS interop.
        /// </summary>
        private bool jsRegistered;

        /// <summary>
        /// A JS interop object reference used to access this modal.
        /// </summary>
        private DotNetObjectReference<CloseActivatorAdapter> dotNetObjectRef;

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
        private List<TItem> filteredData = new();

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

        /// <inheritdoc/>
        protected override async Task OnParametersSetAsync()
        {
            ExecuteAfterRender( SyncMultipleValuesAndTexts );
            await base.OnParametersSetAsync();
        }

        private async Task SyncMultipleValuesAndTexts()
        {
            List<string> textsToAdd = new();
            if ( SelectedValues is not null)
                foreach ( var selectedValue in SelectedValues )
                    textsToAdd.Add( GetDisplayValue( selectedValue ) );

            if ( SelectedTexts != null )
                foreach ( var selectedText in SelectedTexts )
                    await AddMultipleValue( GetValueByMultipleText( selectedText ) );

            foreach ( var textToAdd in textsToAdd )
                await AddMultipleText( textToAdd );
        }

        /// <inheritdoc/>
        protected override Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                dotNetObjectRef ??= DotNetObjectReference.Create( new CloseActivatorAdapter( this ) );
            }

            return base.OnAfterRenderAsync( firstRender );
        }

        /// <summary>
        /// Handles the search field onchange or oninput event.
        /// </summary>
        /// <param name="text">Search value.</param>
        /// <returns>Returns awaitable task</returns>
        protected async Task OnTextChangedHandler( string text )
        {
            CurrentSearch = text ?? string.Empty;
            SelectedText = CurrentSearch;
            dirtyFilter = true;

            //If input field is empty, clear current SelectedValue.
            if ( string.IsNullOrEmpty( text ) )
                await Clear();

            await SearchChanged.InvokeAsync( CurrentSearch );
            await SelectedTextChanged.InvokeAsync( SelectedText );

            if ( FilteredData?.Count == 0 && NotFound.HasDelegate )
                await NotFound.InvokeAsync( CurrentSearch );

            await InvokeAsync( StateHasChanged );
        }

        /// <summary>
        /// Handles the search field OnKeyDown event.
        /// </summary>
        /// <param name="eventArgs">Event arguments.</param>
        /// <returns>Returns awaitable task</returns>
        protected async Task OnTextKeyDownHandler( KeyboardEventArgs eventArgs )
        {
            if ( !DropdownVisible )
            {
                if ( ConfirmKey( eventArgs ) )
                {
                    if ( FreeTyping && Multiple )
                    {
                        await AddMultipleText( SelectedText );
                        await ResetSelectedText();
                    }
                }
                await UnregisterClosableComponent();
                return;
            }

            if ( dirtyFilter )
                FilterData();

            var activeItemIndex = ActiveItemIndex;

            if ( ConfirmKey( eventArgs ) )
            {
                var item = FilteredData.ElementAtOrDefault( activeItemIndex );

                if ( item != null && ValueField != null )
                    await OnDropdownItemClicked( ValueField.Invoke( item ) );
                else if ( FreeTyping && Multiple )
                {
                    await AddMultipleText( SelectedText );
                    await ResetSelectedText();
                }

            }
            else if ( eventArgs.Code == "ArrowUp" )
            {
                await UpdateActiveFilterIndex( --activeItemIndex );
            }
            else if ( eventArgs.Code == "ArrowDown" )
            {
                await UpdateActiveFilterIndex( ++activeItemIndex );
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
            // Give enough time for other events to do their stuff before closing
            // the dropdown.
            await Task.Delay( 250 );
            await UnregisterClosableComponent();

            if ( !FreeTyping && ( SelectedValue == null || Multiple ) )
            {
                await ResetSelectedText();
            }

            if ( FreeTyping && Multiple )
            {
                await AddMultipleText( SelectedText );
                await ResetSelectedText();
            }

            TextFocused = false;
        }

        private async Task OnDropdownItemClicked( object value )
        {
            CurrentSearch = null;

            var item = Data.FirstOrDefault( x => ValueField( x ).IsEqual( value ) );

            SelectedValue = Converters.ChangeType<TValue>( value );

            await SelectedValueChanged.InvokeAsync( SelectedValue );
            await SearchChanged.InvokeAsync( CurrentSearch );

            if ( Multiple )
            {
                await AddMultipleText( selectedValue );
                await AddMultipleValue( selectedValue );
                await ResetSelectedText();
            }
            else
            {
                SelectedText = TextField?.Invoke( item ) ?? string.Empty;
                await SelectedTextChanged.InvokeAsync( SelectedText );
            }

            await textEditRef?.Revalidate();
        }

        private static bool ConfirmKey( KeyboardEventArgs eventArgs )
        {
            return eventArgs.Code == "Enter" || eventArgs.Code == "NumpadEnter" || eventArgs.Code == "Tab";
        }


        private Task ResetSelectedText()
        {
            SelectedText = string.Empty;
            return SelectedTextChanged.InvokeAsync( string.Empty );
        }
        private async Task AddMultipleValue( TValue value )
        {
            SelectedValues ??= new();
            if ( !SelectedValues.Contains( value ) && value != null )
            {
                SelectedValues.Add( value );
                await SelectedValuesChanged.InvokeAsync( SelectedValues );
            }
        }

        private async Task RemoveMultipleValue( TValue value )
        {
            SelectedValues.Remove( value );
            await SelectedValuesChanged.InvokeAsync( SelectedValues );
        }


        private Task AddMultipleText( TValue value )
            => AddMultipleText( GetDisplayValue( value ) );


        private Task AddMultipleText( string text )
        {
            SelectedTexts ??= new();
            if ( !string.IsNullOrEmpty( text ) && !SelectedTexts.Contains( text ) )
            {
                SelectedTexts.Add( text );
                return SelectedTextsChanged.InvokeAsync( SelectedTexts );
            }
            return Task.CompletedTask;
        }

        private Task AddMultipleText( List<string> texts )
        {
            SelectedTexts ??= new();

            foreach ( var text in texts )
            {
                if ( !string.IsNullOrEmpty( text ) && !SelectedTexts.Contains( text ) )
                    SelectedTexts.Add( text );
            }

            return SelectedTextsChanged.InvokeAsync( SelectedTexts );
        }

        private async Task RemoveMultipleText( string text )
        {
            SelectedTexts.Remove( text );
            await RemoveMultipleValue( GetValueByMultipleText( text ) );
            await SelectedTextsChanged.InvokeAsync( SelectedTexts );
        }

        private TValue GetValueByMultipleText( string text )
            => SelectedValues.FirstOrDefault( x => GetDisplayValue( x ) == text );

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

            var currentSearch = CurrentSearch ?? string.Empty;
            if ( CustomFilter != null )
            {
                query = from q in query
                        where q != null
                        where CustomFilter( q, currentSearch )
                        select q;
            }
            else if ( Filter == AutocompleteFilter.Contains )
            {
                query = from q in query
                        let text = GetDisplayValue( q )
                        where text.IndexOf( currentSearch, 0, StringComparison.CurrentCultureIgnoreCase ) >= 0
                        select q;
            }
            else
            {
                query = from q in query
                        let text = GetDisplayValue( q )
                        where text.StartsWith( currentSearch, StringComparison.OrdinalIgnoreCase )
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
            CurrentSearch = string.Empty;
            SelectedText = string.Empty;
            SelectedValue = default;

            await SelectedValueChanged.InvokeAsync( selectedValue );
            await SearchChanged.InvokeAsync( CurrentSearch );
            await SelectedTextChanged.InvokeAsync( SelectedText );

        }

        private async Task UpdateActiveFilterIndex( int activeItemIndex )
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

                SelectedText = TextField?.Invoke( item ) ?? string.Empty;
                await SelectedTextChanged.InvokeAsync( SelectedText );
            }
        }

        /// <summary>
        /// Sets focus on the input element, if it can be focused.
        /// </summary>
        /// <param name="scrollToElement">If true the browser should scroll the document to bring the newly-focused element into view.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task Focus( bool scrollToElement = true )
        {
            return textEditRef.Focus( scrollToElement );
        }

        /// <summary>
        /// Determines if Autocomplete can be closed
        /// Only accounts for Escape Key, Lost focus is handled by the component onBlur event.
        /// </summary>
        /// <returns>True if Autocomplete can be closed.</returns>
        public Task<bool> IsSafeToClose( string elementId, CloseReason closeReason, bool isChild )
        {
            return Task.FromResult( ElementId == elementId && closeReason == CloseReason.EscapeClosing );
        }

        /// <inheritdoc/>
        public async Task Close( CloseReason closeReason )
        {
            await Clear();
            await UnregisterClosableComponent();
        }

        /// <summary>
        /// Unregisters the closable component.
        /// </summary>
        /// <returns></returns>
        protected async Task UnregisterClosableComponent()
        {
            if ( jsRegistered )
            {
                jsRegistered = false;

                await JSRunner.UnregisterClosableComponent( this );
            }
        }

        protected override async ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing )
            {
                var task = UnregisterClosableComponent();

                try
                {
                    await task;
                }
                catch when ( task.IsCanceled )
                {
                }

                dotNetObjectRef?.Dispose();
                dotNetObjectRef = null;
            }

            await base.DisposeAsync( disposing );
        }

        private string GetValidationValue()
        {
            return FreeTyping
                    ? Multiple
                        ? string.Join( ';', SelectedTexts )
                        : SelectedText?.ToString()
                    : SelectedValue?.ToString();
        }

        private string GetDisplayValue( TValue value )
        {
            var item = Data.FirstOrDefault( x => ValueField.Invoke( x ).Equals( value ) );
            return item is null
                ? string.Empty
                : GetDisplayValue( item );
        }

        private string GetDisplayValue( TItem item )
            => TextField.Invoke( item ) ?? string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the dropdown CSS styles.
        /// </summary>
        protected string CssStyle
        {
            get
            {
                var sb = new StringBuilder();

                if ( MaxMenuHeight != null )
                    sb.Append( $"--autocomplete-menu-max-height: {MaxMenuHeight};" );

                if ( Style != null )
                    sb.Append( Style );

                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the current search value.
        /// </summary>
        protected string CurrentSearch { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the currently active item index.
        /// </summary>
        protected int ActiveItemIndex { get; set; }

        /// <summary>
        /// Gets or sets the search field focus state.
        /// </summary>
        protected bool TextFocused { get; set; }

        /// <summary>
        /// True if the dropdown menu should be visible.
        /// </summary>
        protected bool DropdownVisible
            => CanSearch && TextField != null;

        /// <summary>
        /// True if the not found content should be visible.
        /// </summary>
        protected bool NotFoundVisible
            => FilteredData?.Count == 0 && IsTextSearchable && TextFocused && NotFoundContent != null;

        /// <summary>
        /// True if the component has the pre-requirements to search
        /// </summary>
        protected bool CanSearch
            => FilteredData?.Count > 0 && IsTextSearchable && TextFocused;

        /// <summary>
        /// True if the text complies to the search requirements
        /// </summary>
        protected bool IsTextSearchable
            => CurrentSearch?.Length >= MinLength;

        /// <summary>
        /// Gets the custom class-names for dropdown element.
        /// </summary>
        protected string DropdownClassNames
            => $"{Class} b-is-autocomplete {( Multiple ? "b-is-autocomplete-multipleselection" : string.Empty )} {( TextFocused ? "focus" : string.Empty )}";

        /// <summary>
        /// Gets or set the JavaScript runner.
        /// </summary>
        [Inject] protected IJSRunner JSRunner { get; set; }

        /// <summary>
        /// Gets or sets the dropdown element id.
        /// </summary>
        [Parameter] public string ElementId { get; set; }

        /// <summary>
        /// Defines the method by which the search will be done.
        /// </summary>
        [Parameter] public AutocompleteFilter Filter { get; set; } = AutocompleteFilter.StartsWith;

        /// <summary>
        /// The minimum number of characters a user must type before a search is performed.
        /// </summary>
        [Parameter] public int MinLength { get; set; } = 1;

        /// <summary>
        /// Sets the maximum height of the dropdown menu.
        /// </summary>
        [Parameter] public string MaxMenuHeight { get; set; }

        /// <summary>
        /// Sets the placeholder for the empty search.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }

        /// <summary>
        /// Size of a search field.
        /// </summary>
        [Parameter] public Size? Size { get; set; }

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
        /// Allows the value to not be on the data source.
        /// The value will be bound to the <see cref="SelectedText"/>
        /// </summary>
        [Parameter] public bool FreeTyping { get; set; }

        /// <summary>
        /// Gets or sets the currently selected item text.
        /// </summary>
        [Parameter] public string SelectedText { get; set; }

        /// <summary>
        /// Gets or sets the currently selected item text.
        /// </summary>
        [Parameter] public EventCallback<string> SelectedTextChanged { get; set; }

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
        /// Occurs on every search text change where the data does not contain the text being searched.
        /// </summary>
        [Parameter] public EventCallback<string> NotFound { get; set; }

        /// <summary>
        /// Custom class-name for dropdown element.
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
        /// If true the entered text will be slightly delayed before submitting it to the internal value.
        /// </summary>
        [Parameter] public bool? DelayTextOnKeyPress { get; set; }

        /// <summary>
        /// Interval in milliseconds that entered text will be delayed from submitting to the internal value.
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

        /// <summary>
        /// Specifies the not found content to be rendered inside this <see cref="Autocomplete{TItem, TValue}"/> when no data is found.
        /// </summary>
        [Parameter] public RenderFragment<string> NotFoundContent { get; set; }

        /// <summary>
        /// Handler for custom filtering on Autocomplete's data source.
        /// </summary>
        [Parameter] public Func<TItem, string, bool> CustomFilter { get; set; }

        /// <summary>
        /// Allows for multiple selection.
        /// </summary>
        [Parameter] public bool Multiple { get; set; }

        /// <summary>
        /// Sets the Badge color for the multiple selection values.
        /// Used when <see cref="Multiple"/> is true.
        /// </summary>
        [Parameter] public Color MultipleBadgeColor { get; set; } = Color.Primary;

        /// <summary>
        /// Currently selected items values.
        /// Used when <see cref="Multiple"/> is true.
        /// </summary>
        [Parameter] public List<TValue> SelectedValues { get; set; }

        /// <summary>
        /// Occurs after the selected values have changed.
        /// Used when <see cref="Multiple"/> is true.
        /// </summary>
        [Parameter] public EventCallback<List<TValue>> SelectedValuesChanged { get; set; }

        /// <summary>
        /// Currently selected items texts.
        /// Used when <see cref="Multiple"/> is true.
        /// </summary>
        [Parameter] public List<string> SelectedTexts { get; set; }

        /// <summary>
        /// Occurs after the selected texts have changed.
        /// Used when <see cref="Multiple"/> is true.
        /// </summary>
        [Parameter] public EventCallback<List<string>> SelectedTextsChanged { get; set; }


        public ElementReference ElementRef { get; }


        #endregion
    }
}
