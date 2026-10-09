#region Using directives
using System;
using System.Globalization;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Localization;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#endregion

namespace Blazorise;

/// <summary>
/// The editor that allows you to select a color from a dropdown menu.
/// </summary>
public partial class ColorPicker : BaseInputComponent<string, ColorPickerClasses, ColorPickerStyles>, IAsyncDisposable, ISelectableComponent
{
    #region Members

    private ColorPickerColor selectedColor = new( 0, 100, 100, 1 );

    private string colorBeforeOpen;

    private bool pickerOpen;

    private bool rgbaFormat;

    private string targetElementId;

    private PickerObserverCoordinator observerCoordinator;

    #endregion

    #region Constructors

    /// <summary>
    /// A default constructor.
    /// </summary>
    public ColorPicker()
    {
        PreviewClassBuilder = new( BuildPreviewClasses );
        SwatchClassBuilder = new( BuildSwatchClasses );
        PickerContainerClassBuilder = new( BuildPickerContainerClasses, builder => builder.Append( Classes?.Wrapper ) );
        PickerContainerStyleBuilder = new( BuildPickerContainerStyles, builder => builder.Append( Styles?.Wrapper ) );
        MenuClassBuilder = new( BuildMenuClasses );
        MenuStyleBuilder = new( BuildMenuStyles );
        SliderClassBuilder = new( BuildSliderClasses );
        SwatchStyleBuilder = new( BuildSwatchStyles );
        HueSliderStyleBuilder = new( BuildHueSliderStyles );
        OpacitySliderStyleBuilder = new( BuildOpacitySliderStyles );
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var valueChanged = !Rendered || parameters.IsParameterChanged( Value );
        var nextValue = parameters.TryGetValue<string>( nameof( Value ), out var paramValue ) ? paramValue : Value;
        var nextPalette = parameters.TryGetValue<string[]>( nameof( Palette ), out var paramPalette ) ? paramPalette : Palette;
        var nextDisabled = parameters.TryGetValue<bool>( nameof( Disabled ), out var paramDisabled ) ? paramDisabled : IsDisabled;
        var nextReadOnly = parameters.TryGetValue<bool>( nameof( ReadOnly ), out var paramReadOnly ) ? paramReadOnly : ReadOnly;

        if ( parameters.IsParameterChanged( Animated ) || parameters.IsParameterChanged( AnimationDuration ) )
        {
            MenuStyleBuilder.Dirty();
        }

        if ( valueChanged || ( string.IsNullOrEmpty( nextValue ) && parameters.TryGetValue<string[]>( nameof( Palette ), out _ ) ) )
        {
            SynchronizeColor( nextValue, nextPalette );
            colorBeforeOpen = nextValue;
        }

        if ( nextDisabled || nextReadOnly )
        {
            if ( IsPickerVisible && targetElementId is not null )
            {
                await JSUtilitiesModule.RestoreElement( MenuElementId );
            }

            pickerOpen = false;
        }

        if ( valueChanged )
        {
            InvalidateSwatchStyles( nextValue );
        }

        await base.SetParametersAsync( parameters );

        if ( !IsPickerVisible && observerCoordinator is not null )
        {
            await observerCoordinator.DisposeOutsideSubscriptionAsync();
        }
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing )
        {
            LocalizerService.LocalizationChanged -= OnLocalizationChanged;

            if ( observerCoordinator is not null )
            {
                await observerCoordinator.DisposeAsync();
            }

            if ( targetElementId is not null )
            {
                await JSUtilitiesModule.RestoreElement( MenuElementId );
            }
        }

        await base.DisposeAsync( disposing );
    }

    /// <inheritdoc/>
    protected override void BuildClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ColorPicker() );
        builder.Append( ClassProvider.ColorPickerSize( ThemeSize ) );

        base.BuildClasses( builder );
    }

    /// <summary>
    /// Builds the classes for the preview wrapper.
    /// </summary>
    protected virtual void BuildPreviewClasses( ClassBuilder builder ) => builder.Append( ClassProvider.ColorPickerPreview() );

    /// <summary>
    /// Builds the classes for the color swatch.
    /// </summary>
    protected virtual void BuildSwatchClasses( ClassBuilder builder ) => builder.Append( ClassProvider.ColorPickerSwatch() );

    private void BuildPickerContainerClasses( ClassBuilder builder )
    {
        builder.Append( ClassProvider.ColorPickerContainer() );
        AppendWrapperUtilities( builder );
    }

    private void BuildPickerContainerStyles( StyleBuilder builder ) => AppendWrapperUtilities( builder );

    private void BuildMenuClasses( ClassBuilder builder ) => builder.Append( ClassProvider.ColorPickerMenu() );

    private void BuildMenuStyles( StyleBuilder builder )
    {
        builder.Append( StyleProvider.ColorPickerMenuAnchor( targetElementId ) );
        builder.Append( StyleProvider.DropdownAnimationDuration( EffectiveAnimationDuration ) );
    }

    private void BuildSliderClasses( ClassBuilder builder ) => builder.Append( ClassProvider.ColorPickerSlider() );

    private void BuildSwatchStyles( StyleBuilder builder ) => builder.Append( $"background-color: {GetColorString( Value )};" );

    private void BuildHueSliderStyles( StyleBuilder builder ) => builder.Append( $"color: {CssColor.Hsl( selectedColor.Hue, 100, 50 )};" );

    private void BuildOpacitySliderStyles( StyleBuilder builder ) => builder.Append( $"color: {( selectedColor with { Alpha = 1 } ).ToHexString()};" );

    /// <inheritdoc/>
    protected internal override void DirtyClasses()
    {
        PreviewClassBuilder.Dirty();
        SwatchClassBuilder.Dirty();
        PickerContainerClassBuilder.Dirty();
        MenuClassBuilder.Dirty();
        SliderClassBuilder.Dirty();

        base.DirtyClasses();
    }

    /// <inheritdoc/>
    protected internal override void DirtyStyles()
    {
        PickerContainerStyleBuilder.Dirty();

        base.DirtyStyles();
    }

    /// <summary>
    /// Handles changes to the color input.
    /// </summary>
    protected Task OnChangeHandler( ChangeEventArgs eventArgs ) => SetValue( eventArgs?.Value?.ToString() );

    /// <summary>
    /// Handles activation of the picker trigger.
    /// </summary>
    protected Task OnClickHandler( MouseEventArgs eventArgs ) => IsPickerVisible ? Hide() : Show();

    /// <inheritdoc/>
    protected override async Task OnKeyDownHandler( KeyboardEventArgs eventArgs )
    {
        if ( eventArgs.Key == "Escape" && IsPickerVisible )
        {
            await Hide();
        }
        else if ( eventArgs.Key == "ArrowDown" && !IsPickerVisible )
        {
            await Show();
        }

        await base.OnKeyDownHandler( eventArgs );
    }

    internal async Task OnMenuKeyDownHandler( KeyboardEventArgs eventArgs )
    {
        if ( eventArgs.Key == "Escape" )
        {
            await Hide();
            await Focus( false );
        }
    }

    private Task OnOutsidePointerHandler( DocumentEventArgs eventArgs ) => Hide();

    internal async Task<bool> SelectSurfaceColor( double saturation, double brightness )
    {
        if ( IsInteractionDisabled || !IsPickerVisible )
        {
            return false;
        }

        var color = selectedColor with { Saturation = saturation, Brightness = brightness };

        if ( !SetSelectedColor( color ) )
        {
            return false;
        }

        await ApplySelectedColor();

        return true;
    }

    internal async Task OnHueChangedHandler( double hue )
    {
        if ( IsInteractionDisabled )
        {
            return;
        }

        SetSelectedColor( selectedColor with { Hue = Math.Clamp( hue, 0, 360 ) } );
        await ApplySelectedColor();
    }

    internal async Task OnOpacityChangedHandler( double opacity )
    {
        if ( IsInteractionDisabled )
        {
            return;
        }

        SetSelectedColor( selectedColor with { Alpha = Math.Clamp( opacity, 0, 1 ) } );
        await ApplySelectedColor();
    }

    internal Task OnInputChangedHandler( string text ) => SetValue( text );

    internal async Task OnSaveClickHandler( MouseEventArgs eventArgs )
    {
        if ( IsInteractionDisabled || !IsPickerVisible )
        {
            return;
        }

        if ( !GetColorString( Value ).IsEqual( selectedColor.ToHexString() ) )
        {
            await ApplySelectedColor();
        }

        await Hide();
        await Focus( false );
    }

    internal async Task OnClearClickHandler( MouseEventArgs eventArgs )
    {
        await SetValue( null );
        await Hide();
        await Focus( false );
    }

    internal async Task OnCancelClickHandler( MouseEventArgs eventArgs )
    {
        await SetValue( colorBeforeOpen );
        await Hide();
        await Focus( false );
    }

    internal void OnFormatChangedHandler( bool isRgbaFormat ) => rgbaFormat = isRgbaFormat;

    internal Task OnCopyClickHandler( MouseEventArgs eventArgs ) => JSUtilitiesModule.CopyToClipboard( default, InputElementId ).AsTask();

    internal async Task SelectPaletteColor( string value )
    {
        if ( IsInteractionDisabled || !ColorPickerColor.TryParse( value, out var color ) )
        {
            return;
        }

        await SetValue( color.ToHexString() );

        if ( HideAfterPaletteSelect )
        {
            await Hide();
            await Focus( false );
        }
    }

    private async Task ApplySelectedColor()
    {
        var value = selectedColor.ToHexString();

        if ( Value.IsEqual( value ) )
        {
            return;
        }

        InvalidateSwatchStyles( value );

        await CurrentValueHandler( value );
        await InvokeAsync( StateHasChanged );
    }

    private void SynchronizeColor( string value, string[] palette )
    {
        if ( ColorPickerColor.TryParse( value, out var color ) )
        {
            SetSelectedColor( color );
            return;
        }

        if ( palette is not null )
        {
            foreach ( var paletteColor in palette )
            {
                if ( ColorPickerColor.TryParse( paletteColor, out color ) )
                {
                    SetSelectedColor( color );
                    return;
                }
            }
        }

        SetSelectedColor( new( 0, 100, 100, 1 ) );
    }

    private bool SetSelectedColor( ColorPickerColor color )
    {
        if ( color == selectedColor )
        {
            return false;
        }

        if ( color.Hue != selectedColor.Hue )
        {
            HueSliderStyleBuilder.Dirty();
        }

        if ( color.Hue != selectedColor.Hue
            || color.Saturation != selectedColor.Saturation
            || color.Brightness != selectedColor.Brightness )
        {
            var previousColorString = ( selectedColor with { Alpha = 1 } ).ToHexString();
            var colorString = ( color with { Alpha = 1 } ).ToHexString();

            if ( !previousColorString.IsEqual( colorString ) )
            {
                OpacitySliderStyleBuilder.Dirty();
            }
        }

        selectedColor = color;

        return true;
    }

    private void InvalidateSwatchStyles( string value )
    {
        if ( !GetColorString( Value ).IsEqual( GetColorString( value ) ) )
        {
            SwatchStyleBuilder.Dirty();
        }
    }

    private void OnLocalizationChanged( object sender, EventArgs eventArgs )
    {
        if ( PickerLocalizer is null )
        {
            _ = InvokeAsync( StateHasChanged );
        }
    }

    /// <inheritdoc/>
    protected override string FormatValueAsString( string value ) => value;

    /// <inheritdoc/>
    protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
        => Task.FromResult( new ParseValue<string>( string.IsNullOrEmpty( value ) || ColorPickerColor.TryParse( value, out _ ), value, null ) );

    /// <inheritdoc/>
    public virtual Task Select( bool focus = true ) => JSUtilitiesModule.Select( ElementRef, ElementId, focus ).AsTask();

    /// <inheritdoc/>
    public override Task Focus( bool scrollToElement = true )
        => targetElementId is not null
            ? JSUtilitiesModule.Focus( default, targetElementId, scrollToElement ).AsTask()
            : base.Focus( scrollToElement );

    /// <summary>
    /// Opens the color picker when the input is enabled and editable.
    /// </summary>
    public virtual Task Show() => Show( null );

    /// <summary>
    /// Opens the color picker against the specified target, outside its containing menu.
    /// </summary>
    /// <param name="targetId">The element identifier to anchor the picker to and return focus to when closed.</param>
    public virtual async Task Show( string targetId )
    {
        if ( IsInteractionDisabled || IsPickerVisible )
        {
            return;
        }

        colorBeforeOpen = Value;
        SynchronizeColor( Value, Palette );

        var nextTargetElementId = string.IsNullOrWhiteSpace( targetId ) ? null : targetId;

        if ( targetElementId != nextTargetElementId )
        {
            targetElementId = nextTargetElementId;
            MenuStyleBuilder.Dirty();
        }

        pickerOpen = true;

        await InvokeAsync( StateHasChanged );

        await ObserverCoordinator.SynchronizeOutsideSubscriptionAsync( IsPickerVisible, false, ElementId, targetElementId ?? ElementId, MenuElementId, OnOutsidePointerHandler );
    }

    /// <summary>
    /// Closes the color picker.
    /// </summary>
    public virtual async Task Hide()
    {
        if ( !pickerOpen )
        {
            return;
        }

        if ( targetElementId is not null )
        {
            await JSUtilitiesModule.RestoreElement( MenuElementId );
        }

        pickerOpen = false;

        if ( observerCoordinator is not null )
        {
            await observerCoordinator.DisposeOutsideSubscriptionAsync();
        }

        await InvokeAsync( StateHasChanged );
    }

    internal async Task OnMenuRendered()
    {
        if ( !IsPickerVisible )
        {
            return;
        }

        if ( targetElementId is not null )
        {
            await JSUtilitiesModule.ShowAnchoredElement( MenuElementId, targetElementId );
        }

        if ( IsPickerVisible )
        {
            await JSUtilitiesModule.Focus( default, SurfaceElementId, false );
        }
    }

    /// <summary>
    /// Updates the picker with a new color value.
    /// </summary>
    /// <param name="value">New color value.</param>
    public async Task SetValue( string value )
    {
        if ( IsInteractionDisabled || Value.IsEqual( value ) )
        {
            return;
        }

        if ( !string.IsNullOrEmpty( value ) && !ColorPickerColor.TryParse( value, out _ ) )
        {
            return;
        }

        InvalidateSwatchStyles( value );

        await CurrentValueHandler( value );
        SynchronizeColor( Value, Palette );

        await InvokeAsync( StateHasChanged );
    }

    internal string Localize( string key ) => PickerLocalizer?.Invoke( key ) ?? Localizer.GetString( key );

    internal static string GetColorString( string value ) => ColorPickerColor.TryParse( value, out var color ) ? color.ToHexString() : "transparent";

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override string FieldLabelTargetElementId => null;

    /// <inheritdoc/>
    protected override bool UsesAutomaticAriaLabelledBy => true;

    /// <summary>
    /// Gets the disabled state serialized for the aria-disabled attribute.
    /// </summary>
    protected string AriaDisabledString => IsInteractionDisabled ? "true" : "false";

    /// <summary>
    /// Gets the value visibility serialized for provider styling.
    /// </summary>
    protected string ShowValueString => ShowValue ? "true" : "false";

    /// <summary>
    /// Gets the custom preview state serialized for provider styling.
    /// </summary>
    protected string CustomPreviewString => PreviewContent is not null ? "true" : "false";

    /// <summary>
    /// Gets the expanded state of the picker trigger.
    /// </summary>
    protected string AriaExpandedString => IsPickerVisible ? "true" : "false";

    internal bool IsInteractionDisabled => IsDisabled || ReadOnly;

    private PickerObserverCoordinator ObserverCoordinator => observerCoordinator ??= new( DocumentObserver );

    internal bool IsPickerVisible => pickerOpen && !IsInteractionDisabled;

    internal string MenuElementId => $"{ElementId}-menu";

    internal string SurfaceElementId => $"{ElementId}-surface";

    internal string HueSliderElementId => $"{ElementId}-hue";

    internal string OpacitySliderElementId => $"{ElementId}-opacity";

    internal string InputElementId => $"{ElementId}-input";

    internal string FormatSelectElementId => $"{ElementId}-format";

    internal string CopyButtonElementId => $"{ElementId}-copy";

    internal string ClearButtonElementId => $"{ElementId}-clear";

    internal string CancelButtonElementId => $"{ElementId}-cancel";

    internal string SaveButtonElementId => $"{ElementId}-save";

    internal string FirstControlElementId
    {
        get
        {
            if ( ShowHueSlider )
            {
                return HueSliderElementId;
            }

            if ( ShowOpacitySlider )
            {
                return OpacitySliderElementId;
            }

            if ( ShowPalette && Palette is not null )
            {
                for ( var index = 0; index < Palette.Length; index++ )
                {
                    if ( ColorPickerColor.TryParse( Palette[index], out _ ) )
                    {
                        return $"{ElementId}-palette-{index}";
                    }
                }
            }

            if ( ShowInputField )
            {
                return FormatSelectElementId;
            }

            if ( ShowClearButton )
            {
                return ClearButtonElementId;
            }

            if ( ShowCancelButton )
            {
                return CancelButtonElementId;
            }

            return ShowSaveButton ? SaveButtonElementId : ElementId;
        }
    }

    internal ColorPickerColor SelectedColor => selectedColor;

    internal double Hue => selectedColor.Hue;

    internal double Opacity => selectedColor.Alpha;

    internal string InputText
        => ColorPickerColor.TryParse( Value, out var color )
            ? rgbaFormat ? color.ToRgbaString() : color.ToHexString()
            : Value;

    internal bool IsRgbaFormat => rgbaFormat;

    /// <summary>
    /// Gets the duration override, or null to retain the provider's default timing.
    /// </summary>
    protected int? EffectiveAnimationDuration => !Animated ? 0 : AnimationDuration.HasValue ? Math.Max( 0, AnimationDuration.Value ) : null;

    internal string AnimationDurationString => EffectiveAnimationDuration?.ToString( CultureInfo.InvariantCulture );

    /// <summary>
    /// Gets the class builder for the preview wrapper.
    /// </summary>
    protected ClassBuilder PreviewClassBuilder { get; }

    /// <summary>
    /// Gets the class builder for the color swatch.
    /// </summary>
    protected ClassBuilder SwatchClassBuilder { get; }

    private ClassBuilder PickerContainerClassBuilder { get; }

    private StyleBuilder PickerContainerStyleBuilder { get; }

    private ClassBuilder MenuClassBuilder { get; }

    private StyleBuilder MenuStyleBuilder { get; }

    private ClassBuilder SliderClassBuilder { get; }

    private StyleBuilder SwatchStyleBuilder { get; }

    private StyleBuilder HueSliderStyleBuilder { get; }

    private StyleBuilder OpacitySliderStyleBuilder { get; }

    /// <summary>
    /// Gets the classes for the preview wrapper.
    /// </summary>
    protected string PreviewClassNames => PreviewClassBuilder.Class;

    /// <summary>
    /// Gets the classes for the color swatch.
    /// </summary>
    protected string SwatchClassNames => SwatchClassBuilder.Class;

    /// <summary>
    /// Gets the styles for the color swatch.
    /// </summary>
    protected string SwatchStyleNames => SwatchStyleBuilder.Styles;

    /// <summary>
    /// Gets the classes for the picker container.
    /// </summary>
    protected string PickerContainerClassNames => PickerContainerClassBuilder.Class;

    /// <summary>
    /// Gets the styles for the picker container.
    /// </summary>
    protected string PickerContainerStyleNames => PickerContainerStyleBuilder.Styles;

    internal string MenuClassNames => MenuClassBuilder.Class;

    internal string MenuStyleNames => MenuStyleBuilder.Styles;

    internal string SliderClassNames => SliderClassBuilder.Class;

    internal string HueSliderStyleNames => HueSliderStyleBuilder.Styles;

    internal string OpacitySliderStyleNames => OpacitySliderStyleBuilder.Styles;

    /// <summary>
    /// Specifies the DI registered localization service.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Specifies the DI registered color picker localizer.
    /// </summary>
    [Inject] protected ITextLocalizer<ColorPicker> Localizer { get; set; }

    /// <summary>
    /// Specifies the document observer used for outside interaction.
    /// </summary>
    [Inject] protected IDocumentObserver DocumentObserver { get; set; }

    /// <summary>
    /// Enables the opening animation supplied by the CSS provider. Set to false to open immediately.
    /// </summary>
    [Parameter] public bool Animated { get; set; } = true;

    /// <summary>
    /// Overrides the provider's opening animation duration, in milliseconds. Null preserves the provider's default.
    /// Zero or a negative value disables the animation.
    /// </summary>
    [Parameter] public int? AnimationDuration { get; set; }

    /// <summary>
    /// List a colors below the colorpicker to make it convenient for users to choose from
    /// frequently or recently used colors.
    /// </summary>
    [Parameter]
    public string[] Palette { get; set; } = new[]
    {
        "rgba(244, 67, 54, 1)",
        "rgba(233, 30, 99, 0.95)",
        "rgba(156, 39, 176, 0.9)",
        "rgba(103, 58, 183, 0.85)",
        "rgba(63, 81, 181, 0.8)",
        "rgba(33, 150, 243, 0.75)",
        "rgba(3, 169, 244, 0.7)",
        "rgba(0, 188, 212, 0.7)",
        "rgba(0, 150, 136, 0.75)",
        "rgba(76, 175, 80, 0.8)",
        "rgba(139, 195, 74, 0.85)",
        "rgba(205, 220, 57, 0.9)",
        "rgba(255, 235, 59, 0.95)",
        "rgba(255, 193, 7, 1)",
    };

    /// <summary>
    /// Controls the visibility of the palette below the colorpicker to make it convenient for users to
    /// choose from frequently or recently used colors.
    /// </summary>
    [Parameter] public bool ShowPalette { get; set; } = true;

    /// <summary>
    /// Automatically hides the dropdown menu after a palette color is selected.
    /// </summary>
    [Parameter] public bool HideAfterPaletteSelect { get; set; } = true;

    /// <summary>
    /// Controls the visibility of the clear buttons.
    /// </summary>
    [Parameter] public bool ShowClearButton { get; set; } = true;

    /// <summary>
    /// Controls the visibility of the cancel buttons.
    /// </summary>
    [Parameter] public bool ShowCancelButton { get; set; } = true;

    /// <summary>
    /// Controls the visibility of the save button that confirms the selected color and closes the picker.
    /// </summary>
    [Parameter] public bool ShowSaveButton { get; set; } = true;

    /// <summary>
    /// Controls the visibility of the opacity slider.
    /// </summary>
    [Parameter] public bool ShowOpacitySlider { get; set; } = true;

    /// <summary>
    /// Controls the visibility of the hue slider.
    /// </summary>
    [Parameter] public bool ShowHueSlider { get; set; } = true;

    /// <summary>
    /// Controls the visibility of the textbox which shows the selected color value.
    /// </summary>
    [Parameter] public bool ShowInputField { get; set; } = true;

    /// <summary>
    /// Controls the visibility of the selected color value beside the swatch in the closed picker.
    /// </summary>
    [Parameter] public bool ShowValue { get; set; } = true;

    /// <summary>
    /// Replaces the color swatch in the closed picker with custom content, such as an icon.
    /// </summary>
    /// <remarks>
    /// The content opens the color picker when activated. Use non-interactive content and supply an accessible name for the picker.
    /// <see cref="ShowValue"/> independently controls the selected color text. When omitted, the default swatch is displayed.
    /// </remarks>
    [Parameter] public RenderFragment PreviewContent { get; set; }

    /// <summary>
    /// Function used to handle custom localization that will override a default <see cref="ITextLocalizer"/>.
    /// </summary>
    [Parameter] public TextLocalizerHandler PickerLocalizer { get; set; }

    #endregion
}