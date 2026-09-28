#region Using directives
using Blazorise.Bootstrap.Providers;
using Xunit;
#endregion

namespace Blazorise.Tests;

public class ZIndexTest
{
    private readonly IClassProvider classProvider;

    private readonly IStyleProvider styleProvider;

    public ZIndexTest()
    {
        classProvider = new BootstrapClassProvider();
        styleProvider = new BootstrapStyleProvider();
    }

    [Theory]
    [InlineData( "z-index:-1", -1 )]
    [InlineData( "z-index:0", 0 )]
    [InlineData( "z-index:2000", 2000 )]
    public void AreNumericValues( string expected, int number )
    {
        var value = (ZIndex)number;
        var nullableValue = (ZIndex)(int?)number;

        Assert.Equal( number, value.Value );
        Assert.Null( value.Name );
        Assert.False( value.IsDefault );
        Assert.Equal( value, nullableValue );
        Assert.NotEqual( ZIndex.Default, value );
        Assert.Null( classProvider.ZIndex( value ) );
        Assert.Equal( expected, styleProvider.ZIndex( value.Value ) );
    }

    [Fact]
    public void IsDefault()
    {
        var value = (ZIndex)(int?)null;

        Assert.Equal( ZIndex.Default, value );
        Assert.True( value.IsDefault );
        Assert.Null( value.Name );
        Assert.Null( value.Value );
        Assert.Null( classProvider.ZIndex( value ) );
        Assert.Null( classProvider.ZIndex( null ) );
        Assert.Null( styleProvider.ZIndex( value.Value ) );
    }

    [Fact]
    public void AreLayers()
    {
        var layers = new (string Expected, ZIndex Layer)[]
        {
            ("z-n1", ZIndex.IsNegative1),
            ("z-0", ZIndex.Is0),
            ("z-1", ZIndex.Is1),
            ("z-2", ZIndex.Is2),
            ("z-3", ZIndex.Is3),
            ("z-dropdown", ZIndex.Dropdown),
            ("z-sticky", ZIndex.Sticky),
            ("z-fixed", ZIndex.Fixed),
            ("z-offcanvas-backdrop", ZIndex.OffcanvasBackdrop),
            ("z-offcanvas", ZIndex.Offcanvas),
            ("z-modal-backdrop", ZIndex.ModalBackdrop),
            ("z-modal", ZIndex.Modal),
            ("z-popover", ZIndex.Popover),
            ("z-tooltip", ZIndex.Tooltip),
            ("z-toast", ZIndex.Toast),
            ("z-snackbar", ZIndex.Snackbar),
            ("z-on-screen-keyboard", ZIndex.OnScreenKeyboard),
        };

        foreach ( var (expected, layer) in layers )
        {
            var classname = classProvider.ZIndex( layer );

            Assert.Equal( expected, classname );
            Assert.Null( layer.Value );
            Assert.False( layer.IsDefault );
            Assert.Null( styleProvider.ZIndex( layer.Value ) );
        }
    }

    [Fact]
    public void AreLevelsDistinctFromNumbers()
    {
        var levels = new[] { ZIndex.IsNegative1, ZIndex.Is0, ZIndex.Is1, ZIndex.Is2, ZIndex.Is3 };

        for ( var index = 0; index < levels.Length; index++ )
        {
            Assert.NotEqual( (ZIndex)( index - 1 ), levels[index] );
        }

        Assert.NotEqual( ZIndex.Default, ZIndex.Is0 );
    }
}