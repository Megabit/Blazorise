#region Using directives
using Xunit;
#endregion

namespace Blazorise.Tests;

public class SizingTest
{
    IClassProvider classProvider;

    public SizingTest()
    {
        classProvider = new Bootstrap.BootstrapClassProvider();
    }

    [Theory]
    [InlineData( "w-25", SizingSize.Is25 )]
    [InlineData( "w-33", SizingSize.Is33 )]
    [InlineData( "w-50", SizingSize.Is50 )]
    [InlineData( "w-66", SizingSize.Is66 )]
    [InlineData( "w-75", SizingSize.Is75 )]
    [InlineData( "w-100", SizingSize.Is100 )]
    [InlineData( "w-auto", SizingSize.Auto )]
    public void AreWidth( string expected, SizingSize sizingSize )
    {
        var sizing = new FluentSizing( SizingType.Width );

        sizing.WithSize( sizingSize );

        var classname = sizing.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Theory]
    [InlineData( "h-25", SizingSize.Is25 )]
    [InlineData( "h-33", SizingSize.Is33 )]
    [InlineData( "h-50", SizingSize.Is50 )]
    [InlineData( "h-66", SizingSize.Is66 )]
    [InlineData( "h-75", SizingSize.Is75 )]
    [InlineData( "h-100", SizingSize.Is100 )]
    [InlineData( "h-auto", SizingSize.Auto )]
    public void AreHeight( string expected, SizingSize sizingSize )
    {
        var sizing = new FluentSizing( SizingType.Height );

        sizing.WithSize( sizingSize );

        var classname = sizing.Class( classProvider );

        Assert.Equal( expected, classname );
    }

    [Fact]
    public void IsWidth_Max100()
    {
        var sizing = new FluentSizing( SizingType.Width );

        sizing.WithSize( SizingSize.Is100 );
        sizing.WithMax();

        var classname = sizing.Class( classProvider );

        Assert.Equal( "mw-100", classname );
    }

    [Fact]
    public void IsWidth_MinViewport()
    {
        var sizing = new FluentSizing( SizingType.Width );

        sizing.WithSize( SizingSize.Is100 );
        sizing.WithMin();
        sizing.WithViewport();

        var classname = sizing.Class( classProvider );

        Assert.Equal( "min-vw-100", classname );
    }
}