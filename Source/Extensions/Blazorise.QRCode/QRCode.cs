using System;
using System.Threading.Tasks;
using Blazorise.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using QRCoder;

namespace Blazorise.QRCode;

public class QRCode : BaseComponent
{
    private QRCodeGenerator generator;

    protected override ValueTask DisposeAsync( bool disposing )
    {
        generator?.Dispose();
        return base.DisposeAsync( disposing );
    }

    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        int sequence = -1;

        if ( AsAnchor )
        {
            builder.OpenElement( sequence++, "a" );
            builder.AddAttribute( sequence++, "href", GetUrl() );
            builder.AddAttribute( sequence++, "target", "_blank" );
        }

        builder.OpenElement( sequence++, "img" );
        builder.AddAttribute( sequence++, "src", BuildQrCode() );

        if ( !string.IsNullOrWhiteSpace( Alt ) )
        {
            builder.AddAttribute( sequence++, "alt", Alt );
        }

        builder.CloseElement();

        if ( AsAnchor )
        {
            builder.CloseElement();
        }
    }

    private string GetUrl()
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

    private string BuildQrCode()
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

        using var data = generator.CreateQrCode( Text, eccLevel );
        var code = new PngByteQRCode( data );
        var bytes = code.GetGraphic( 5, darkColor, lightColor );

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

    [Parameter]
    public string Text { get; set; }

    [Parameter]
    public string Alt { get; set; }

    [Parameter]
    public EccLevel EccLevel { get; set; } = EccLevel.L;

    [Parameter]
    public string DarkColor { get; set; } = "black";

    [Parameter]
    public string LightColor { get; set; } = "white";

    [Parameter]
    public bool AsAnchor { get; set; }
}