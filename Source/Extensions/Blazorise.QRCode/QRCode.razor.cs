#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
#endregion

namespace Blazorise.QRCode;

/// <summary>
/// Component used to generate QR code.
/// </summary>
public partial class QRCode : BaseComponent, IAsyncDisposable
{
    #region Methods

    /// <inheritdoc />
    public override async Task SetParametersAsync( ParameterView parameters )
    {
        if ( Rendered )
        {
            var valueChanged = parameters.TryGetValue<string>( nameof( Value ), out var paramValue ) && paramValue != Value;
            var eccLevelChanged = parameters.TryGetValue<EccLevel>( nameof( EccLevel ), out var paramEccLevel ) && paramEccLevel != EccLevel;
            var darkColorChanged = parameters.TryGetValue<string>( nameof( DarkColor ), out var paramDarkColor ) && paramDarkColor != DarkColor;
            var lightColorChanged = parameters.TryGetValue<string>( nameof( LightColor ), out var paramLightColor ) && paramLightColor != LightColor;
            var pixelsPerModuleChanged = parameters.TryGetValue<int>( nameof( PixelsPerModule ), out var paramPixelsPerModule ) && paramPixelsPerModule != PixelsPerModule;
            var drawQuietZonesChanged = parameters.TryGetValue<bool>( nameof( DrawQuietZones ), out var paramDrawQuietZones ) && paramDrawQuietZones != DrawQuietZones;
            var iconChanged = parameters.TryGetValue<string>( nameof( Icon ), out var paramIcon ) && paramIcon != Icon;
            var iconSizePercentageChanged = parameters.TryGetValue<int>( nameof( IconSizePercentage ), out var paramIconSizePercentage ) && paramIconSizePercentage != IconSizePercentage;
            var iconBorderWidthChanged = parameters.TryGetValue<int>( nameof( IconBorderWidth ), out var paramIconBorderWidth ) && paramIconBorderWidth != IconBorderWidth;

            if ( valueChanged
                 || eccLevelChanged
                 || darkColorChanged
                 || lightColorChanged
                 || pixelsPerModuleChanged
                 || drawQuietZonesChanged
                 || iconChanged
                 || iconSizePercentageChanged
                 || iconBorderWidthChanged )
            {
                ExecuteAfterRender( SynchronizeQRCode );
            }
        }

        await base.SetParametersAsync( parameters );
    }

    /// <inheritdoc/>
    protected override Task OnInitializedAsync()
    {
        if ( JSModule == null )
        {
            JSModule = new JSQRCodeModule( JSRuntime, VersionProvider, BlazoriseOptions );
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
    /// Synchronizes the state of QCode parameters with the JS.
    /// </summary>
    protected virtual async Task SynchronizeQRCode()
    {
        await JSModule.Update( ElementRef, ElementId, GetOptions() );
    }

    private QRCodeJSOptions GetOptions()
    {
        var eccLevel = EccLevel switch
        {
            EccLevel.L => "L",
            EccLevel.M => "M",
            EccLevel.Q => "Q",
            EccLevel.H => "H",
            _ => throw new ArgumentOutOfRangeException()
        };

        return new()
        {
            Value = Payload?.ToString() ?? Value,
            EccLevel = eccLevel,
            DarkColor = DarkColor,
            LightColor = LightColor,
            PixelsPerModule = PixelsPerModule,
            DrawQuietZones = DrawQuietZones,
            Icon = Icon,
            IconSizePercentage = IconSizePercentage,
            IconBorderWidth = IconBorderWidth,
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the <see cref="JSQRCodeModule"/> instance.
    /// </summary>
    protected JSQRCodeModule JSModule { get; private set; }

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
    /// Value used for QR code generation.
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <summary>
    /// Custom payload used for QR code generation.
    /// </summary>
    [Parameter] public PayloadGenerator.Payload Payload { get; set; }

    /// <summary>
    /// Image alt text.
    /// </summary>
    [Parameter] public string Alt { get; set; }

    /// <summary>
    /// The level of error correction to use.
    /// </summary>
    [Parameter] public EccLevel EccLevel { get; set; } = EccLevel.L;

    /// <summary>
    /// Color used as dark color.
    /// </summary>
    [Parameter] public string DarkColor { get; set; } = "#000000";

    /// <summary>
    /// Color used as light color.
    /// </summary>
    [Parameter] public string LightColor { get; set; } = "#ffffff";

    /// <summary>
    /// Pixels per module.
    /// </summary>
    [Parameter] public int PixelsPerModule { get; set; } = 10;

    /// <summary>
    /// Draw quiet zones (blank margins around QR Code image).
    /// </summary>
    [Parameter] public bool DrawQuietZones { get; set; } = true;

    /// <summary>
    /// The icon that is places in the middle of the QRCode, can be in base64 format or an absolute url.
    /// </summary>
    [Parameter] public string Icon { get; set; }

    /// <summary>
    /// Defines how much space the icon will occupy within the QRCode.
    /// </summary>
    [Parameter] public int IconSizePercentage { get; set; } = 40;

    /// <summary>
    /// Defines how large the borders will be for the icon that is placed within the QRCode.
    /// </summary>
    [Parameter] public int IconBorderWidth { get; set; } = 0;

    #endregion
}