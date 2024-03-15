#region Using directives
using Blazorise.Bootstrap.Providers;
using Xunit;
#endregion

namespace Blazorise.Tests;

public class FluentGapTest
{
    IClassProvider classProvider;

    public FluentGapTest()
    {
        classProvider = new BootstrapClassProvider();
    }

    [Theory]
    [InlineData( "gap-0", GapSize.Is0 )]
    [InlineData( "gap-1", GapSize.Is1 )]
    [InlineData( "gap-2", GapSize.Is2 )]
    [InlineData( "gap-3", GapSize.Is3 )]
    [InlineData( "gap-4", GapSize.Is4 )]
    [InlineData( "gap-5", GapSize.Is5 )]
    public void AreSizes( string expected, GapSize gapSize )
    {
        var padding = new FluentGap();

        padding.WithSize( gapSize );

        var classname = padding.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "gap-x-1", GapSize.Is1, GapSide.X )]
    [InlineData( "gap-y-1", GapSize.Is1, GapSide.Y )]
    [InlineData( "gap-1", GapSize.Is1, GapSide.All )]
    public void AreGapSides( string expected, GapSize gapSize, GapSide gapSide )
    {
        var padding = new FluentGap();

        padding.WithSize( gapSize );

        if ( gapSide != GapSide.None )
            padding.WithSide( gapSide );

        var classname = padding.Class( classProvider );

        Assert.Equal( expected, classname );
    }
}