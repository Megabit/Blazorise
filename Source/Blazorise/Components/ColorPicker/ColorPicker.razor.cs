﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Localization;
using Blazorise.Modules;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
#endregion

namespace Blazorise;

/// <summary>
/// The editor that allows you to select a color from a dropdown menu.
/// </summary>
public partial class ColorPicker : BaseInputComponent<string>, ISelectableComponent, IAsyncDisposable
{
    #region Members

    /// <summary>
    /// Object reference that can be accessed through the JSInterop.
    /// </summary>
    private DotNetObjectReference<ColorPicker> dotNetObjectRef;

    #endregion

    #region Methods

    /// <inheritdoc/>
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        var colorChanged = parameters.TryGetValue<string>( nameof( Color ), out var color ) && !Color.IsEqual( color );
        var paletteChanged = parameters.TryGetValue( nameof( Palette ), out string[] palette ) && !Palette.AreEqual( palette );
        var showPaletteChanged = parameters.TryGetValue( nameof( ShowPalette ), out bool showPalette ) && ShowPalette != showPalette;
        var hideAfterPaletteSelectChanged = parameters.TryGetValue( nameof( HideAfterPaletteSelect ), out bool hideAfterPaletteSelect ) && HideAfterPaletteSelect != hideAfterPaletteSelect;
        var disabledChanged = parameters.TryGetValue( nameof( Disabled ), out bool disabled ) && Disabled != disabled;
        var readOnlyChanged = parameters.TryGetValue( nameof( ReadOnly ), out bool readOnly ) && ReadOnly != readOnly;

        if ( colorChanged )
        {
            await CurrentValueHandler( color );

            if ( Rendered )
            {
                ExecuteAfterRender( async () => await JSModule.UpdateValue( ElementRef, ElementId, color ) );
            }
        }

        if ( Rendered && ( paletteChanged
                           || showPaletteChanged
                           || hideAfterPaletteSelectChanged
                           || disabledChanged
                           || readOnlyChanged ) )
        {
            ExecuteAfterRender( async () => await JSModule.UpdateOptions( ElementRef, ElementId, new
            {
                Palette = new { Changed = paletteChanged, Value = palette },
                ShowPalette = new { Changed = showPaletteChanged, Value = showPalette },
                HideAfterPaletteSelect = new { Changed = hideAfterPaletteSelectChanged, Value = hideAfterPaletteSelect },
                Disabled = new { Changed = disabledChanged, Value = disabled },
                ReadOnly = new { Changed = readOnlyChanged, Value = readOnly },
            } ) );
        }

        await base.SetParametersAsync( parameters );

        if ( ParentValidation != null )
        {
            if ( parameters.TryGetValue<Expression<Func<string>>>( nameof( ColorExpression ), out var expression ) )
                await ParentValidation.InitializeInputExpression( expression );

            await InitializeValidation();
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
        if ( PickerLocalizer != null )
            return;

        ExecuteAfterRender( async () => await JSModule.UpdateLocalization( ElementRef, ElementId, Localizer.GetStrings() ) );

        await InvokeAsync( StateHasChanged );
    }

    /// <inheritdoc/>
    protected override async Task OnFirstAfterRenderAsync()
    {
        dotNetObjectRef ??= CreateDotNetObjectRef( this );

        await JSModule.Initialize( dotNetObjectRef, ElementRef, ElementId, new
        {
            Default = Color,
            Palette,
            ShowPalette,
            HideAfterPaletteSelect,
            ShowClearButton,
            ShowCancelButton,
            Disabled,
            ReadOnly,
            Localization = Localizer.GetStrings(),
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
    protected override Task OnInternalValueChanged( string value )
    {
        return ColorChanged.InvokeAsync( value );
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
        if ( Color.IsEqual( value ) )
            return Task.CompletedTask;

        return CurrentValueHandler( value );
    }

    #endregion

    #region Properties

    /// <inheritdoc/>
    protected override string InternalValue { get => Color; set => Color = value; }

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
    /// Gets or sets the input color value.
    /// </summary>
    [Parameter] public string Color { get; set; } = "#000000";

    /// <summary>
    /// Occurs when the color has changed.
    /// </summary>
    [Parameter] public EventCallback<string> ColorChanged { get; set; }

    /// <summary>
    /// Gets or sets an expression that identifies the color value.
    /// </summary>
    [Parameter] public Expression<Func<string>> ColorExpression { get; set; }

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
    /// Function used to handle custom localization that will override a default <see cref="ITextLocalizer"/>.
    /// </summary>
    [Parameter] public TextLocalizerHandler PickerLocalizer { get; set; }

    #endregion
}