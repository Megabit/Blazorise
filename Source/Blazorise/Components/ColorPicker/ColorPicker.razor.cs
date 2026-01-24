#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Localization;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// The editor that allows you to select a color from a dropdown menu.
/// </summary>
public partial class ColorPicker : BaseInputComponent<string, ColorPickerClasses, ColorPickerStyles>, ISelectableComponent, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Object reference that can be accessed through the JSInterop.
    /// </summary>
    private DotNetObjectReference<ColorPicker> dotNetObjectRef;

    /// <summary>
    /// Captured Palette parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<string[]> paramPalette;

    /// <summary>
    /// Captured ShowPalette parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramShowPalette;

    /// <summary>
    /// Captured HideAfterPaletteSelect parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramHideAfterPaletteSelect;

    /// <summary>
    /// Captured Disabled parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramDisabled;

    /// <summary>
    /// Captured ReadOnly parameter snapshot.
    /// </summary>
    protected ComponentParameterInfo<bool> paramReadOnly;

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void CaptureParameters( ParameterView parameters )
    {
        base.CaptureParameters( parameters );

        parameters.TryGetParameter( Palette, out paramPalette );
        parameters.TryGetParameter( ShowPalette, out paramShowPalette );
        parameters.TryGetParameter( HideAfterPaletteSelect, out paramHideAfterPaletteSelect );
        parameters.TryGetParameter( Disabled, out paramDisabled );
        parameters.TryGetParameter( ReadOnly, out paramReadOnly );
    }

    /// <inheritdoc/>
    protected override async Task OnBeforeSetParametersAsync( ParameterView parameters )
    {
        await base.OnBeforeSetParametersAsync( parameters );

        var paletteChanged = paramPalette.Defined && paramPalette.Changed;
        var showPaletteChanged = paramShowPalette.Defined && paramShowPalette.Changed;
        var hideAfterPaletteSelectChanged = paramHideAfterPaletteSelect.Defined && paramHideAfterPaletteSelect.Changed;
        var disabledChanged = paramDisabled.Defined && paramDisabled.Changed;
        var readOnlyChanged = paramReadOnly.Defined && paramReadOnly.Changed;

        if ( paramValue.Changed )
        {
            await CurrentValueHandler( paramValue.Value );

            if ( Rendered )
            {
                ExecuteAfterRender( async () => await JSModule.UpdateValue( ElementRef, ElementId, paramValue ) );
            }
        }

        if ( Rendered && ( paletteChanged
            || showPaletteChanged
            || hideAfterPaletteSelectChanged
            || disabledChanged
            || readOnlyChanged ) )
        {
            ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId,
            new ColorPickerUpdateJsOptions
            {
                Palette = new JSOptionChange<string[]>( paletteChanged, paramPalette.Value ),
                ShowPalette = new JSOptionChange<bool>( showPaletteChanged, paramShowPalette.Value ),
                HideAfterPaletteSelect = new JSOptionChange<bool>( hideAfterPaletteSelectChanged, paramHideAfterPaletteSelect.Value ),
                Disabled = new JSOptionChange<bool>( disabledChanged, paramDisabled.Value ),
                ReadOnly = new JSOptionChange<bool>( readOnlyChanged, paramReadOnly.Value )
            } ) );


        }
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        LocalizerService.LocalizationChanged += OnLocalizationChanged;

        base.OnInitialized();
    }

    /// <summary>
    /// Handles the localization changed event.
    /// </summary>
    /// <param name="sender">Object that raised the event.</param>
    /// <param name="eventArgs">Data about the localization event.</param>
    private async void OnLocalizationChanged( object sender, EventArgs eventArgs )
    {
        // no need to refresh if we're using custom localization
        if ( PickerLocalizer is not null )
            return;

        ExecuteAfterRender( async () => await JSModule.UpdateLocalization( ElementRef, ElementId, Localizer.GetStrings() ) );

        await InvokeAsync( StateHasChanged );
    }

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        dotNetObjectRef ??= CreateDotNetObjectRef( value: this );

        await JSModule.Initialize( dotNetObjectRef: dotNetObjectRef, elementRef: ElementRef, elementId: ElementId,
        options: new()
        {
            Default = Value,
            Palette = Palette,
            ShowPalette = ShowPalette,
            HideAfterPaletteSelect = HideAfterPaletteSelect,
            ShowClearButton = ShowClearButton,
            ShowCancelButton = ShowCancelButton,
            ShowOpacitySlider = ShowOpacitySlider,
            ShowHueSlider = ShowHueSlider,
            ShowInputField = ShowInputField,
            Disabled = Disabled,
            ReadOnly = ReadOnly,
            Localization = Localizer.GetStrings(),
            ColorPreviewElementSelector = ColorPreviewElementSelector,
            ColorValueElementSelector = ColorValueElementSelector
        } );


        await base.OnFirstAfterRenderAsync();
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDestroy( ElementRef, ElementId );

            DisposeDotNetObjectRef( dotNetObjectRef );
            dotNetObjectRef = null;

            LocalizerService.LocalizationChanged -= OnLocalizationChanged;
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
    /// Handles the input onchange event.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task OnChangeHandler( ChangeEventArgs eventArgs )
    {
        return CurrentValueHandler( eventArgs?.Value?.ToString() );
    }

    /// <inheritdoc/>
    protected override string FormatValueAsString( string value )
    {
        return value;
    }

    /// <inheritdoc/>
    protected override Task<ParseValue<string>> ParseValueFromStringAsync( string value )
    {
        return Task.FromResult( new ParseValue<string>( true, value, null ) );
    }

    /// <inheritdoc/>
    public virtual Task Select( bool focus = true )
    {
        return JSUtilitiesModule.Select( ElementRef, ElementId, focus ).AsTask();
    }

    /// <summary>
    /// Updated the <see cref="ColorPicker"/> with the new value.
    /// </summary>
    /// <param name="value">New color value.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [JSInvokable]
    public Task SetValue( string value )
    {
        if ( Value.IsEqual( value ) )
            return Task.CompletedTask;

        return CurrentValueHandler( value );
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the CSS selector for the color preview element.
    /// </summary>
    protected virtual string ColorPreviewElementSelector => ":scope > .b-input-color-picker-preview > .b-input-color-picker-curent-color";

    /// <summary>
    /// Gets the CSS selector for the color value element.
    /// </summary>
    protected virtual string ColorValueElementSelector => ":scope > .b-input-color-picker-preview > .b-input-color-picker-curent-value";

    /// <summary>
    /// Gets or sets the <see cref="IJSColorPickerModule"/> instance.
    /// </summary>
    [Inject] public IJSColorPickerModule JSModule { get; set; }

    /// <summary>
    /// Gets or sets the DI registered <see cref="ITextLocalizerService"/>.
    /// </summary>
    [Inject] protected ITextLocalizerService LocalizerService { get; set; }

    /// <summary>
    /// Gets or sets the DI registered <see cref="ITextLocalizer{ColorPicker}"/>.
    /// </summary>
    [Inject] protected ITextLocalizer<ColorPicker> Localizer { get; set; }

    /// <summary>
    /// List a colors below the colorpicker to make it convenient for users to choose from
    /// frequently or recently used colors.
    /// </summary>
    [Parameter]
    public string[] Palette { get; set; } = new string[]
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
        "rgba(255, 193, 7, 1)"
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
    /// Controls the visibility of the opacity slider.
    /// </summary>
    [Parameter] public bool ShowOpacitySlider { get; set; } = true;

    /// <summary>
    /// Controls the visibility of the hue slider.
    /// </summary>
    [Parameter] public bool ShowHueSlider { get; set; }

    /// <summary>
    /// Controls the visibility of the textbox which shows the selected color value.
    /// </summary>
    [Parameter] public bool ShowInputField { get; set; } = true;

    /// <summary>
    /// Function used to handle custom localization that will override a default <see cref="ITextLocalizer"/>.
    /// </summary>
    [Parameter] public TextLocalizerHandler PickerLocalizer { get; set; }

    #endregion
}