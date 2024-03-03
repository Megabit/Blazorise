#region Using directives
using Xunit;
#endregion

namespace Blazorise.Tests;

public class FluentFlexTest
{
    IClassProvider classProvider;

    public FluentFlexTest()
    {
        classProvider = new Bootstrap.BootstrapClassProvider();
    }

    [Fact]
    public void IsUndefined()
    {
        var flex = new FluentFlex();

        var classname = flex.Class( classProvider );

        Assert.Null( classname );
    }

    [Fact]
    public void IsOnly_Flex()
    {
        var flex = Flex._;

        var classname = flex.Class( classProvider );

        Assert.Equal( "d-flex", classname );
    }

    [Fact]
    public void IsOnly_InlineFlex()
    {
        var flex = Flex.InlineFlex;

        var classname = flex.Class( classProvider );

        Assert.Equal( "d-inline-flex", classname );
    }

    [Theory]
    [InlineData( "d-flex flex-row", FlexDirection.Row )]
    [InlineData( "d-flex flex-row-reverse", FlexDirection.ReverseRow )]
    [InlineData( "d-flex flex-column", FlexDirection.Column )]
    [InlineData( "d-flex flex-column-reverse", FlexDirection.ReverseColumn )]
    public void AreDirection( string expected, FlexDirection direction )
    {
        var flex = new FluentFlex();

        flex.WithFlexType( FlexType.Flex );
        flex.WithDirection( direction );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "d-flex flex-row", FlexDirection.Row, Breakpoint.Mobile )]
    [InlineData( "d-flex flex-row-reverse", FlexDirection.ReverseRow, Breakpoint.Mobile )]
    [InlineData( "d-flex flex-column", FlexDirection.Column, Breakpoint.Mobile )]
    [InlineData( "d-flex flex-column-reverse", FlexDirection.ReverseColumn, Breakpoint.Mobile )]
    [InlineData( "d-flex flex-sm-row", FlexDirection.Row, Breakpoint.Tablet )]
    [InlineData( "d-flex flex-sm-row-reverse", FlexDirection.ReverseRow, Breakpoint.Tablet )]
    [InlineData( "d-flex flex-sm-column", FlexDirection.Column, Breakpoint.Tablet )]
    [InlineData( "d-flex flex-sm-column-reverse", FlexDirection.ReverseColumn, Breakpoint.Tablet )]
    [InlineData( "d-flex flex-md-row", FlexDirection.Row, Breakpoint.Desktop )]
    [InlineData( "d-flex flex-md-row-reverse", FlexDirection.ReverseRow, Breakpoint.Desktop )]
    [InlineData( "d-flex flex-md-column", FlexDirection.Column, Breakpoint.Desktop )]
    [InlineData( "d-flex flex-md-column-reverse", FlexDirection.ReverseColumn, Breakpoint.Desktop )]
    [InlineData( "d-flex flex-lg-row", FlexDirection.Row, Breakpoint.Widescreen )]
    [InlineData( "d-flex flex-lg-row-reverse", FlexDirection.ReverseRow, Breakpoint.Widescreen )]
    [InlineData( "d-flex flex-lg-column", FlexDirection.Column, Breakpoint.Widescreen )]
    [InlineData( "d-flex flex-lg-column-reverse", FlexDirection.ReverseColumn, Breakpoint.Widescreen )]
    [InlineData( "d-flex flex-xl-row", FlexDirection.Row, Breakpoint.FullHD )]
    [InlineData( "d-flex flex-xl-row-reverse", FlexDirection.ReverseRow, Breakpoint.FullHD )]
    [InlineData( "d-flex flex-xl-column", FlexDirection.Column, Breakpoint.FullHD )]
    [InlineData( "d-flex flex-xl-column-reverse", FlexDirection.ReverseColumn, Breakpoint.FullHD )]
    [InlineData( "d-flex flex-xxl-row", FlexDirection.Row, Breakpoint.Full2K )]
    [InlineData( "d-flex flex-xxl-row-reverse", FlexDirection.ReverseRow, Breakpoint.Full2K )]
    [InlineData( "d-flex flex-xxl-column", FlexDirection.Column, Breakpoint.Full2K )]
    [InlineData( "d-flex flex-xxl-column-reverse", FlexDirection.ReverseColumn, Breakpoint.Full2K )]
    public void AreDirection_With_Breakpoints( string expected, FlexDirection direction, Breakpoint breakpoint )
    {
        var flex = new FluentFlex();

        flex.WithFlexType( FlexType.Flex );
        flex.WithDirection( direction );
        flex.WithBreakpoint( breakpoint );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }


    [Theory]
    [InlineData( "d-flex justify-content-start", FlexJustifyContent.Start )]
    [InlineData( "d-flex justify-content-end", FlexJustifyContent.End )]
    [InlineData( "d-flex justify-content-center", FlexJustifyContent.Center )]
    [InlineData( "d-flex justify-content-between", FlexJustifyContent.Between )]
    [InlineData( "d-flex justify-content-around", FlexJustifyContent.Around )]
    public void AreJustifyContent( string expected, FlexJustifyContent justifyContent )
    {
        var flex = new FluentFlex();

        flex.WithFlexType( FlexType.Flex );
        flex.WithJustifyContent( justifyContent );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "d-flex justify-content-start", FlexJustifyContent.Start, Breakpoint.Mobile )]
    [InlineData( "d-flex justify-content-end", FlexJustifyContent.End, Breakpoint.Mobile )]
    [InlineData( "d-flex justify-content-center", FlexJustifyContent.Center, Breakpoint.Mobile )]
    [InlineData( "d-flex justify-content-between", FlexJustifyContent.Between, Breakpoint.Mobile )]
    [InlineData( "d-flex justify-content-around", FlexJustifyContent.Around, Breakpoint.Mobile )]
    [InlineData( "d-flex justify-content-sm-start", FlexJustifyContent.Start, Breakpoint.Tablet )]
    [InlineData( "d-flex justify-content-sm-end", FlexJustifyContent.End, Breakpoint.Tablet )]
    [InlineData( "d-flex justify-content-sm-center", FlexJustifyContent.Center, Breakpoint.Tablet )]
    [InlineData( "d-flex justify-content-sm-between", FlexJustifyContent.Between, Breakpoint.Tablet )]
    [InlineData( "d-flex justify-content-sm-around", FlexJustifyContent.Around, Breakpoint.Tablet )]
    [InlineData( "d-flex justify-content-md-start", FlexJustifyContent.Start, Breakpoint.Desktop )]
    [InlineData( "d-flex justify-content-md-end", FlexJustifyContent.End, Breakpoint.Desktop )]
    [InlineData( "d-flex justify-content-md-center", FlexJustifyContent.Center, Breakpoint.Desktop )]
    [InlineData( "d-flex justify-content-md-between", FlexJustifyContent.Between, Breakpoint.Desktop )]
    [InlineData( "d-flex justify-content-md-around", FlexJustifyContent.Around, Breakpoint.Desktop )]
    [InlineData( "d-flex justify-content-lg-start", FlexJustifyContent.Start, Breakpoint.Widescreen )]
    [InlineData( "d-flex justify-content-lg-end", FlexJustifyContent.End, Breakpoint.Widescreen )]
    [InlineData( "d-flex justify-content-lg-center", FlexJustifyContent.Center, Breakpoint.Widescreen )]
    [InlineData( "d-flex justify-content-lg-between", FlexJustifyContent.Between, Breakpoint.Widescreen )]
    [InlineData( "d-flex justify-content-lg-around", FlexJustifyContent.Around, Breakpoint.Widescreen )]
    [InlineData( "d-flex justify-content-xl-start", FlexJustifyContent.Start, Breakpoint.FullHD )]
    [InlineData( "d-flex justify-content-xl-end", FlexJustifyContent.End, Breakpoint.FullHD )]
    [InlineData( "d-flex justify-content-xl-center", FlexJustifyContent.Center, Breakpoint.FullHD )]
    [InlineData( "d-flex justify-content-xl-between", FlexJustifyContent.Between, Breakpoint.FullHD )]
    [InlineData( "d-flex justify-content-xl-around", FlexJustifyContent.Around, Breakpoint.FullHD )]
    [InlineData( "d-flex justify-content-xxl-start", FlexJustifyContent.Start, Breakpoint.Full2K )]
    [InlineData( "d-flex justify-content-xxl-end", FlexJustifyContent.End, Breakpoint.Full2K )]
    [InlineData( "d-flex justify-content-xxl-center", FlexJustifyContent.Center, Breakpoint.Full2K )]
    [InlineData( "d-flex justify-content-xxl-between", FlexJustifyContent.Between, Breakpoint.Full2K )]
    [InlineData( "d-flex justify-content-xxl-around", FlexJustifyContent.Around, Breakpoint.Full2K )]
    public void AreJustifyContent_With_Breakpoints( string expected, FlexJustifyContent justifyContent, Breakpoint breakpoint )
    {
        var flex = new FluentFlex();

        flex.WithFlexType( FlexType.Flex );
        flex.WithJustifyContent( justifyContent );
        flex.WithBreakpoint( breakpoint );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "d-flex align-items-start", FlexAlignItems.Start )]
    [InlineData( "d-flex align-items-end", FlexAlignItems.End )]
    [InlineData( "d-flex align-items-center", FlexAlignItems.Center )]
    [InlineData( "d-flex align-items-baseline", FlexAlignItems.Baseline )]
    [InlineData( "d-flex align-items-stretch", FlexAlignItems.Stretch )]
    public void AreAlignItems( string expected, FlexAlignItems alignItems )
    {
        var flex = new FluentFlex();

        flex.WithFlexType( FlexType.Flex );
        flex.WithAlignItems( alignItems );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "d-flex align-items-start", FlexAlignItems.Start, Breakpoint.Mobile )]
    [InlineData( "d-flex align-items-end", FlexAlignItems.End, Breakpoint.Mobile )]
    [InlineData( "d-flex align-items-center", FlexAlignItems.Center, Breakpoint.Mobile )]
    [InlineData( "d-flex align-items-baseline", FlexAlignItems.Baseline, Breakpoint.Mobile )]
    [InlineData( "d-flex align-items-stretch", FlexAlignItems.Stretch, Breakpoint.Mobile )]
    [InlineData( "d-flex align-items-sm-start", FlexAlignItems.Start, Breakpoint.Tablet )]
    [InlineData( "d-flex align-items-sm-end", FlexAlignItems.End, Breakpoint.Tablet )]
    [InlineData( "d-flex align-items-sm-center", FlexAlignItems.Center, Breakpoint.Tablet )]
    [InlineData( "d-flex align-items-sm-baseline", FlexAlignItems.Baseline, Breakpoint.Tablet )]
    [InlineData( "d-flex align-items-sm-stretch", FlexAlignItems.Stretch, Breakpoint.Tablet )]
    [InlineData( "d-flex align-items-md-start", FlexAlignItems.Start, Breakpoint.Desktop )]
    [InlineData( "d-flex align-items-md-end", FlexAlignItems.End, Breakpoint.Desktop )]
    [InlineData( "d-flex align-items-md-center", FlexAlignItems.Center, Breakpoint.Desktop )]
    [InlineData( "d-flex align-items-md-baseline", FlexAlignItems.Baseline, Breakpoint.Desktop )]
    [InlineData( "d-flex align-items-md-stretch", FlexAlignItems.Stretch, Breakpoint.Desktop )]
    [InlineData( "d-flex align-items-lg-start", FlexAlignItems.Start, Breakpoint.Widescreen )]
    [InlineData( "d-flex align-items-lg-end", FlexAlignItems.End, Breakpoint.Widescreen )]
    [InlineData( "d-flex align-items-lg-center", FlexAlignItems.Center, Breakpoint.Widescreen )]
    [InlineData( "d-flex align-items-lg-baseline", FlexAlignItems.Baseline, Breakpoint.Widescreen )]
    [InlineData( "d-flex align-items-lg-stretch", FlexAlignItems.Stretch, Breakpoint.Widescreen )]
    [InlineData( "d-flex align-items-xl-start", FlexAlignItems.Start, Breakpoint.FullHD )]
    [InlineData( "d-flex align-items-xl-end", FlexAlignItems.End, Breakpoint.FullHD )]
    [InlineData( "d-flex align-items-xl-center", FlexAlignItems.Center, Breakpoint.FullHD )]
    [InlineData( "d-flex align-items-xl-baseline", FlexAlignItems.Baseline, Breakpoint.FullHD )]
    [InlineData( "d-flex align-items-xl-stretch", FlexAlignItems.Stretch, Breakpoint.FullHD )]
    [InlineData( "d-flex align-items-xxl-start", FlexAlignItems.Start, Breakpoint.Full2K )]
    [InlineData( "d-flex align-items-xxl-end", FlexAlignItems.End, Breakpoint.Full2K )]
    [InlineData( "d-flex align-items-xxl-center", FlexAlignItems.Center, Breakpoint.Full2K )]
    [InlineData( "d-flex align-items-xxl-baseline", FlexAlignItems.Baseline, Breakpoint.Full2K )]
    [InlineData( "d-flex align-items-xxl-stretch", FlexAlignItems.Stretch, Breakpoint.Full2K )]
    public void AreAlignItems_With_Breakpoints( string expected, FlexAlignItems alignItems, Breakpoint breakpoint )
    {
        var flex = new FluentFlex();

        flex.WithFlexType( FlexType.Flex );
        flex.WithAlignItems( alignItems );
        flex.WithBreakpoint( breakpoint );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "align-self-start", FlexAlignSelf.Start )]
    [InlineData( "align-self-end", FlexAlignSelf.End )]
    [InlineData( "align-self-center", FlexAlignSelf.Center )]
    [InlineData( "align-self-baseline", FlexAlignSelf.Baseline )]
    [InlineData( "align-self-stretch", FlexAlignSelf.Stretch )]
    public void AreAlignSelf( string expected, FlexAlignSelf alignSelf )
    {
        var flex = new FluentFlex();

        flex.WithAlignSelf( alignSelf );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "align-self-auto", FlexAlignSelf.Auto, Breakpoint.Mobile )]
    [InlineData( "align-self-start", FlexAlignSelf.Start, Breakpoint.Mobile )]
    [InlineData( "align-self-end", FlexAlignSelf.End, Breakpoint.Mobile )]
    [InlineData( "align-self-center", FlexAlignSelf.Center, Breakpoint.Mobile )]
    [InlineData( "align-self-baseline", FlexAlignSelf.Baseline, Breakpoint.Mobile )]
    [InlineData( "align-self-stretch", FlexAlignSelf.Stretch, Breakpoint.Mobile )]
    [InlineData( "align-self-sm-auto", FlexAlignSelf.Auto, Breakpoint.Tablet )]
    [InlineData( "align-self-sm-start", FlexAlignSelf.Start, Breakpoint.Tablet )]
    [InlineData( "align-self-sm-end", FlexAlignSelf.End, Breakpoint.Tablet )]
    [InlineData( "align-self-sm-center", FlexAlignSelf.Center, Breakpoint.Tablet )]
    [InlineData( "align-self-sm-baseline", FlexAlignSelf.Baseline, Breakpoint.Tablet )]
    [InlineData( "align-self-sm-stretch", FlexAlignSelf.Stretch, Breakpoint.Tablet )]
    [InlineData( "align-self-md-auto", FlexAlignSelf.Auto, Breakpoint.Desktop )]
    [InlineData( "align-self-md-start", FlexAlignSelf.Start, Breakpoint.Desktop )]
    [InlineData( "align-self-md-end", FlexAlignSelf.End, Breakpoint.Desktop )]
    [InlineData( "align-self-md-center", FlexAlignSelf.Center, Breakpoint.Desktop )]
    [InlineData( "align-self-md-baseline", FlexAlignSelf.Baseline, Breakpoint.Desktop )]
    [InlineData( "align-self-md-stretch", FlexAlignSelf.Stretch, Breakpoint.Desktop )]
    [InlineData( "align-self-lg-auto", FlexAlignSelf.Auto, Breakpoint.Widescreen )]
    [InlineData( "align-self-lg-start", FlexAlignSelf.Start, Breakpoint.Widescreen )]
    [InlineData( "align-self-lg-end", FlexAlignSelf.End, Breakpoint.Widescreen )]
    [InlineData( "align-self-lg-center", FlexAlignSelf.Center, Breakpoint.Widescreen )]
    [InlineData( "align-self-lg-baseline", FlexAlignSelf.Baseline, Breakpoint.Widescreen )]
    [InlineData( "align-self-lg-stretch", FlexAlignSelf.Stretch, Breakpoint.Widescreen )]
    [InlineData( "align-self-xl-auto", FlexAlignSelf.Auto, Breakpoint.FullHD )]
    [InlineData( "align-self-xl-start", FlexAlignSelf.Start, Breakpoint.FullHD )]
    [InlineData( "align-self-xl-end", FlexAlignSelf.End, Breakpoint.FullHD )]
    [InlineData( "align-self-xl-center", FlexAlignSelf.Center, Breakpoint.FullHD )]
    [InlineData( "align-self-xl-baseline", FlexAlignSelf.Baseline, Breakpoint.FullHD )]
    [InlineData( "align-self-xl-stretch", FlexAlignSelf.Stretch, Breakpoint.FullHD )]
    [InlineData( "align-self-xxl-auto", FlexAlignSelf.Auto, Breakpoint.Full2K )]
    [InlineData( "align-self-xxl-start", FlexAlignSelf.Start, Breakpoint.Full2K )]
    [InlineData( "align-self-xxl-end", FlexAlignSelf.End, Breakpoint.Full2K )]
    [InlineData( "align-self-xxl-center", FlexAlignSelf.Center, Breakpoint.Full2K )]
    [InlineData( "align-self-xxl-baseline", FlexAlignSelf.Baseline, Breakpoint.Full2K )]
    [InlineData( "align-self-xxl-stretch", FlexAlignSelf.Stretch, Breakpoint.Full2K )]
    public void AreAlignSelf_With_Breakpoints( string expected, FlexAlignSelf alignSelf, Breakpoint breakpoint )
    {
        var flex = new FluentFlex();

        flex.WithAlignSelf( alignSelf );
        flex.WithBreakpoint( breakpoint );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "d-flex align-content-start", FlexAlignContent.Start )]
    [InlineData( "d-flex align-content-end", FlexAlignContent.End )]
    [InlineData( "d-flex align-content-center", FlexAlignContent.Center )]
    [InlineData( "d-flex align-content-between", FlexAlignContent.Between )]
    [InlineData( "d-flex align-content-around", FlexAlignContent.Around )]
    [InlineData( "d-flex align-content-stretch", FlexAlignContent.Stretch )]
    public void AreAlignContent( string expected, FlexAlignContent alignContent )
    {
        var flex = new FluentFlex();

        flex.WithFlexType( FlexType.Flex );
        flex.WithAlignContent( alignContent );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "d-flex align-content-start", FlexAlignContent.Start, Breakpoint.Mobile )]
    [InlineData( "d-flex align-content-end", FlexAlignContent.End, Breakpoint.Mobile )]
    [InlineData( "d-flex align-content-center", FlexAlignContent.Center, Breakpoint.Mobile )]
    [InlineData( "d-flex align-content-between", FlexAlignContent.Between, Breakpoint.Mobile )]
    [InlineData( "d-flex align-content-around", FlexAlignContent.Around, Breakpoint.Mobile )]
    [InlineData( "d-flex align-content-stretch", FlexAlignContent.Stretch, Breakpoint.Mobile )]
    [InlineData( "d-flex align-content-sm-start", FlexAlignContent.Start, Breakpoint.Tablet )]
    [InlineData( "d-flex align-content-sm-end", FlexAlignContent.End, Breakpoint.Tablet )]
    [InlineData( "d-flex align-content-sm-center", FlexAlignContent.Center, Breakpoint.Tablet )]
    [InlineData( "d-flex align-content-sm-between", FlexAlignContent.Between, Breakpoint.Tablet )]
    [InlineData( "d-flex align-content-sm-around", FlexAlignContent.Around, Breakpoint.Tablet )]
    [InlineData( "d-flex align-content-sm-stretch", FlexAlignContent.Stretch, Breakpoint.Tablet )]
    [InlineData( "d-flex align-content-md-start", FlexAlignContent.Start, Breakpoint.Desktop )]
    [InlineData( "d-flex align-content-md-end", FlexAlignContent.End, Breakpoint.Desktop )]
    [InlineData( "d-flex align-content-md-center", FlexAlignContent.Center, Breakpoint.Desktop )]
    [InlineData( "d-flex align-content-md-between", FlexAlignContent.Between, Breakpoint.Desktop )]
    [InlineData( "d-flex align-content-md-around", FlexAlignContent.Around, Breakpoint.Desktop )]
    [InlineData( "d-flex align-content-md-stretch", FlexAlignContent.Stretch, Breakpoint.Desktop )]
    [InlineData( "d-flex align-content-lg-start", FlexAlignContent.Start, Breakpoint.Widescreen )]
    [InlineData( "d-flex align-content-lg-end", FlexAlignContent.End, Breakpoint.Widescreen )]
    [InlineData( "d-flex align-content-lg-center", FlexAlignContent.Center, Breakpoint.Widescreen )]
    [InlineData( "d-flex align-content-lg-between", FlexAlignContent.Between, Breakpoint.Widescreen )]
    [InlineData( "d-flex align-content-lg-around", FlexAlignContent.Around, Breakpoint.Widescreen )]
    [InlineData( "d-flex align-content-lg-stretch", FlexAlignContent.Stretch, Breakpoint.Widescreen )]
    [InlineData( "d-flex align-content-xl-start", FlexAlignContent.Start, Breakpoint.FullHD )]
    [InlineData( "d-flex align-content-xl-end", FlexAlignContent.End, Breakpoint.FullHD )]
    [InlineData( "d-flex align-content-xl-center", FlexAlignContent.Center, Breakpoint.FullHD )]
    [InlineData( "d-flex align-content-xl-between", FlexAlignContent.Between, Breakpoint.FullHD )]
    [InlineData( "d-flex align-content-xl-around", FlexAlignContent.Around, Breakpoint.FullHD )]
    [InlineData( "d-flex align-content-xl-stretch", FlexAlignContent.Stretch, Breakpoint.FullHD )]
    [InlineData( "d-flex align-content-xxl-start", FlexAlignContent.Start, Breakpoint.Full2K )]
    [InlineData( "d-flex align-content-xxl-end", FlexAlignContent.End, Breakpoint.Full2K )]
    [InlineData( "d-flex align-content-xxl-center", FlexAlignContent.Center, Breakpoint.Full2K )]
    [InlineData( "d-flex align-content-xxl-between", FlexAlignContent.Between, Breakpoint.Full2K )]
    [InlineData( "d-flex align-content-xxl-around", FlexAlignContent.Around, Breakpoint.Full2K )]
    [InlineData( "d-flex align-content-xxl-stretch", FlexAlignContent.Stretch, Breakpoint.Full2K )]
    public void AreAlignContent_With_Breakpoints( string expected, FlexAlignContent alignContent, Breakpoint breakpoint )
    {
        var flex = new FluentFlex();

        flex.WithFlexType( FlexType.Flex );
        flex.WithAlignContent( alignContent );
        flex.WithBreakpoint( breakpoint );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Fact]
    public void IsFill()
    {
        var flex = new FluentFlex();

        flex.WithFill();

        var classname = flex.Class( classProvider );

        Assert.Equal( "flex-fill", classname );
    }

    [Theory]
    [InlineData( "flex-fill", Breakpoint.Mobile )]
    [InlineData( "flex-sm-fill", Breakpoint.Tablet )]
    [InlineData( "flex-md-fill", Breakpoint.Desktop )]
    [InlineData( "flex-lg-fill", Breakpoint.Widescreen )]
    [InlineData( "flex-xl-fill", Breakpoint.FullHD )]
    [InlineData( "flex-xxl-fill", Breakpoint.Full2K )]
    public void AreFill_With_Breakpoint( string expected, Breakpoint breakpoint )
    {
        var flex = new FluentFlex();

        flex.WithFill();
        flex.WithBreakpoint( breakpoint );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "d-flex flex-wrap", FlexWrap.Wrap )]
    [InlineData( "d-flex flex-wrap-reverse", FlexWrap.ReverseWrap )]
    [InlineData( "d-flex flex-nowrap", FlexWrap.NoWrap )]
    public void AreWrap( string expected, FlexWrap wrap )
    {
        var flex = new FluentFlex();

        flex.WithFlexType( FlexType.Flex );
        flex.WithWrap( wrap );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "d-flex flex-wrap", FlexWrap.Wrap, Breakpoint.Mobile )]
    [InlineData( "d-flex flex-wrap-reverse", FlexWrap.ReverseWrap, Breakpoint.Mobile )]
    [InlineData( "d-flex flex-nowrap", FlexWrap.NoWrap, Breakpoint.Mobile )]
    [InlineData( "d-flex flex-sm-wrap", FlexWrap.Wrap, Breakpoint.Tablet )]
    [InlineData( "d-flex flex-sm-wrap-reverse", FlexWrap.ReverseWrap, Breakpoint.Tablet )]
    [InlineData( "d-flex flex-sm-nowrap", FlexWrap.NoWrap, Breakpoint.Tablet )]
    [InlineData( "d-flex flex-md-wrap", FlexWrap.Wrap, Breakpoint.Desktop )]
    [InlineData( "d-flex flex-md-wrap-reverse", FlexWrap.ReverseWrap, Breakpoint.Desktop )]
    [InlineData( "d-flex flex-md-nowrap", FlexWrap.NoWrap, Breakpoint.Desktop )]
    [InlineData( "d-flex flex-lg-wrap", FlexWrap.Wrap, Breakpoint.Widescreen )]
    [InlineData( "d-flex flex-lg-wrap-reverse", FlexWrap.ReverseWrap, Breakpoint.Widescreen )]
    [InlineData( "d-flex flex-lg-nowrap", FlexWrap.NoWrap, Breakpoint.Widescreen )]
    [InlineData( "d-flex flex-xl-wrap", FlexWrap.Wrap, Breakpoint.FullHD )]
    [InlineData( "d-flex flex-xl-wrap-reverse", FlexWrap.ReverseWrap, Breakpoint.FullHD )]
    [InlineData( "d-flex flex-xl-nowrap", FlexWrap.NoWrap, Breakpoint.FullHD )]
    [InlineData( "d-flex flex-xxl-wrap", FlexWrap.Wrap, Breakpoint.Full2K )]
    [InlineData( "d-flex flex-xxl-wrap-reverse", FlexWrap.ReverseWrap, Breakpoint.Full2K )]
    [InlineData( "d-flex flex-xxl-nowrap", FlexWrap.NoWrap, Breakpoint.Full2K )]
    public void AreWrap_With_Breakpoint( string expected, FlexWrap wrap, Breakpoint breakpoint )
    {
        var flex = new FluentFlex();

        flex.WithFlexType( FlexType.Flex );
        flex.WithWrap( wrap );
        flex.WithBreakpoint( breakpoint );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "order-0", FlexOrder.Is0 )]
    [InlineData( "order-1", FlexOrder.Is1 )]
    [InlineData( "order-2", FlexOrder.Is2 )]
    [InlineData( "order-3", FlexOrder.Is3 )]
    [InlineData( "order-4", FlexOrder.Is4 )]
    [InlineData( "order-5", FlexOrder.Is5 )]
    [InlineData( "order-6", FlexOrder.Is6 )]
    [InlineData( "order-7", FlexOrder.Is7 )]
    [InlineData( "order-8", FlexOrder.Is8 )]
    [InlineData( "order-9", FlexOrder.Is9 )]
    [InlineData( "order-10", FlexOrder.Is10 )]
    [InlineData( "order-11", FlexOrder.Is11 )]
    [InlineData( "order-12", FlexOrder.Is12 )]
    public void AreOrder( string expected, FlexOrder order )
    {
        var flex = new FluentFlex();

        flex.WithOrder( order );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "flex-row", FlexDirection.Row, true )]
    [InlineData( null, FlexDirection.Row, false )]
    public void AreConditions( string expected, FlexDirection flexDirection, bool condition )
    {
        var flex = new FluentFlex();

        flex.WithDirection( flexDirection );
        flex.If( condition );

        var classname = flex.Class( classProvider );

        Assert.Equal( expected, classname );
    }
}