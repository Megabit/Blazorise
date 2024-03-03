#region Using directives
using Xunit;
#endregion

namespace Blazorise.Tests;

public class FluentMarginTest
{
    IClassProvider classProvider;

    public FluentMarginTest()
    {
        classProvider = new Bootstrap.BootstrapClassProvider();
    }

    [Theory]
    [InlineData( "m-0", SpacingSize.Is0 )]
    [InlineData( "m-1", SpacingSize.Is1 )]
    [InlineData( "m-2", SpacingSize.Is2 )]
    [InlineData( "m-3", SpacingSize.Is3 )]
    [InlineData( "m-4", SpacingSize.Is4 )]
    [InlineData( "m-5", SpacingSize.Is5 )]
    [InlineData( "m-auto", SpacingSize.IsAuto )]
    public void AreSizes( string expected, SpacingSize spacingSize )
    {
        var margin = new FluentMargin();

        margin.WithSize( spacingSize );

        var classname = margin.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "mt-1", SpacingSize.Is1, Side.Top )]
    [InlineData( "mb-1", SpacingSize.Is1, Side.Bottom )]
    [InlineData( "ml-1", SpacingSize.Is1, Side.Start )]
    [InlineData( "mr-1", SpacingSize.Is1, Side.End )]
    [InlineData( "mx-1", SpacingSize.Is1, Side.X )]
    [InlineData( "my-1", SpacingSize.Is1, Side.Y )]
    public void AreSides( string expected, SpacingSize spacingSize, Side side )
    {
        var margin = new FluentMargin();

        margin.WithSize( spacingSize );

        if ( side != Side.None )
            margin.WithSide( side );

        var classname = margin.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "m-1", SpacingSize.Is1, Side.All, Breakpoint.Mobile )]
    [InlineData( "m-sm-1", SpacingSize.Is1, Side.All, Breakpoint.Tablet )]
    [InlineData( "m-md-1", SpacingSize.Is1, Side.All, Breakpoint.Desktop )]
    [InlineData( "m-lg-1", SpacingSize.Is1, Side.All, Breakpoint.Widescreen )]
    [InlineData( "m-xl-1", SpacingSize.Is1, Side.All, Breakpoint.FullHD )]
    [InlineData( "m-xxl-1", SpacingSize.Is1, Side.All, Breakpoint.Full2K )]
    [InlineData( "mt-1", SpacingSize.Is1, Side.Top, Breakpoint.Mobile )]
    [InlineData( "mt-sm-1", SpacingSize.Is1, Side.Top, Breakpoint.Tablet )]
    [InlineData( "mt-md-1", SpacingSize.Is1, Side.Top, Breakpoint.Desktop )]
    [InlineData( "mt-lg-1", SpacingSize.Is1, Side.Top, Breakpoint.Widescreen )]
    [InlineData( "mt-xl-1", SpacingSize.Is1, Side.Top, Breakpoint.FullHD )]
    [InlineData( "mt-xxl-1", SpacingSize.Is1, Side.Top, Breakpoint.Full2K )]
    [InlineData( "mb-1", SpacingSize.Is1, Side.Bottom, Breakpoint.Mobile )]
    [InlineData( "mb-sm-1", SpacingSize.Is1, Side.Bottom, Breakpoint.Tablet )]
    [InlineData( "mb-md-1", SpacingSize.Is1, Side.Bottom, Breakpoint.Desktop )]
    [InlineData( "mb-lg-1", SpacingSize.Is1, Side.Bottom, Breakpoint.Widescreen )]
    [InlineData( "mb-xl-1", SpacingSize.Is1, Side.Bottom, Breakpoint.FullHD )]
    [InlineData( "mb-xxl-1", SpacingSize.Is1, Side.Bottom, Breakpoint.Full2K )]
    [InlineData( "ml-1", SpacingSize.Is1, Side.Start, Breakpoint.Mobile )]
    [InlineData( "ml-sm-1", SpacingSize.Is1, Side.Start, Breakpoint.Tablet )]
    [InlineData( "ml-md-1", SpacingSize.Is1, Side.Start, Breakpoint.Desktop )]
    [InlineData( "ml-lg-1", SpacingSize.Is1, Side.Start, Breakpoint.Widescreen )]
    [InlineData( "ml-xl-1", SpacingSize.Is1, Side.Start, Breakpoint.FullHD )]
    [InlineData( "ml-xxl-1", SpacingSize.Is1, Side.Start, Breakpoint.Full2K )]
    [InlineData( "mr-1", SpacingSize.Is1, Side.End, Breakpoint.Mobile )]
    [InlineData( "mr-sm-1", SpacingSize.Is1, Side.End, Breakpoint.Tablet )]
    [InlineData( "mr-md-1", SpacingSize.Is1, Side.End, Breakpoint.Desktop )]
    [InlineData( "mr-lg-1", SpacingSize.Is1, Side.End, Breakpoint.Widescreen )]
    [InlineData( "mr-xl-1", SpacingSize.Is1, Side.End, Breakpoint.FullHD )]
    [InlineData( "mr-xxl-1", SpacingSize.Is1, Side.End, Breakpoint.Full2K )]
    [InlineData( "mx-1", SpacingSize.Is1, Side.X, Breakpoint.Mobile )]
    [InlineData( "mx-sm-1", SpacingSize.Is1, Side.X, Breakpoint.Tablet )]
    [InlineData( "mx-md-1", SpacingSize.Is1, Side.X, Breakpoint.Desktop )]
    [InlineData( "mx-lg-1", SpacingSize.Is1, Side.X, Breakpoint.Widescreen )]
    [InlineData( "mx-xl-1", SpacingSize.Is1, Side.X, Breakpoint.FullHD )]
    [InlineData( "mx-xxl-1", SpacingSize.Is1, Side.X, Breakpoint.Full2K )]
    [InlineData( "my-1", SpacingSize.Is1, Side.Y, Breakpoint.Mobile )]
    [InlineData( "my-sm-1", SpacingSize.Is1, Side.Y, Breakpoint.Tablet )]
    [InlineData( "my-md-1", SpacingSize.Is1, Side.Y, Breakpoint.Desktop )]
    [InlineData( "my-lg-1", SpacingSize.Is1, Side.Y, Breakpoint.Widescreen )]
    [InlineData( "my-xl-1", SpacingSize.Is1, Side.Y, Breakpoint.FullHD )]
    [InlineData( "my-xxl-1", SpacingSize.Is1, Side.Y, Breakpoint.Full2K )]
    public void AreBreakpoints_OnAll( string expected, SpacingSize spacingSize, Side side, Breakpoint breakpoint )
    {
        var margin = new FluentMargin();

        margin.WithSize( spacingSize );

        if ( side != Side.None )
            margin.WithSide( side );

        if ( breakpoint != Breakpoint.None )
            margin.WithBreakpoint( breakpoint );

        var classname = margin.Class( classProvider );

        Assert.Equal( expected, classname );
    }
}