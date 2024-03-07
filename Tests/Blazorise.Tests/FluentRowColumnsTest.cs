#region Using directives
using Xunit;
#endregion

namespace Blazorise.Tests;

public class FluentRowColumnsTest
{
    IClassProvider classProvider;

    public FluentRowColumnsTest()
    {
        classProvider = new Bootstrap.BootstrapClassProvider();
    }

    [Theory]
    [InlineData( null, RowColumnsSize.Default )]
    [InlineData( "row-cols-1", RowColumnsSize.Are1 )]
    [InlineData( "row-cols-2", RowColumnsSize.Are2 )]
    [InlineData( "row-cols-3", RowColumnsSize.Are3 )]
    [InlineData( "row-cols-4", RowColumnsSize.Are4 )]
    [InlineData( "row-cols-5", RowColumnsSize.Are5 )]
    [InlineData( "row-cols-6", RowColumnsSize.Are6 )]
    public void AreRowColumns( string expected, RowColumnsSize rowColumnsSize )
    {
        var fluentRowColumns = new FluentRowColumns();

        fluentRowColumns.WithRowColumnsSize( rowColumnsSize );

        var classname = fluentRowColumns.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "row-cols-1", RowColumnsSize.Are1, Breakpoint.Mobile )]
    [InlineData( "row-cols-2", RowColumnsSize.Are2, Breakpoint.Mobile )]
    [InlineData( "row-cols-3", RowColumnsSize.Are3, Breakpoint.Mobile )]
    [InlineData( "row-cols-4", RowColumnsSize.Are4, Breakpoint.Mobile )]
    [InlineData( "row-cols-5", RowColumnsSize.Are5, Breakpoint.Mobile )]
    [InlineData( "row-cols-6", RowColumnsSize.Are6, Breakpoint.Mobile )]
    [InlineData( "row-cols-sm-1", RowColumnsSize.Are1, Breakpoint.Tablet )]
    [InlineData( "row-cols-sm-2", RowColumnsSize.Are2, Breakpoint.Tablet )]
    [InlineData( "row-cols-sm-3", RowColumnsSize.Are3, Breakpoint.Tablet )]
    [InlineData( "row-cols-sm-4", RowColumnsSize.Are4, Breakpoint.Tablet )]
    [InlineData( "row-cols-sm-5", RowColumnsSize.Are5, Breakpoint.Tablet )]
    [InlineData( "row-cols-sm-6", RowColumnsSize.Are6, Breakpoint.Tablet )]
    [InlineData( "row-cols-md-1", RowColumnsSize.Are1, Breakpoint.Desktop )]
    [InlineData( "row-cols-md-2", RowColumnsSize.Are2, Breakpoint.Desktop )]
    [InlineData( "row-cols-md-3", RowColumnsSize.Are3, Breakpoint.Desktop )]
    [InlineData( "row-cols-md-4", RowColumnsSize.Are4, Breakpoint.Desktop )]
    [InlineData( "row-cols-md-5", RowColumnsSize.Are5, Breakpoint.Desktop )]
    [InlineData( "row-cols-md-6", RowColumnsSize.Are6, Breakpoint.Desktop )]
    [InlineData( "row-cols-lg-1", RowColumnsSize.Are1, Breakpoint.Widescreen )]
    [InlineData( "row-cols-lg-2", RowColumnsSize.Are2, Breakpoint.Widescreen )]
    [InlineData( "row-cols-lg-3", RowColumnsSize.Are3, Breakpoint.Widescreen )]
    [InlineData( "row-cols-lg-4", RowColumnsSize.Are4, Breakpoint.Widescreen )]
    [InlineData( "row-cols-lg-5", RowColumnsSize.Are5, Breakpoint.Widescreen )]
    [InlineData( "row-cols-lg-6", RowColumnsSize.Are6, Breakpoint.Widescreen )]
    [InlineData( "row-cols-xl-1", RowColumnsSize.Are1, Breakpoint.FullHD )]
    [InlineData( "row-cols-xl-2", RowColumnsSize.Are2, Breakpoint.FullHD )]
    [InlineData( "row-cols-xl-3", RowColumnsSize.Are3, Breakpoint.FullHD )]
    [InlineData( "row-cols-xl-4", RowColumnsSize.Are4, Breakpoint.FullHD )]
    [InlineData( "row-cols-xl-5", RowColumnsSize.Are5, Breakpoint.FullHD )]
    [InlineData( "row-cols-xl-6", RowColumnsSize.Are6, Breakpoint.FullHD )]
    [InlineData( "row-cols-xxl-1", RowColumnsSize.Are1, Breakpoint.QuadHD )]
    [InlineData( "row-cols-xxl-2", RowColumnsSize.Are2, Breakpoint.QuadHD )]
    [InlineData( "row-cols-xxl-3", RowColumnsSize.Are3, Breakpoint.QuadHD )]
    [InlineData( "row-cols-xxl-4", RowColumnsSize.Are4, Breakpoint.QuadHD )]
    [InlineData( "row-cols-xxl-5", RowColumnsSize.Are5, Breakpoint.QuadHD )]
    [InlineData( "row-cols-xxl-6", RowColumnsSize.Are6, Breakpoint.QuadHD )]
    public void AreBreakpoints_OnAll( string expected, RowColumnsSize rowColumnsSize, Breakpoint breakpoint )
    {
        var fluentRowColumns = new FluentRowColumns();

        fluentRowColumns.WithRowColumnsSize( rowColumnsSize );

        if ( breakpoint != Breakpoint.None )
            fluentRowColumns.WithBreakpoint( breakpoint );

        var classname = fluentRowColumns.Class( classProvider );

        Assert.Equal( expected, classname );
    }
}