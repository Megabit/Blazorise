using System.Threading.Tasks;
using Blazorise.Cropper;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using CropperComponent = Blazorise.Cropper.Cropper;
using QRCodeComponent = Blazorise.QRCode.QRCode;

namespace Blazorise.Tests.Components;

public class ComponentRenderingTest : BunitContext
{
    public ComponentRenderingTest()
    {
        Services.AddBlazoriseTests().AddBootstrapProviders().AddEmptyIconProvider().AddTestData();
        JSInterop
            .AddBlazoriseTextInput()
            .AddBlazoriseButton();
    }

    [Fact]
    public void CanRenderTextOnlyComponent()
    {
        // setup

        // test
        var appElement = Render<TextOnlyComponent>();

        // validate
        Assert.Contains( "Hello from TextOnlyComponent", appElement.Markup );
    }

    [Fact]
    public void CanRenderButtonComponent()
    {
        // setup
        var buttonOpen = "<button";
        var buttonClose = "</button>";
        var buttonType = @"type=""button""";
        var buttonContent = "hello primary";

        // test
        var comp = Render<ButtonOnlyComponent>();

        // validate
        this.JSInterop.VerifyNotInvoke( "initialize" );
        Assert.Contains( buttonOpen, comp.Markup );
        Assert.Contains( buttonClose, comp.Markup );
        Assert.Contains( buttonType, comp.Markup );
        Assert.Contains( buttonContent, comp.Markup );
    }

    [Fact]
    public async Task CannotChangeElementId()
    {
        // setup
        var comp = Render<ElementIdComponent>();
        var date = comp.Find( "input" );
        var button = comp.Find( "button" );

        Assert.NotEqual( string.Empty, date.GetAttribute( "id" ) );

        // test
        var before = date.GetAttribute( "id" );
        await button.ClickAsync();

        // validate
        this.JSInterop.VerifyNotInvoke( "initialize" );
        Assert.Equal( before, date.GetAttribute( "id" ) );
    }

    [Fact]
    public void ImageText_ShouldRenderAltAttribute()
    {
        var component = Render<Image>( parameters => parameters
            .Add( x => x.Text, "Image text" ) );

        Assert.Equal( "Image text", component.Find( "img" ).GetAttribute( "alt" ) );
    }

    [Fact]
    public void CardImageText_ShouldRenderAltAttribute()
    {
        var component = Render<CardImage>( parameters => parameters
            .Add( x => x.Text, "Card image text" ) );

        Assert.Equal( "Card image text", component.Find( "img" ).GetAttribute( "alt" ) );
    }

    [Fact]
    public void CardImageLegacyAlt_ShouldRenderAltAttribute()
    {
#pragma warning disable CS0618
        var component = Render<CardImage>( parameters => parameters
            .Add( x => x.Alt, "Legacy card image text" ) );
#pragma warning restore CS0618

        Assert.Equal( "Legacy card image text", component.Find( "img" ).GetAttribute( "alt" ) );
    }

    [Fact]
    public void FigureImageText_ShouldRenderAltAttribute()
    {
        var component = Render<FigureImage>( parameters => parameters
            .Add( x => x.Text, "Figure image text" ) );

        Assert.Equal( "Figure image text", component.Find( "img" ).GetAttribute( "alt" ) );
    }

    [Fact]
    public void FigureImageLegacyAlternateText_ShouldRenderAltAttribute()
    {
#pragma warning disable CS0618
        var component = Render<FigureImage>( parameters => parameters
            .Add( x => x.AlternateText, "Legacy figure image text" ) );
#pragma warning restore CS0618

        Assert.Equal( "Legacy figure image text", component.Find( "img" ).GetAttribute( "alt" ) );
    }

    [Fact]
    public void QRCodeText_ShouldRenderAccessibleName()
    {
        var version = Services.GetRequiredService<IVersionProvider>().Version;
        var module = JSInterop.SetupModule( $"./_content/Blazorise.QRCode/blazorise.qrcode.js?v={version}" );
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();

        var component = Render<QRCodeComponent>( parameters => parameters
            .Add( x => x.Text, "QR code text" ) );

        var image = component.Find( "[role='img']" );
        Assert.Equal( "QR code text", image.GetAttribute( "aria-label" ) );
    }

    [Fact]
    public void QRCodeLegacyAlt_ShouldRenderAccessibleName()
    {
        var version = Services.GetRequiredService<IVersionProvider>().Version;
        var module = JSInterop.SetupModule( $"./_content/Blazorise.QRCode/blazorise.qrcode.js?v={version}" );
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();

#pragma warning disable CS0618
        var component = Render<QRCodeComponent>( parameters => parameters
            .Add( x => x.Alt, "Legacy QR code text" ) );
#pragma warning restore CS0618

        var image = component.Find( "[role='img']" );
        Assert.Equal( "Legacy QR code text", image.GetAttribute( "aria-label" ) );
    }

    [Fact]
    public void CropperTextChanges_ShouldUpdateImageAltOption()
    {
        var version = Services.GetRequiredService<IVersionProvider>().Version;
        var module = JSInterop.SetupModule( $"./_content/Blazorise.Cropper/blazorise.cropper.js?v={version}" );
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();
        module.SetupVoid( "updateOptions", _ => true ).SetVoidResult();

        var component = Render<CropperComponent>( parameters => parameters
            .Add( x => x.Source, "image.png" )
            .Add( x => x.Text, "Initial image text" ) );

        var initializeInvocation = JSInterop.VerifyInvoke( "initialize" );
        var initializeOptions = Assert.IsType<CropperJSOptions>( initializeInvocation.Arguments[3] );
        Assert.Equal( "Initial image text", initializeOptions.Alt );

        component.Render( parameters => parameters
            .Add( x => x.Source, "image.png" )
            .Add( x => x.Text, "Updated image text" ) );

        var updateInvocation = JSInterop.VerifyInvoke( "updateOptions" );
        var updateOptions = Assert.IsType<CropperUpdateJSOptions>( updateInvocation.Arguments[2] );
        Assert.True( updateOptions.Alt.Changed );
        Assert.Equal( "Updated image text", updateOptions.Alt.Value );
    }

    [Fact]
    public void CropperLegacyAltChanges_ShouldUpdateImageAltOption()
    {
        var version = Services.GetRequiredService<IVersionProvider>().Version;
        var module = JSInterop.SetupModule( $"./_content/Blazorise.Cropper/blazorise.cropper.js?v={version}" );
        module.SetupVoid( "initialize", _ => true ).SetVoidResult();
        module.SetupVoid( "updateOptions", _ => true ).SetVoidResult();

#pragma warning disable CS0618
        var component = Render<CropperComponent>( parameters => parameters
            .Add( x => x.Source, "image.png" )
            .Add( x => x.Alt, "Initial legacy image text" ) );

        component.Render( parameters => parameters
            .Add( x => x.Source, "image.png" )
            .Add( x => x.Alt, "Updated legacy image text" ) );
#pragma warning restore CS0618

        var updateInvocation = JSInterop.VerifyInvoke( "updateOptions" );
        var updateOptions = Assert.IsType<CropperUpdateJSOptions>( updateInvocation.Arguments[2] );
        Assert.True( updateOptions.Alt.Changed );
        Assert.Equal( "Updated legacy image text", updateOptions.Alt.Value );
    }
}