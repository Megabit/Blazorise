#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blazorise.Components.Autocomplete;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Components
{
    /// <summary>
    /// The autocomplete is a normal text input enhanced by a panel of suggested options.
    /// </summary>
    /// <typeparam name="TItem">Type of an item filtered by the autocomplete component.</typeparam>
    /// <typeparam name="TValue">Type of an SelectedValue field.</typeparam>
    public partial class Autocomplete<TItem, TValue> : BaseAfterRenderComponent, ICloseActivator, IAsyncDisposable
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

        /// <summary>
        /// When CloseOnSelection is set to false, tracks whether the auto complete is in a state where it can actually close.
        /// </summary>
        private bool closeOnSelectionAllowClose = true;

        #endregion

        #region Methods

        protected async Task HandleReadData( CancellationToken cancellationToken = default )
        {
            try
            {
                Loading = true;

                if ( !cancellationToken.IsCancellationRequested && IsTextSearchable )
                    await ReadData.InvokeAsync( new( CurrentSearch, cancellationToken ) );
            }
            finally
            {
                Loading = false;

                await InvokeAsync( StateHasChanged );
            }
        }

        /// <summary>
        /// Triggers the reload of the <see cref="Autocomplete{TItem, TValue}"/> data.
        /// Makes sure not to reload if the <see cref="Autocomplete{TItem, TValue}"/> is in a loading state.
        /// </summary>
        /// <returns>Returns the awaitable task.</returns>
        public async Task Reload( CancellationToken cancellationToken = default )
        {
            if ( Loading )
                return;

            dirtyFilter = true;

            if ( ManualReadMode )
            {
                await InvokeAsync( () => HandleReadData( cancellationToken ) );
            }
            else
            {
                await InvokeAsync( StateHasChanged );
            }
        }

        /// <inheritdoc/>
        public override async Task SetParametersAsync( ParameterView parameters )
        {
            var selectedValueHasChanged = parameters.TryGetValue<TValue>( nameof( SelectedValue ), out var paramSelectedValue )
                && !paramSelectedValue.IsEqual( SelectedValue );

            var selectedValuesHaveChanged = parameters.TryGetValue<List<TValue>>( nameof( SelectedValues ), out var paramSelectedValues )
                && !paramSelectedValues.IsEqual( SelectedValues );

            var selectedTextsHaveChanged = parameters.TryGetValue<List<string>>( nameof( SelectedTexts ), out var paramSelectedTexts )
                && !paramSelectedTexts.IsEqual( SelectedTexts );

            List<TValue> staleValues = null;
            List<string> staleTexts = null;

            if ( selectedValuesHaveChanged && paramSelectedValues is not null && !SelectedValues.IsNullOrEmpty() )
                staleValues = SelectedValues.Where( x => !paramSelectedValues.Contains( x ) ).ToList();

            if ( selectedTextsHaveChanged && paramSelectedTexts is not null && !SelectedTexts.IsNullOrEmpty() )
                staleTexts = SelectedTexts.Where( x => !paramSelectedTexts.Contains( x ) ).ToList();

            await base.SetParametersAsync( parameters );

            // Override after parameters have already been set.
            // Avoids property setters running out of order
            if ( selectedValueHasChanged )
            {
                ExecuteAfterRender( async () =>
                {
                    var item = GetItemByValue( paramSelectedValue );

                    SelectedText = GetItemText( item );
                    await SelectedTextChanged.InvokeAsync( SelectedText );

                    if ( textEditRef != null )
                    {
                        await textEditRef.Revalidate();
                    }

                    await InvokeAsync( StateHasChanged );
                } );
            }

            if ( selectedValuesHaveChanged )
            {
                if ( SelectedValues is null )
                {
                    SelectedTexts?.Clear();

                }
                else
                {
                    if ( !staleValues.IsNullOrEmpty() )
                    {
                        foreach ( var staleValue in staleValues )
                            await RemoveMultipleText( GetItemText( staleValue ) );
                    }

                    foreach ( var selectedValue in SelectedValues )
                        await AddMultipleText( GetItemText( selectedValue ) );
                }

            }

            if ( selectedTextsHaveChanged )
            {
                if ( SelectedTexts is null )
                {
                    SelectedValues?.Clear();
                }
                else
                {
                    if ( !staleTexts.IsNullOrEmpty() )
                    {
                        foreach ( var staleText in staleTexts )
                            await RemoveMultipleValue( GetValueByText( staleText ) );
                    }

                    foreach ( var selectedText in SelectedTexts )
                        await AddMultipleValue( GetValueByText( selectedText ) );
                }
            }
        }

        /// <inheritdoc/>
        protected async override Task OnAfterRenderAsync( bool firstRender )
        {
            if ( firstRender )
            {
                dotNetObjectRef ??= DotNetObjectReference.Create( new CloseActivatorAdapter( this ) );

                if ( ManualReadMode )
                    await Reload();
            }
            await base.OnAfterRenderAsync( firstRender );
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

            Loading = true;
            await SearchChanged.InvokeAsync( CurrentSearch );
            Loading = false;

            await SelectedTextChanged.InvokeAsync( SelectedText );

            if ( ManualReadMode )
                await HandleReadData();

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
            if (Multiple && string.IsNullOrEmpty(SelectedText) && eventArgs.Code == "Backspace")
            {
                await RemoveMultipleText( SelectedTexts.LastOrDefault() );
                return;
            }

            if ( !DropdownVisible )
            {
                if ( IsConfirmKey( eventArgs ) )
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

            if ( IsConfirmKey( eventArgs ) )
            {
                var item = FilteredData.ElementAtOrDefault( ActiveItemIndex ?? -1 );

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
                await UpdateActiveFilterIndex( ( ActiveItemIndex ?? -1 ) - 1 );
            }
            else if ( eventArgs.Code == "ArrowDown" )
            {
                await UpdateActiveFilterIndex( ( ActiveItemIndex ?? -1 ) + 1 );
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
            if ( !CloseOnSelection )
                closeOnSelectionAllowClose = false;
            else
            {
                CurrentSearch = null;
                Loading = true;
                await SearchChanged.InvokeAsync( CurrentSearch );
                Loading = false;
            }

            SelectedValue = Converters.ChangeType<TValue>( value );
            await SelectedValueChanged.InvokeAsync( SelectedValue );


            if ( Multiple )
            {
                await AddMultipleText( selectedValue );
                await AddMultipleValue( selectedValue );
                await ResetSelectedText();
            }
            else
            {
                var item = Data.FirstOrDefault( x => ValueField( x ).IsEqual( value ) );

                SelectedText = GetItemText( item );
                await SelectedTextChanged.InvokeAsync( SelectedText );
            }

            if ( textEditRef != null )
            {
                await textEditRef.Revalidate();
            }
        }

        private bool IsConfirmKey( KeyboardEventArgs eventArgs )
        {
            if ( ConfirmKey.IsNullOrEmpty() )
                return false;

            return ConfirmKey.Contains( eventArgs.Code ) && !eventArgs.IsModifierKey();
        }

        private bool ShouldNotClose()
            => Multiple && !CloseOnSelection && !closeOnSelectionAllowClose && filteredData.Count > 0;

        private async Task ResetSelectedText()
        {
            if ( ShouldNotClose() )
            {
                dirtyFilter = true;
                await InvokeAsync( StateHasChanged );
            }
            else
            {
                SelectedText = string.Empty;
                await SelectedTextChanged.InvokeAsync( string.Empty );
            }
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
            => AddMultipleText( GetItemText( value ) );

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

            await RemoveMultipleValue( GetValueByText( text ) );
            await SelectedTextsChanged.InvokeAsync( SelectedTexts );

            dirtyFilter = true;
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

            if ( !ManualReadMode )
            {
                if ( Multiple )
                    query = query.Where( x => !SelectedValues.Contains( ValueField.Invoke( x ) ) );

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
                            let text = GetItemText( q )
                            where text.IndexOf( currentSearch, 0, StringComparison.CurrentCultureIgnoreCase ) >= 0
                            select q;
                }
                else
                {
                    query = from q in query
                            let text = GetItemText( q )
                            where text.StartsWith( currentSearch, StringComparison.OrdinalIgnoreCase )
                            select q;
                }
            }

            filteredData = query.ToList();
            ActiveItemIndex = AutoSelectFirstItem ? 0 : null;

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
                var item = FilteredData[ActiveItemIndex.Value];

                SelectedText = GetItemText( item );
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
            closeOnSelectionAllowClose = ( DropdownMenuRef.ElementId == elementId && closeReason == CloseReason.EscapeClosing ) ||
                ( closeReason == CloseReason.FocusLostClosing && !isChild );
            return Task.FromResult( closeOnSelectionAllowClose );
        }

        /// <inheritdoc/>
        public Task Close( CloseReason closeReason )
            => UnregisterClosableComponent();

        /// <summary>
        /// Unregisters the closable component.
        /// </summary>
        /// <returns></returns>
        protected async Task UnregisterClosableComponent()
        {
            if ( jsRegistered )
            {
                jsRegistered = false;

                await JSClosableModule.Unregister( this );
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

        private string GetItemText( TValue value )
        {
            var item = Data.FirstOrDefault( x => ValueField.Invoke( x ).Equals( value ) );

            return item is null
                ? string.Empty
                : GetItemText( item );
        }

        private string GetItemText( TItem item )
        {
            if ( item is null )
                return string.Empty;

            return TextField?.Invoke( item ) ?? string.Empty;
        }

        private TValue GetItemValue( TItem item )
        {
            if ( item is null || ValueField == null )
                return default;

            return ValueField.Invoke( item );
        }

        private TItem GetItemByValue( TValue value )
            => Data != null
                   ? Data.FirstOrDefault( x => ValueField( x ).IsEqual( value ) )
                   : default;

        private TValue GetValueByText( string text )
            => SelectedValues.FirstOrDefault( x => GetItemText( x ) == text );

        #endregion

        #region Properties

        /// <summary>
        /// True if user is using <see cref="ReadData"/> for loading the data.
        /// </summary>
        public bool ManualReadMode => ReadData.HasDelegate;

        /// <summary>
        /// Returns true if ReadData will be invoked.
        /// </summary>
        protected bool Loading { get; set; }

        /// <summary>
        /// Gets the DropdownMenu reference.
        /// </summary>
        public DropdownMenu DropdownMenuRef { get; set; }

        /// <summary>
        /// Gets the Element Reference
        /// </summary>
        public ElementReference ElementRef => DropdownMenuRef.ElementRef;

        /// <summary>
        /// Gets the DropdownMenu ElementId.
        /// </summary>
        public string DropdownElementId { get; set; }

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
        protected int? ActiveItemIndex { get; set; }

        /// <summary>
        /// Gets or sets the search field focus state.
        /// </summary>
        protected bool TextFocused { get; set; }

        /// <summary>
        /// True if the dropdown menu should be visible.
        /// Takes into account whether menu was open and whether CloseOnSelection is set to false.
        /// </summary>
        protected bool DropdownVisible
            => ( CanSearch || ShouldNotClose() ) && TextField != null;

        /// <summary>
        /// True if the not found content should be visible.
        /// </summary>
        protected bool NotFoundVisible
            => FilteredData?.Count == 0 && IsTextSearchable && TextFocused && NotFoundContent != null && !Loading;

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
        /// Gets or sets the <see cref="IJSClosableModule"/> instance.
        /// </summary>
        [Inject] public IJSClosableModule JSClosableModule { get; set; }

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
        [EditorRequired]
        [Parameter]
        public IEnumerable<TItem> Data
        {
            get { return data; }
            set
            {
                if ( data.IsEqual( value ) )
                    return;
                data = value;

                // make sure everything is recalculated
                dirtyFilter = true;
            }
        }

        /// <summary>
        /// Event handler used to load data manually based on the current search value.
        /// </summary>
        [Parameter] public EventCallback<AutocompleteReadDataEventArgs> ReadData { get; set; }

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
        [EditorRequired]
        [Parameter]
        public Func<TItem, string> TextField { get; set; }

        /// <summary>
        /// Method used to get the value field from the supplied data source.
        /// </summary>
        [EditorRequired]
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
        /// Note that setting this will override global settings in <see cref="BlazoriseOptions.Immediate"/>.
        /// </remarks>
        [Parameter] public bool? Immediate { get; set; }

        /// <summary>
        /// If true the entered text will be slightly delayed before submitting it to the internal value.
        /// </summary>
        [Parameter] public bool? Debounce { get; set; }

        /// <summary>
        /// Interval in milliseconds that entered text will be delayed from submitting to the internal value.
        /// </summary>
        [Parameter] public int? DebounceInterval { get; set; }

        /// <summary>
        /// If defined, indicates that its element can be focused and can participates in sequential keyboard navigation.
        /// </summary>
        [Parameter] public int? TabIndex { get; set; }

        /// <summary>
        /// Validation handler used to validate selected value.
        /// </summary>
        [Parameter] public Action<ValidatorEventArgs> Validator { get; set; }

        /// <summary>
        /// Asynchronously validates the selected value.
        /// </summary>
        [Parameter] public Func<ValidatorEventArgs, CancellationToken, Task> AsyncValidator { get; set; }

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

        /// <summary>
        /// Specifies the item content to be rendered inside each dropdown item.
        /// </summary>
        [Parameter] public RenderFragment<ItemContext<TItem, TValue>> ItemContent { get; set; }

        /// <summary>
        /// Specifies whether <see cref="Autocomplete{TItem, TValue}"/> dropdown closes on selection. This is only evaluated when the <see cref="Multiple"/> is set to true.
        /// Defauls to true.
        /// </summary>
        [Parameter] public bool CloseOnSelection { get; set; } = true;

        /// <summary>
        /// Gets or sets an array of the keyboard pressed values for the ConfirmKey.
        /// If this is null or empty, there will be no confirmation key.
        /// <para>Defauls to: { "Enter", "NumpadEnter", "Tab" }.</para>
        /// </summary>
        /// <remarks>
        /// If the value has a printed representation, this attribute's value is the same as the char attribute.
        /// Otherwise, it's one of the key value strings specified in 'Key values'.
        /// </remarks>
        [Parameter] public string[] ConfirmKey { get; set; } = new[] { "Enter", "NumpadEnter", "Tab" };

        /// <summary>
        /// Gets or sets whether <see cref="Autocomplete{TItem, TValue}"/> auto selects the first item displayed on the dropdown.
        /// Defauls to true.
        /// </summary>
        [Parameter] public bool AutoSelectFirstItem { get; set; } = true;

        #endregion
    }
}
