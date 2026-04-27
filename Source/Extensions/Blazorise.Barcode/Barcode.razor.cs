#region Using directives
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.Barcode;

/// <summary>
/// Component used to generate barcodes.
/// </summary>
public partial class Barcode : BaseComponent, IAsyncDisposable
{
    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            var valueChanged = parameters.TryGetValue<string>( nameof( Value ), out var paramValue ) && paramValue != Value;
            var typeChanged = parameters.TryGetValue<BarcodeType>( nameof( Type ), out var paramType ) && paramType != Type;
            var renderModeChanged = parameters.TryGetValue<BarcodeRenderMode>( nameof( RenderMode ), out var paramRenderMode ) && paramRenderMode != RenderMode;
            var maxSymbolWidthChanged = parameters.TryGetValue<int?>( nameof( MaxSymbolWidth ), out var paramMaxSymbolWidth ) && paramMaxSymbolWidth != MaxSymbolWidth;
            var maxSymbolHeightChanged = parameters.TryGetValue<int?>( nameof( MaxSymbolHeight ), out var paramMaxSymbolHeight ) && paramMaxSymbolHeight != MaxSymbolHeight;
            var scaleChanged = parameters.TryGetValue<int>( nameof( Scale ), out var paramScale ) && paramScale != Scale;
            var foregroundColorChanged = parameters.TryGetValue<string>( nameof( ForegroundColor ), out var paramForegroundColor ) && paramForegroundColor != ForegroundColor;
            var backgroundColorChanged = parameters.TryGetValue<string>( nameof( BackgroundColor ), out var paramBackgroundColor ) && paramBackgroundColor != BackgroundColor;
            var showValueChanged = parameters.TryGetValue<bool>( nameof( ShowValue ), out var paramShowValue ) && paramShowValue != ShowValue;
            var valueAlignmentChanged = parameters.TryGetValue<BarcodeValueAlignment>( nameof( ValueAlignment ), out var paramValueAlignment ) && paramValueAlignment != ValueAlignment;
            var rotationChanged = parameters.TryGetValue<BarcodeRotation>( nameof( Rotation ), out var paramRotation ) && paramRotation != Rotation;
            var paddingTopChanged = parameters.TryGetValue<int?>( nameof( PaddingTop ), out var paramPaddingTop ) && paramPaddingTop != PaddingTop;
            var paddingRightChanged = parameters.TryGetValue<int?>( nameof( PaddingRight ), out var paramPaddingRight ) && paramPaddingRight != PaddingRight;
            var paddingBottomChanged = parameters.TryGetValue<int?>( nameof( PaddingBottom ), out var paramPaddingBottom ) && paramPaddingBottom != PaddingBottom;
            var paddingLeftChanged = parameters.TryGetValue<int?>( nameof( PaddingLeft ), out var paramPaddingLeft ) && paramPaddingLeft != PaddingLeft;
            var providerOptionsChanged = parameters.TryGetValue<Dictionary<string, object>>( nameof( ProviderOptions ), out var paramProviderOptions ) && paramProviderOptions != ProviderOptions;

            if ( valueChanged
                 || typeChanged
                 || renderModeChanged
                 || maxSymbolWidthChanged
                 || maxSymbolHeightChanged
                 || scaleChanged
                 || foregroundColorChanged
                 || backgroundColorChanged
                 || showValueChanged
                 || valueAlignmentChanged
                 || rotationChanged
                 || paddingTopChanged
                 || paddingRightChanged
                 || paddingBottomChanged
                 || paddingLeftChanged
                 || providerOptionsChanged )
            {
                ExecuteAfterRender( SynchronizeBarcode );
            }
        }

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if ( JSModule == null )
        {
            JSModule = new JSBarcodeModule( JSRuntime, VersionProvider, BlazoriseOptions );
        }

        return base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnFirstAfterRenderAsync()
    {
        await JSModule.Initialize( ElementRef, ElementId, GetOptions() );
    }

    /// <inheritdoc/>
    protected override async ValueTask DisposeAsync( bool disposing )
    {
        if ( disposing && Rendered )
        {
            await JSModule.SafeDisposeAsync();
        }

        await base.DisposeAsync( disposing );
    }

    /// <summary>
    /// Synchronizes the state of Barcode parameters with the JS.
    /// </summary>
    protected virtual async Task SynchronizeBarcode()
    {
        await JSModule.Update( ElementRef, ElementId, GetOptions() );
    }

    private BarcodeJSOptions GetOptions()
    {
        return new()
        {
            Value = Value,
            Type = Type.ToString(),
            RenderMode = RenderMode.ToString(),
            MaxSymbolWidth = MaxSymbolWidth,
            MaxSymbolHeight = MaxSymbolHeight,
            Scale = Scale,
            ForegroundColor = ForegroundColor,
            BackgroundColor = BackgroundColor,
            ShowValue = ShowValue,
            ValueAlignment = ValueAlignment.ToString(),
            Rotation = Rotation.ToString(),
            PaddingTop = PaddingTop,
            PaddingRight = PaddingRight,
            PaddingBottom = PaddingBottom,
            PaddingLeft = PaddingLeft,
            ProviderOptions = ProviderOptions,
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="JSBarcodeModule"/> instance.
    /// </summary>
    protected JSBarcodeModule JSModule { get; private set; }

    /// <summary>
    /// Gets or sets the JS runtime.
    /// </summary>
    [Inject] private IJSRuntime JSRuntime { get; set; }

    /// <summary>
    /// Gets or sets the version provider.
    /// </summary>
    [Inject] private IVersionProvider VersionProvider { get; set; }

    /// <summary>
    /// Gets or sets the blazorise options.
    /// </summary>
    [Inject] protected BlazoriseOptions BlazoriseOptions { get; set; }

    /// <summary>
    /// Value used for barcode generation.
    /// </summary>
    [Parameter, EditorRequired] public string Value { get; set; }

    /// <summary>
    /// The barcode symbology.
    /// </summary>
    [Parameter] public BarcodeType Type { get; set; } = BarcodeType.Code128;

    /// <summary>
    /// Defines how the barcode will be rendered.
    /// </summary>
    [Parameter] public BarcodeRenderMode RenderMode { get; set; } = BarcodeRenderMode.Canvas;

    /// <summary>
    /// Defines the maximum requested barcode symbol width.
    /// The provider preserves valid module sizing, so the rendered symbol can be smaller than this value.
    /// </summary>
    [Parameter] public int? MaxSymbolWidth { get; set; }

    /// <summary>
    /// Defines the maximum requested barcode symbol height.
    /// The provider preserves valid module sizing, so the rendered symbol can be smaller than this value.
    /// </summary>
    [Parameter] public int? MaxSymbolHeight { get; set; }

    /// <summary>
    /// Defines the barcode scale.
    /// </summary>
    [Parameter] public int Scale { get; set; } = 2;

    /// <summary>
    /// Color used as foreground color.
    /// </summary>
    [Parameter] public string ForegroundColor { get; set; } = "#000000";

    /// <summary>
    /// Color used as background color.
    /// </summary>
    [Parameter] public string BackgroundColor { get; set; } = "#ffffff";

    /// <summary>
    /// Defines whether the encoded value should be shown as human-readable text.
    /// </summary>
    [Parameter] public bool ShowValue { get; set; }

    /// <summary>
    /// Defines the alignment of the human-readable value text.
    /// </summary>
    [Parameter] public BarcodeValueAlignment ValueAlignment { get; set; } = BarcodeValueAlignment.Center;

    /// <summary>
    /// Defines the barcode rotation.
    /// </summary>
    [Parameter] public BarcodeRotation Rotation { get; set; } = BarcodeRotation.None;

    /// <summary>
    /// Defines the top padding.
    /// </summary>
    [Parameter] public int? PaddingTop { get; set; }

    /// <summary>
    /// Defines the right padding.
    /// </summary>
    [Parameter] public int? PaddingRight { get; set; }

    /// <summary>
    /// Defines the bottom padding.
    /// </summary>
    [Parameter] public int? PaddingBottom { get; set; }

    /// <summary>
    /// Defines the left padding.
    /// </summary>
    [Parameter] public int? PaddingLeft { get; set; }

    /// <summary>
    /// Provides additional provider-specific options for advanced scenarios.
    /// </summary>
    [Parameter] public Dictionary<string, object> ProviderOptions { get; set; }

    #endregion
}