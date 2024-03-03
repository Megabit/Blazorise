#region Using directives
using Xunit;
#endregion

namespace Blazorise.Tests;

public class FluentColumnTest
{
    IClassProvider classProvider;

    public FluentColumnTest()
    {
        classProvider = new Bootstrap.BootstrapClassProvider();
    }

    [Theory]
    [InlineData( null, ColumnWidth.Default )]
    [InlineData( "col-1", ColumnWidth.Is1 )]
    [InlineData( "col-2", ColumnWidth.Is2 )]
    [InlineData( "col-3", ColumnWidth.Is3 )]
    [InlineData( "col-4", ColumnWidth.Is4 )]
    [InlineData( "col-5", ColumnWidth.Is5 )]
    [InlineData( "col-6", ColumnWidth.Is6 )]
    [InlineData( "col-7", ColumnWidth.Is7 )]
    [InlineData( "col-8", ColumnWidth.Is8 )]
    [InlineData( "col-9", ColumnWidth.Is9 )]
    [InlineData( "col-10", ColumnWidth.Is10 )]
    [InlineData( "col-11", ColumnWidth.Is11 )]
    [InlineData( "col-12", ColumnWidth.Is12 )]
    public void AreColumn( string expected, ColumnWidth columnWidth )
    {
        var columnsSize = new FluentColumn();

        columnsSize.WithColumnSize( columnWidth );

        var classname = columnsSize.Class( false, classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "col-1", ColumnWidth.Is1, Breakpoint.Mobile )]
    [InlineData( "col-2", ColumnWidth.Is2, Breakpoint.Mobile )]
    [InlineData( "col-3", ColumnWidth.Is3, Breakpoint.Mobile )]
    [InlineData( "col-4", ColumnWidth.Is4, Breakpoint.Mobile )]
    [InlineData( "col-5", ColumnWidth.Is5, Breakpoint.Mobile )]
    [InlineData( "col-6", ColumnWidth.Is6, Breakpoint.Mobile )]
    [InlineData( "col-7", ColumnWidth.Is7, Breakpoint.Mobile )]
    [InlineData( "col-8", ColumnWidth.Is8, Breakpoint.Mobile )]
    [InlineData( "col-9", ColumnWidth.Is9, Breakpoint.Mobile )]
    [InlineData( "col-10", ColumnWidth.Is10, Breakpoint.Mobile )]
    [InlineData( "col-11", ColumnWidth.Is11, Breakpoint.Mobile )]
    [InlineData( "col-12", ColumnWidth.Is12, Breakpoint.Mobile )]
    [InlineData( "col-sm-1", ColumnWidth.Is1, Breakpoint.Tablet )]
    [InlineData( "col-sm-2", ColumnWidth.Is2, Breakpoint.Tablet )]
    [InlineData( "col-sm-3", ColumnWidth.Is3, Breakpoint.Tablet )]
    [InlineData( "col-sm-4", ColumnWidth.Is4, Breakpoint.Tablet )]
    [InlineData( "col-sm-5", ColumnWidth.Is5, Breakpoint.Tablet )]
    [InlineData( "col-sm-6", ColumnWidth.Is6, Breakpoint.Tablet )]
    [InlineData( "col-sm-7", ColumnWidth.Is7, Breakpoint.Tablet )]
    [InlineData( "col-sm-8", ColumnWidth.Is8, Breakpoint.Tablet )]
    [InlineData( "col-sm-9", ColumnWidth.Is9, Breakpoint.Tablet )]
    [InlineData( "col-sm-10", ColumnWidth.Is10, Breakpoint.Tablet )]
    [InlineData( "col-sm-11", ColumnWidth.Is11, Breakpoint.Tablet )]
    [InlineData( "col-sm-12", ColumnWidth.Is12, Breakpoint.Tablet )]
    [InlineData( "col-md-1", ColumnWidth.Is1, Breakpoint.Desktop )]
    [InlineData( "col-md-2", ColumnWidth.Is2, Breakpoint.Desktop )]
    [InlineData( "col-md-3", ColumnWidth.Is3, Breakpoint.Desktop )]
    [InlineData( "col-md-4", ColumnWidth.Is4, Breakpoint.Desktop )]
    [InlineData( "col-md-5", ColumnWidth.Is5, Breakpoint.Desktop )]
    [InlineData( "col-md-6", ColumnWidth.Is6, Breakpoint.Desktop )]
    [InlineData( "col-md-7", ColumnWidth.Is7, Breakpoint.Desktop )]
    [InlineData( "col-md-8", ColumnWidth.Is8, Breakpoint.Desktop )]
    [InlineData( "col-md-9", ColumnWidth.Is9, Breakpoint.Desktop )]
    [InlineData( "col-md-10", ColumnWidth.Is10, Breakpoint.Desktop )]
    [InlineData( "col-md-11", ColumnWidth.Is11, Breakpoint.Desktop )]
    [InlineData( "col-md-12", ColumnWidth.Is12, Breakpoint.Desktop )]
    [InlineData( "col-lg-1", ColumnWidth.Is1, Breakpoint.Widescreen )]
    [InlineData( "col-lg-2", ColumnWidth.Is2, Breakpoint.Widescreen )]
    [InlineData( "col-lg-3", ColumnWidth.Is3, Breakpoint.Widescreen )]
    [InlineData( "col-lg-4", ColumnWidth.Is4, Breakpoint.Widescreen )]
    [InlineData( "col-lg-5", ColumnWidth.Is5, Breakpoint.Widescreen )]
    [InlineData( "col-lg-6", ColumnWidth.Is6, Breakpoint.Widescreen )]
    [InlineData( "col-lg-7", ColumnWidth.Is7, Breakpoint.Widescreen )]
    [InlineData( "col-lg-8", ColumnWidth.Is8, Breakpoint.Widescreen )]
    [InlineData( "col-lg-9", ColumnWidth.Is9, Breakpoint.Widescreen )]
    [InlineData( "col-lg-10", ColumnWidth.Is10, Breakpoint.Widescreen )]
    [InlineData( "col-lg-11", ColumnWidth.Is11, Breakpoint.Widescreen )]
    [InlineData( "col-lg-12", ColumnWidth.Is12, Breakpoint.Widescreen )]
    [InlineData( "col-xl-1", ColumnWidth.Is1, Breakpoint.FullHD )]
    [InlineData( "col-xl-2", ColumnWidth.Is2, Breakpoint.FullHD )]
    [InlineData( "col-xl-3", ColumnWidth.Is3, Breakpoint.FullHD )]
    [InlineData( "col-xl-4", ColumnWidth.Is4, Breakpoint.FullHD )]
    [InlineData( "col-xl-5", ColumnWidth.Is5, Breakpoint.FullHD )]
    [InlineData( "col-xl-6", ColumnWidth.Is6, Breakpoint.FullHD )]
    [InlineData( "col-xl-7", ColumnWidth.Is7, Breakpoint.FullHD )]
    [InlineData( "col-xl-8", ColumnWidth.Is8, Breakpoint.FullHD )]
    [InlineData( "col-xl-9", ColumnWidth.Is9, Breakpoint.FullHD )]
    [InlineData( "col-xl-10", ColumnWidth.Is10, Breakpoint.FullHD )]
    [InlineData( "col-xl-11", ColumnWidth.Is11, Breakpoint.FullHD )]
    [InlineData( "col-xl-12", ColumnWidth.Is12, Breakpoint.FullHD )]
    [InlineData( "col-xxl-1", ColumnWidth.Is1, Breakpoint.Full2K )]
    [InlineData( "col-xxl-2", ColumnWidth.Is2, Breakpoint.Full2K )]
    [InlineData( "col-xxl-3", ColumnWidth.Is3, Breakpoint.Full2K )]
    [InlineData( "col-xxl-4", ColumnWidth.Is4, Breakpoint.Full2K )]
    [InlineData( "col-xxl-5", ColumnWidth.Is5, Breakpoint.Full2K )]
    [InlineData( "col-xxl-6", ColumnWidth.Is6, Breakpoint.Full2K )]
    [InlineData( "col-xxl-7", ColumnWidth.Is7, Breakpoint.Full2K )]
    [InlineData( "col-xxl-8", ColumnWidth.Is8, Breakpoint.Full2K )]
    [InlineData( "col-xxl-9", ColumnWidth.Is9, Breakpoint.Full2K )]
    [InlineData( "col-xxl-10", ColumnWidth.Is10, Breakpoint.Full2K )]
    [InlineData( "col-xxl-11", ColumnWidth.Is11, Breakpoint.Full2K )]
    [InlineData( "col-xxl-12", ColumnWidth.Is12, Breakpoint.Full2K )]
    public void AreBreakpoints_OnAll( string expected, ColumnWidth columnWidth, Breakpoint breakpoint )
    {
        var columnsSize = new FluentColumn();

        columnsSize.WithColumnSize( columnWidth );

        if ( breakpoint != Breakpoint.None )
            columnsSize.WithBreakpoint( breakpoint );

        var classname = columnsSize.Class( false, classProvider );

        Assert.Equal( expected, classname );
    }
}