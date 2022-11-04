#region Using directives
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.QRCode.Enums;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using QRCoder;
using static QRCoder.Base64QRCode;
#endregion

namespace Blazorise.QRCode
{
    /// <summary>
    /// Component used to generate QR code.
    /// </summary>
    public class QRCode : BaseComponent, IAsyncDisposable
    {
        #region Members

        private QRCodeGenerator generator;

        #endregion

        #region Methods

        /// <inheritdoc/>
        protected override ValueTask DisposeAsync( bool disposing )
        {
            if ( disposing )
            {
                generator?.Dispose();
            }

            return base.DisposeAsync( disposing );
        }

        /// <inheritdoc/>
        protected override void BuildRenderTree( RenderTreeBuilder builder )
        {
            builder.OpenElement( "img" )
                .Id( ElementId )
                .Attribute( "src", GetAsBase64Image() )
                .Attribute( "alt", Alt )
                .Class( ClassNames )
                .Style( StyleNames )
                .Attributes( Attributes )
                .CloseElement();
        }

        /// <summary>
        /// Generate QR Code image as base64 string
        /// </summary>
        public string GetAsBase64Image()
        {
            generator ??= new QRCodeGenerator();

            var eccLevel = EccLevel switch
            {
                EccLevel.L => QRCodeGenerator.ECCLevel.L,
                EccLevel.M => QRCodeGenerator.ECCLevel.M,
                EccLevel.Q => QRCodeGenerator.ECCLevel.Q,
                EccLevel.H => QRCodeGenerator.ECCLevel.H,
                _ => throw new ArgumentOutOfRangeException()
            };

            var darkColor = GetColor( DarkColor );
            var lightColor = GetColor( LightColor );

            using var data = Payload == null
                ? generator.CreateQrCode( Value, eccLevel )
                : generator.CreateQrCode( Payload.ToString(), eccLevel );

            var code = new Base64QRCode( data );

            if ( !string.IsNullOrEmpty( Icon ) )
            {
                using ( var image = GetImage( Icon ) )
                {
                    var imageType = GetImageType( IconImageType );

                    return $"data:image/png;base64, {code.GetGraphic( PixelsPerModule, darkColor, lightColor, image, IconSizePercent, IconBorderWidth, DrawQuietZones, imageType )}";
                }
            }

            return $"data:image/png;base64, {code.GetGraphic( PixelsPerModule, darkColor, lightColor, DrawQuietZones )}";
        }

        private static System.Drawing.Color GetColor( string color )
        {
            if ( HtmlColorCodeParser.TryParse( color, out var red, out var green, out var blue ) )
            {
                return System.Drawing.Color.FromArgb( byte.MaxValue, red, green, blue );
            }

            return System.Drawing.Color.Empty;
        }

        private static Bitmap GetImage( string base64 )
        {
            using var ms = new MemoryStream( Convert.FromBase64String( base64 ) );

            return new Bitmap( ms );
        }

        private static ImageType GetImageType( IconImageType iconImageType )
        {
            return iconImageType switch
            {
                IconImageType.Gif => ImageType.Gif,
                IconImageType.Jpeg => ImageType.Jpeg,
                _ => ImageType.Png,
            };
        }

        #endregion

        #region Properties

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
        /// The icon that is places in the middle of the QRCode, in base64 format.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// Defines how much space the icon will occupy within the QRCode.
        /// </summary>
        [Parameter] public int IconSizePercent { get; set; } = 15;

        /// <summary>
        /// Defines how large the borders will be for the icon that is placed within the QRCode.
        /// </summary>
        [Parameter] public int IconBorderWidth { get; set; } = 6;

        /// <summary>
        /// Defines the icon image file format.
        /// </summary>
        [Parameter] public IconImageType IconImageType { get; set; } = IconImageType.Png;

        #endregion
    }
}