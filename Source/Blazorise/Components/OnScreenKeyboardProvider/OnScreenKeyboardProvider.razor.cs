#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
#endregion

namespace Blazorise;

/// <summary>
/// Renders the on-screen keyboard controlled by <see cref="IOnScreenKeyboardService"/>.
/// </summary>
public partial class OnScreenKeyboardProvider : BaseComponent, IDisposable, IAsyncDisposable
{
    #region Members

    private bool shift;

    private bool specialCharacters;

    private bool closableLightRegistered;

    private ComponentParameterInfo<bool> paramShowSpecialCharactersKey;

    #endregion

    #region Constructors

    /// <summary>
    /// A default <see cref="OnScreenKeyboardProvider"/> constructor.
    /// </summary>
    public OnScreenKeyboardProvider()
    {
        RowClassBuilder = new( BuildRowClasses );
        KeyClassBuilder = new( BuildKeyClasses );
        Background = Blazorise.Background.Light;
        Border = GetKeyboardBorder( OnScreenKeyboardPlacement.Bottom );
        Padding = Blazorise.Padding.Is2;
        Shadow = GetKeyboardShadow( OnScreenKeyboardPlacement.Bottom );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override Task SetParametersAsync( ParameterView parameters )
    {
        bool backgroundDefined = parameters.TryGetValue<Background>( nameof( Background ), out _ );
        bool borderDefined = parameters.TryGetValue<IFluentBorder>( nameof( Border ), out _ );
        bool paddingDefined = parameters.TryGetValue<IFluentSpacing>( nameof( Padding ), out _ );
        bool shadowDefined = parameters.TryGetValue<Shadow>( nameof( Shadow ), out _ );
        OnScreenKeyboardPlacement? placement;
        parameters.TryGetParameter( ShowSpecialCharactersKey, out paramShowSpecialCharactersKey );
        OnScreenKeyboardPlacement effectivePlacement = parameters.TryGetValue<OnScreenKeyboardPlacement?>( nameof( Placement ), out placement )
            ? ResolvePlacement( placement )
            : EffectivePlacement;

        if ( !backgroundDefined )
            Background = Blazorise.Background.Light;

        if ( !borderDefined )
            Border = GetKeyboardBorder( effectivePlacement );

        if ( !paddingDefined )
            Padding = Blazorise.Padding.Is2;

        if ( !shadowDefined )
            Shadow = GetKeyboardShadow( effectivePlacement );

        return base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        OnScreenKeyboardService.StateChanged += OnKeyboardStateChanged;

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override void Dispose( bool disposing )
    {
        if ( disposing && OnScreenKeyboardService is not null )
        {
            OnScreenKeyboardService.StateChanged -= OnKeyboardStateChanged;
            _ = ClearScrollAdjustment();
            _ = UnregisterKeyboardAsClosableLight();
        }

        base.Dispose( disposing );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            if ( OnScreenKeyboardService is not null )
            {
                OnScreenKeyboardService.StateChanged -= OnKeyboardStateChanged;
            }

            await ClearScrollAdjustment();
            await UnregisterKeyboardAsClosableLight();
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.OnScreenKeyboard() );
        builder.Append( ClassProvider.OnScreenKeyboardPlacement( EffectivePlacement ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds the class names for a keyboard row.
    /// </summary>
    /// <param name="builder">Class builder used to append class names.</param>
    private void BuildRowClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.OnScreenKeyboardRow() );
    }

    /// <summary>
    /// Builds the class names for a keyboard key.
    /// </summary>
    /// <param name="builder">Class builder used to append class names.</param>
    private void BuildKeyClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.OnScreenKeyboardKey() );
    }

    /// <inheritdoc/>
    protected override void BuildStyles( StyleBuilder builder )
    {
        if ( EffectivePlacement != OnScreenKeyboardPlacement.Inline )
        {
            builder.Append( "position:fixed" );
            builder.Append( $"z-index:{EffectiveZIndex}" );
            builder.Append( "left:0", EffectiveKeyboardSize == OnScreenKeyboardSize.FullWidth );
            builder.Append( "right:0", EffectiveKeyboardSize == OnScreenKeyboardSize.FullWidth );
            builder.Append( "left:50%", EffectiveKeyboardSize != OnScreenKeyboardSize.FullWidth );
            builder.Append( "transform:translateX(-50%)", EffectiveKeyboardSize != OnScreenKeyboardSize.FullWidth );
            builder.Append( "width:calc(100% - 1rem)", EffectiveKeyboardSize != OnScreenKeyboardSize.FullWidth );
            builder.Append( KeyboardMaxWidthStyle, EffectiveKeyboardSize != OnScreenKeyboardSize.FullWidth );
            builder.Append( "bottom:0", EffectivePlacement == OnScreenKeyboardPlacement.Bottom );
            builder.Append( "top:0", EffectivePlacement == OnScreenKeyboardPlacement.Top );
        }
        else
        {
            builder.Append( "margin-left:auto;margin-right:auto", EffectiveKeyboardSize != OnScreenKeyboardSize.FullWidth );
            builder.Append( "width:100%", EffectiveKeyboardSize != OnScreenKeyboardSize.FullWidth );
            builder.Append( KeyboardMaxWidthStyle, EffectiveKeyboardSize != OnScreenKeyboardSize.FullWidth );
        }

        base.BuildStyles( builder );
    }

    private async void OnKeyboardStateChanged( object sender, OnScreenKeyboardStateChangedEventArgs eventArgs )
    {
        shift = false;
        specialCharacters = false;

        DirtyClasses();
        DirtyStyles();

        if ( eventArgs.State.Visible )
        {
            ExecuteAfterRender( RegisterKeyboardAsClosableLight );
            ExecuteAfterRender( ScrollActiveInputIntoView );
        }
        else
        {
            await ClearScrollAdjustment();
            await UnregisterKeyboardAsClosableLight();
        }

        await InvokeAsync( StateHasChanged );
    }

    private async Task RegisterKeyboardAsClosableLight()
    {
        if ( !Visible || closableLightRegistered || JSClosableModule is null )
            return;

        await JSClosableModule.RegisterLight( ElementRef );
        closableLightRegistered = true;
    }

    private async Task UnregisterKeyboardAsClosableLight()
    {
        if ( !closableLightRegistered || JSClosableModule is null )
            return;

        await JSClosableModule.UnregisterLight( ElementRef );
        closableLightRegistered = false;
    }

    private async Task ScrollActiveInputIntoView()
    {
        var context = OnScreenKeyboardService.State.Context;

        if ( !Visible
            || EffectivePlacement == OnScreenKeyboardPlacement.Inline
            || Options?.AccessibilityOptions?.OnScreenKeyboard?.AutoScroll == false
            || string.IsNullOrEmpty( context?.ElementId )
            || string.IsNullOrEmpty( ElementId )
            || UtilitiesModule is null )
        {
            return;
        }

        await UtilitiesModule.ScrollElementIntoViewForOnScreenKeyboard( context.ElementId, ElementId, EffectiveAutoScrollMargin );
    }

    private async Task ClearScrollAdjustment()
    {
        if ( UtilitiesModule is null )
            return;

        await UtilitiesModule.ClearOnScreenKeyboardScrollAdjustment();
    }

    private async Task OnKeyClicked( OnScreenKeyboardKey key )
    {
        OnScreenKeyboardService.SuppressHideOnBlur();

        if ( key.KeyType == OnScreenKeyboardKeyType.Shift )
        {
            shift = !shift;
            await InvokeAsync( StateHasChanged );
            return;
        }

        if ( key.KeyType == OnScreenKeyboardKeyType.SpecialCharacters )
        {
            shift = false;
            specialCharacters = !SpecialCharactersActive;
            await InvokeAsync( StateHasChanged );
            return;
        }

        if ( key.KeyType == OnScreenKeyboardKeyType.Text && shift )
        {
            await OnScreenKeyboardService.PressKey( new( GetShiftKeyText( key ) ) );
            return;
        }

        await OnScreenKeyboardService.PressKey( key );
    }

    private void OnKeyboardInteraction()
    {
        OnScreenKeyboardService.SuppressHideOnBlur();
    }

    private string GetKeyDisplayText( OnScreenKeyboardKey key )
    {
        if ( !string.IsNullOrEmpty( key.DisplayText ) )
            return key.DisplayText;

        if ( key.KeyType == OnScreenKeyboardKeyType.Text && shift )
            return GetShiftKeyText( key );

        if ( key.KeyType == OnScreenKeyboardKeyType.SpecialCharacters && SpecialCharactersActive )
            return "ABC";

        return key.Text;
    }

    private string GetKeyAriaLabel( OnScreenKeyboardKey key )
    {
        if ( !string.IsNullOrEmpty( key.AriaLabel ) )
            return key.AriaLabel;

        return key.KeyType switch
        {
            OnScreenKeyboardKeyType.Space => "Space",
            OnScreenKeyboardKeyType.Backspace => "Backspace",
            OnScreenKeyboardKeyType.Clear => "Clear",
            OnScreenKeyboardKeyType.Enter => "Enter",
            OnScreenKeyboardKeyType.Shift => shift ? "Shift enabled" : "Shift",
            OnScreenKeyboardKeyType.SpecialCharacters => SpecialCharactersActive ? "Letters" : "Special characters",
            _ => GetKeyDisplayText( key ),
        };
    }

    private string GetKeyStyle( OnScreenKeyboardKey key )
    {
        var width = Math.Max( 1, key.Width );

        return EffectiveKeyLayout == OnScreenKeyboardKeyLayout.Centered
            ? $"flex:0 0 {width * EffectiveKeyWidth}px; min-height:{EffectiveKeyMinHeight}px; max-width:100%"
            : $"flex:{width} 1 0; min-height:{EffectiveKeyMinHeight}px";
    }

    private bool IsShiftKeyActive( OnScreenKeyboardKey key )
    {
        return shift && key.KeyType == OnScreenKeyboardKeyType.Shift;
    }

    private static string GetShiftKeyText( OnScreenKeyboardKey key )
    {
        return key.ShiftText ?? key.Text?.ToUpperInvariant();
    }

    private bool IsKeyActive( OnScreenKeyboardKey key )
    {
        return IsShiftKeyActive( key )
            || ( SpecialCharactersActive && key.KeyType == OnScreenKeyboardKeyType.SpecialCharacters );
    }

    private OnScreenKeyboardKeyContext CreateKeyTemplateContext( OnScreenKeyboardKey key )
    {
        return new( key, GetKeyDisplayText( key ), GetKeyAriaLabel( key ), IsKeyActive( key ), shift, SpecialCharactersActive );
    }

    private IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> CreateRows()
    {
        var customRows = ResolveCustomRows();

        if ( customRows is not null )
            return customRows;

        if ( SpecialCharactersActive )
            return EffectiveSpecialCharactersRows;

        return CurrentLayout switch
        {
            OnScreenKeyboardLayout.Numeric => NumericRows,
            OnScreenKeyboardLayout.Decimal => DecimalRows,
            OnScreenKeyboardLayout.Telephone => TelephoneRows,
            OnScreenKeyboardLayout.Email => EmailRows,
            OnScreenKeyboardLayout.Url => UrlRows,
            _ => TextRows,
        };
    }

    private IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> ResolveCustomRows()
    {
        var context = OnScreenKeyboardService.State.Context;
        var layoutProvider = LayoutProvider ?? Options?.AccessibilityOptions?.OnScreenKeyboard?.LayoutProvider;

        return context is null || layoutProvider is null
            ? null
            : layoutProvider( context );
    }

    private static IReadOnlyList<OnScreenKeyboardKey> CreateTextRow( string keys )
    {
        return keys.Select( key => new OnScreenKeyboardKey( key.ToString() ) ).ToArray();
    }

    private static IReadOnlyList<OnScreenKeyboardKey> CreateKeysRow( params string[] keys )
    {
        return keys.Select( key => new OnScreenKeyboardKey( key ) ).ToArray();
    }

    private static OnScreenKeyboardKey TextKey( string text, string shiftText )
    {
        return new( text, shiftText );
    }

    private static OnScreenKeyboardKey CommandKey( OnScreenKeyboardKeyType keyType, string displayText, int width = 1 )
    {
        return new( keyType, displayText )
        {
            Width = width,
        };
    }

    private static IReadOnlyList<OnScreenKeyboardKey> WithSpecialCharactersKey( IReadOnlyList<OnScreenKeyboardKey> row )
    {
        return new[] { CommandKey( OnScreenKeyboardKeyType.SpecialCharacters, "?123", 2 ) }.Concat( row ).ToArray();
    }

    private static bool SupportsSpecialCharacters( OnScreenKeyboardLayout layout )
    {
        return layout == OnScreenKeyboardLayout.Text
            || layout == OnScreenKeyboardLayout.Email
            || layout == OnScreenKeyboardLayout.Url;
    }

    private OnScreenKeyboardPlacement ResolvePlacement( OnScreenKeyboardPlacement? placement )
    {
        return placement
            ?? Options?.AccessibilityOptions?.OnScreenKeyboard?.Placement
            ?? OnScreenKeyboardPlacement.Bottom;
    }

    private static IFluentBorder GetKeyboardBorder( OnScreenKeyboardPlacement placement )
    {
        return placement switch
        {
            OnScreenKeyboardPlacement.Top => Blazorise.Border.Is1.OnBottom,
            OnScreenKeyboardPlacement.Bottom => Blazorise.Border.Is1.OnTop,
            _ => Blazorise.Border.Is1.Rounded,
        };
    }

    private static Shadow GetKeyboardShadow( OnScreenKeyboardPlacement placement )
    {
        return placement == OnScreenKeyboardPlacement.Inline
            ? Blazorise.Shadow.None
            : Blazorise.Shadow.Default;
    }

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        RowClassBuilder.Dirty();
        KeyClassBuilder.Dirty();

        base.DirtyClasses();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the class builder for keyboard rows.
    /// </summary>
    protected ClassBuilder RowClassBuilder { get; }

    /// <summary>
    /// Gets the class builder for keyboard keys.
    /// </summary>
    protected ClassBuilder KeyClassBuilder { get; }

    private IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> Rows => CreateRows();

    private OnScreenKeyboardLayout CurrentLayout => OnScreenKeyboardService.State.Context?.Layout
        ?? Options?.AccessibilityOptions?.OnScreenKeyboard?.DefaultLayout
        ?? OnScreenKeyboardLayout.Text;

    private OnScreenKeyboardPlacement EffectivePlacement => ResolvePlacement( Placement );

    private OnScreenKeyboardSize EffectiveKeyboardSize => KeyboardSize
        ?? Options?.AccessibilityOptions?.OnScreenKeyboard?.KeyboardSize
        ?? OnScreenKeyboardSize.FullWidth;

    private OnScreenKeyboardKeyLayout EffectiveKeyLayout => KeyLayout
        ?? Options?.AccessibilityOptions?.OnScreenKeyboard?.KeyLayout
        ?? OnScreenKeyboardKeyLayout.Stretch;

    private bool EffectiveShowSpecialCharactersKey => paramShowSpecialCharactersKey.Defined
        ? paramShowSpecialCharactersKey.Value
        : Options?.AccessibilityOptions?.OnScreenKeyboard?.ShowSpecialCharactersKey == true;

    private bool SpecialCharactersActive => specialCharacters
        && EffectiveShowSpecialCharactersKey
        && SupportsSpecialCharacters( CurrentLayout );

    private IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> EffectiveSpecialCharactersRows => SpecialCharactersRows
        ?? Options?.AccessibilityOptions?.OnScreenKeyboard?.SpecialCharactersRows
        ?? DefaultSpecialCharactersRows;

    private bool Visible => OnScreenKeyboardService.State.Visible;

    private string PreviewValue => OnScreenKeyboardService.State.Context?.GetPreviewValue?.Invoke();

    private int? PreviewCaret => OnScreenKeyboardService.State.Context?.GetPreviewCaret?.Invoke();

    private int EffectivePreviewCaret => Math.Min( Math.Max( PreviewCaret.GetValueOrDefault(), 0 ), PreviewValue?.Length ?? 0 );

    private string PreviewValueBeforeCaret => PreviewValue?.Substring( 0, EffectivePreviewCaret );

    private string PreviewValueAfterCaret => PreviewValue?.Substring( EffectivePreviewCaret );

    private IFluentBorder PreviewBorder => Blazorise.Border.Is1.Secondary.Subtle;

    private string EffectiveDecimalSeparator => string.IsNullOrEmpty( OnScreenKeyboardService.State.Context?.DecimalSeparator )
        ? "."
        : OnScreenKeyboardService.State.Context.DecimalSeparator;

    private int EffectiveZIndex => ZIndex ?? StyleProvider.DefaultOnScreenKeyboardZIndex;

    private int EffectiveAutoScrollMargin => Math.Max( 0, Options?.AccessibilityOptions?.OnScreenKeyboard?.AutoScrollMargin ?? 12 );

    private string KeyboardMaxWidthStyle => EffectiveKeyboardSize switch
    {
        OnScreenKeyboardSize.Small => "max-width:48rem",
        OnScreenKeyboardSize.Medium => "max-width:72rem",
        OnScreenKeyboardSize.Large => "max-width:96rem",
        _ => null,
    };

    private IFluentFlex RowFlex => EffectiveKeyLayout == OnScreenKeyboardKeyLayout.Centered
        ? Blazorise.Flex.JustifyContent.Center
        : null;

    private int EffectiveKeyWidth => KeyWidth
        ?? Options?.AccessibilityOptions?.OnScreenKeyboard?.KeyWidth
        ?? 72;

    private int EffectiveKeyMinHeight => KeyMinHeight
        ?? Options?.AccessibilityOptions?.OnScreenKeyboard?.KeyMinHeight
        ?? ( EffectiveKeyLayout == OnScreenKeyboardKeyLayout.Centered ? 56 : 40 );

    /// <inheritdoc/>
    protected override bool ShouldAutoGenerateId => true;

    private bool UseDefaultKeyStyles => KeyColor == Color.Default;

    private TextColor KeyTextColor => UseDefaultKeyStyles
        ? Blazorise.TextColor.Body
        : Blazorise.TextColor.Default;

    private Background KeyBackground => UseDefaultKeyStyles
        ? Blazorise.Background.Body
        : Blazorise.Background.Default;

    private IFluentBorder KeyBorder => UseDefaultKeyStyles
        ? Blazorise.Border.Is1.Secondary.Subtle
        : null;

    private Shadow KeyShadow => UseDefaultKeyStyles
        ? Blazorise.Shadow.Small
        : Blazorise.Shadow.None;

    private IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> TextRows => new[]
    {
        CreateTextRow( "qwertyuiop" ),
        CreateTextRow( "asdfghjkl" ),
        new[] { CommandKey( OnScreenKeyboardKeyType.Shift, shift ? "SHIFT" : "Shift", 2 ) }.Concat( CreateTextRow( "zxcvbnm" ) ).Concat( new[] { CommandKey( OnScreenKeyboardKeyType.Backspace, "Backspace", 2 ) } ).ToArray(),
        EffectiveShowSpecialCharactersKey
            ? WithSpecialCharactersKey( new[] { TextKey( ",", ";" ), TextKey( ".", ":" ), TextKey( "-", "_" ), CommandKey( OnScreenKeyboardKeyType.Clear, "Clear", 2 ), CommandKey( OnScreenKeyboardKeyType.Space, "Space", 3 ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter", 2 ) } )
            : new[] { TextKey( ",", ";" ), TextKey( ".", ":" ), TextKey( "-", "_" ), CommandKey( OnScreenKeyboardKeyType.Clear, "Clear", 2 ), CommandKey( OnScreenKeyboardKeyType.Space, "Space", 4 ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter", 2 ) },
    };

    private IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> EmailRows => new[]
    {
        CreateTextRow( "qwertyuiop" ),
        CreateTextRow( "asdfghjkl" ),
        new[] { CommandKey( OnScreenKeyboardKeyType.Shift, shift ? "SHIFT" : "Shift", 2 ) }.Concat( CreateTextRow( "zxcvbnm" ) ).Concat( new[] { CommandKey( OnScreenKeyboardKeyType.Backspace, "Backspace", 2 ) } ).ToArray(),
        EffectiveShowSpecialCharactersKey
            ? WithSpecialCharactersKey( new[] { new OnScreenKeyboardKey( "@" ), new OnScreenKeyboardKey( "." ), CommandKey( OnScreenKeyboardKeyType.Space, "Space", 4 ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter", 2 ) } )
            : new[] { new OnScreenKeyboardKey( "@" ), new OnScreenKeyboardKey( "." ), CommandKey( OnScreenKeyboardKeyType.Space, "Space", 5 ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter", 2 ) },
    };

    private IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> UrlRows => new[]
    {
        CreateTextRow( "qwertyuiop" ),
        CreateTextRow( "asdfghjkl" ),
        new[] { CommandKey( OnScreenKeyboardKeyType.Shift, shift ? "SHIFT" : "Shift", 2 ) }.Concat( CreateTextRow( "zxcvbnm" ) ).Concat( new[] { CommandKey( OnScreenKeyboardKeyType.Backspace, "Backspace", 2 ) } ).ToArray(),
        EffectiveShowSpecialCharactersKey
            ? WithSpecialCharactersKey( new[] { new OnScreenKeyboardKey( "/" ), new OnScreenKeyboardKey( "." ), new OnScreenKeyboardKey( "-" ), CommandKey( OnScreenKeyboardKeyType.Space, "Space", 3 ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter", 2 ) } )
            : new[] { new OnScreenKeyboardKey( "/" ), new OnScreenKeyboardKey( "." ), new OnScreenKeyboardKey( "-" ), CommandKey( OnScreenKeyboardKeyType.Space, "Space", 4 ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter", 2 ) },
    };

    private static IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> DefaultSpecialCharactersRows => new[]
    {
        CreateKeysRow( "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" ),
        CreateKeysRow( "!", "@", "#", "$", "%", "^", "&", "*", "(", ")" ),
        CreateKeysRow( "-", "_", "=", "+", "[", "]", "{", "}", "\\", "|" ),
        CreateKeysRow( ".", ",", "?", "/", ":", ";", "'", "\"", "`", "~" ),
        new[] { CommandKey( OnScreenKeyboardKeyType.SpecialCharacters, "ABC", 2 ), CommandKey( OnScreenKeyboardKeyType.Space, "Space", 4 ), CommandKey( OnScreenKeyboardKeyType.Backspace, "Backspace", 2 ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter", 2 ) },
    };

    private static IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> NumericRows => new[]
    {
        CreateTextRow( "123" ),
        CreateTextRow( "456" ),
        CreateTextRow( "789" ),
        new[] { CommandKey( OnScreenKeyboardKeyType.Clear, "Clear" ), new OnScreenKeyboardKey( "0" ), CommandKey( OnScreenKeyboardKeyType.Backspace, "Backspace" ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter" ) },
    };

    private IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> DecimalRows => new[]
    {
        CreateTextRow( "123" ),
        CreateTextRow( "456" ),
        CreateTextRow( "789" ),
        new[] { CommandKey( OnScreenKeyboardKeyType.Clear, "Clear" ), new OnScreenKeyboardKey( "0" ), new OnScreenKeyboardKey( EffectiveDecimalSeparator ), CommandKey( OnScreenKeyboardKeyType.Backspace, "Backspace" ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter" ) },
    };

    private static IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> TelephoneRows => new[]
    {
        CreateTextRow( "123" ),
        CreateTextRow( "456" ),
        CreateTextRow( "789" ),
        new[] { new OnScreenKeyboardKey( "+" ), new OnScreenKeyboardKey( "0" ), CommandKey( OnScreenKeyboardKeyType.Backspace, "Backspace" ), CommandKey( OnScreenKeyboardKeyType.Enter, "Enter" ) },
    };

    /// <summary>
    /// Gets the service that controls the on-screen keyboard.
    /// </summary>
    [Inject] protected IOnScreenKeyboardService OnScreenKeyboardService { get; set; }

    /// <summary>
    /// Gets the JavaScript module used to cooperate with Blazorise closable components.
    /// </summary>
    [Inject] protected IJSClosableModule JSClosableModule { get; set; }

    /// <summary>
    /// Gets the JavaScript module used for DOM utilities.
    /// </summary>
    [Inject] protected IJSUtilitiesModule UtilitiesModule { get; set; }

    /// <summary>
    /// Gets the global Blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions Options { get; set; }

    /// <summary>
    /// Gets or sets the keyboard placement.
    /// </summary>
    [Parameter] public OnScreenKeyboardPlacement? Placement { get; set; }

    /// <summary>
    /// Gets or sets the CSS z-index used by the fixed keyboard placement. When not set, the current style provider default is used.
    /// </summary>
    [Parameter] public int? ZIndex { get; set; }

    /// <summary>
    /// Gets or sets the keyboard visual width.
    /// </summary>
    [Parameter] public OnScreenKeyboardSize? KeyboardSize { get; set; }

    /// <summary>
    /// Gets or sets the key arrangement inside keyboard rows.
    /// </summary>
    [Parameter] public OnScreenKeyboardKeyLayout? KeyLayout { get; set; }

    /// <summary>
    /// Gets or sets the button color.
    /// </summary>
    [Parameter] public Color KeyColor { get; set; } = Color.Secondary;

    /// <summary>
    /// Gets or sets whether key buttons should be outlined.
    /// </summary>
    [Parameter] public bool KeyOutline { get; set; } = true;

    /// <summary>
    /// Gets or sets the button size.
    /// </summary>
    [Parameter] public Size KeySize { get; set; } = Size.Default;

    /// <summary>
    /// Gets or sets whether text keyboards should show a key that toggles special characters.
    /// </summary>
    [Parameter] public bool ShowSpecialCharactersKey { get; set; }

    /// <summary>
    /// Gets or sets the rows used when the special characters keyboard is active.
    /// </summary>
    [Parameter] public IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>> SpecialCharactersRows { get; set; }

    /// <summary>
    /// Gets or sets a function that resolves keyboard rows for the active input context.
    /// </summary>
    [Parameter] public Func<OnScreenKeyboardContext, IReadOnlyList<IReadOnlyList<OnScreenKeyboardKey>>> LayoutProvider { get; set; }

    /// <summary>
    /// Gets or sets the tab index for rendered key buttons.
    /// </summary>
    [Parameter] public int? KeyTabIndex { get; set; } = -1;

    /// <summary>
    /// Gets or sets the base key width, in pixels, used by centered key layout.
    /// </summary>
    [Parameter] public int? KeyWidth { get; set; }

    /// <summary>
    /// Gets or sets the key minimum height, in pixels.
    /// </summary>
    [Parameter] public int? KeyMinHeight { get; set; }

    /// <summary>
    /// Gets or sets the keyboard aria-label.
    /// </summary>
    [Parameter] public string AriaLabel { get; set; } = "On-screen keyboard";

    /// <summary>
    /// Gets or sets the template used to render each keyboard key content.
    /// </summary>
    [Parameter] public RenderFragment<OnScreenKeyboardKeyContext> KeyTemplate { get; set; }

    /// <summary>
    /// Gets or sets custom keyboard content.
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    #endregion
}