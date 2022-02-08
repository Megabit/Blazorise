#region Using directives
using System;
using System.Threading.Tasks;
using Blazorise.Extensions;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using QRCoder;
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
            if ( Anchor )
            {
                builder.OpenElement( "a" )
                    .Attribute( "href", BuildUrl() )
                    .Attribute( "target", "_blank" );
            }

            builder.OpenElement( "img" )
                .Id( ElementId )
                .Attribute( "src", GetAsBase64Image() )
                .Attribute( "alt", Alt )
                .Class( ClassNames )
                .Style( StyleNames )
                .Attributes( Attributes )
                .CloseElement();

            if ( Anchor )
            {
                builder.CloseElement(); // </a>
            }
        }

        private string BuildUrl()
        {
            if ( Uri.IsWellFormedUriString( Text, UriKind.Absolute ) )
            {
                return Text;
            }

            var builder = new UriBuilder( Text );

            if ( ( builder.Scheme == Uri.UriSchemeHttp && builder.Port == 80 ) ||
                 ( builder.Scheme == Uri.UriSchemeHttps && builder.Port == 443 ) )
            {
                builder.Port = -1;
            }

            return builder.ToString();
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

            var darkColor = GetColorBytes( DarkColor );
            var lightColor = GetColorBytes( LightColor );

            using var data = Payload == null
                ? generator.CreateQrCode( Text, eccLevel )
                : generator.CreateQrCode( Payload, eccLevel );

            var code = new PngByteQRCode( data );
            var bytes = code.GetGraphic( PixelsPerModule, darkColor, lightColor, DrawQuietZones );

            const string pngData = "data:image/png;base64, ";
            return pngData + Convert.ToBase64String( bytes );
        }

        private static byte[] GetColorBytes( string color )
        {
            byte[] bytes = null;

            if ( HtmlColorCodeParser.TryParse( color, out var red, out var green, out var blue ) )
            {
                bytes = new[] { red, green, blue, byte.MaxValue };
            }

            return bytes;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Text input or URL for QR code generation
        /// </summary>
        [Parameter] public string Text { get; set; }

        /// <summary>
        /// Payload used of QR code generation.
        /// </summary>
        [Parameter] public PayloadGenerator.Payload Payload { get; set; }

        /// <summary>
        /// Image alt text.
        /// </summary>
        [Parameter] public string Alt { get; set; }

        /// <summary>
        /// Error correction level.
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
        /// Generate QR code image as anchor.
        /// </summary>
        [Parameter] public bool Anchor { get; set; }

        /// <summary>
        /// Pixels per module.
        /// </summary>
        [Parameter] public int PixelsPerModule { get; set; } = 10;

        /// <summary>
        /// Draw quiet zones (blank margins around QR Code image)
        /// </summary>
        [Parameter] public bool DrawQuietZones { get; set; } = true;

        #endregion
    }
}